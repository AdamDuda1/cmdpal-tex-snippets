// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace TexSnippets.Interop;

/// <summary>
/// Puts an image on the clipboard. The toolkit's <c>ClipboardHelper</c> only handles text, so this
/// mirrors what it does for text: raw Win32 calls, marshalled onto an STA thread.
/// </summary>
/// <remarks>
/// Two formats are published at once because applications disagree about what they want:
/// the registered "PNG" format keeps transparency (browsers, Slack, Word, Notion), while
/// <c>CF_DIB</c> — flattened onto white — is what the older image editors read.
/// </remarks>
internal static partial class ImageClipboard
{
    private const uint CF_DIB = 8;
    private const uint GMEM_MOVEABLE = 0x0002;

    /// <summary>Another process may hold the clipboard open; Windows itself retries in the same way.</summary>
    private const int OpenAttempts = 10;

    public static void SetPng(byte[] png)
    {
        ArgumentNullException.ThrowIfNull(png);

        var (width, height, bgra) = Decode(png);
        var dib = ToDib(width, height, bgra);

        OnStaThread(() =>
        {
            if (!TryOpenClipboard())
            {
                throw new InvalidOperationException("Could not open the clipboard; another application is holding it.");
            }

            try
            {
                EmptyClipboard();
                Publish(RegisterClipboardFormat("PNG"), png);
                Publish(CF_DIB, dib);
            }
            finally
            {
                CloseClipboard();
            }
        });
    }

    /// <summary>Hands one buffer to the clipboard, which takes ownership of the memory on success.</summary>
    private static void Publish(uint format, byte[] bytes)
    {
        if (format == 0)
        {
            return;
        }

        var handle = GlobalAlloc(GMEM_MOVEABLE, (nuint)bytes.Length);
        if (handle == IntPtr.Zero)
        {
            throw new InvalidOperationException("Out of memory while copying to the clipboard.");
        }

        var block = GlobalLock(handle);
        if (block == IntPtr.Zero)
        {
            GlobalFree(handle);
            throw new InvalidOperationException("Could not lock the clipboard buffer.");
        }

        Marshal.Copy(bytes, 0, block, bytes.Length);
        GlobalUnlock(handle);

        if (SetClipboardData(format, handle) == IntPtr.Zero)
        {
            GlobalFree(handle);
        }
    }

    /// <summary>Decodes the PNG to straight BGRA8 using the imaging codecs Windows already ships.</summary>
    private static (int Width, int Height, byte[] Bgra) Decode(byte[] png)
    {
        using var stream = new InMemoryRandomAccessStream();

        var writer = new DataWriter(stream);
        writer.WriteBytes(png);
        writer.StoreAsync().AsTask().GetAwaiter().GetResult();
        writer.DetachStream();
        stream.Seek(0);

        var decoder = BitmapDecoder.CreateAsync(stream).AsTask().GetAwaiter().GetResult();
        var pixels = decoder.GetPixelDataAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Straight,
            new BitmapTransform(),
            ExifOrientationMode.IgnoreExifOrientation,
            ColorManagementMode.DoNotColorManage).AsTask().GetAwaiter().GetResult();

        return ((int)decoder.PixelWidth, (int)decoder.PixelHeight, pixels.DetachPixelData());
    }

    /// <summary>
    /// Packs the pixels into a 24-bit bottom-up DIB, compositing onto white. Dropping the alpha
    /// channel is deliberate: 32-bit DIBs on the clipboard are read inconsistently by older apps.
    /// </summary>
    private static byte[] ToDib(int width, int height, byte[] bgra)
    {
        const int HeaderSize = 40;
        var stride = ((width * 3) + 3) / 4 * 4;
        var dib = new byte[HeaderSize + (stride * height)];

        // BITMAPINFOHEADER
        BitConverter.TryWriteBytes(dib.AsSpan(0), HeaderSize);
        BitConverter.TryWriteBytes(dib.AsSpan(4), width);
        BitConverter.TryWriteBytes(dib.AsSpan(8), height);
        BitConverter.TryWriteBytes(dib.AsSpan(12), (short)1);      // planes
        BitConverter.TryWriteBytes(dib.AsSpan(14), (short)24);     // bits per pixel
        BitConverter.TryWriteBytes(dib.AsSpan(20), stride * height);

        for (var y = 0; y < height; y++)
        {
            var source = y * width * 4;

            // DIB rows run bottom-up.
            var target = HeaderSize + ((height - 1 - y) * stride);

            for (var x = 0; x < width; x++)
            {
                var alpha = bgra[source + 3];

                dib[target] = Flatten(bgra[source], alpha);
                dib[target + 1] = Flatten(bgra[source + 1], alpha);
                dib[target + 2] = Flatten(bgra[source + 2], alpha);

                source += 4;
                target += 3;
            }
        }

        return dib;
    }

    private static byte Flatten(byte channel, byte alpha) =>
        (byte)((channel * alpha / 255) + (255 - alpha));

    private static bool TryOpenClipboard()
    {
        for (var attempt = 0; attempt < OpenAttempts; attempt++)
        {
            if (OpenClipboard(IntPtr.Zero))
            {
                return true;
            }

            Thread.Sleep(50);
        }

        return false;
    }

    /// <summary>The clipboard is an STA-only API, and the extension host runs us in an MTA.</summary>
    private static void OnStaThread(Action action)
    {
        Exception? failure = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                failure = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure is not null)
        {
            throw failure;
        }
    }

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenClipboard(IntPtr hWndNewOwner);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseClipboard();

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EmptyClipboard();

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    [LibraryImport("user32.dll", EntryPoint = "RegisterClipboardFormatW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    private static partial uint RegisterClipboardFormat(string lpszFormat);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GlobalAlloc(uint uFlags, nuint dwBytes);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GlobalLock(IntPtr hMem);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GlobalUnlock(IntPtr hMem);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GlobalFree(IntPtr hMem);
}
