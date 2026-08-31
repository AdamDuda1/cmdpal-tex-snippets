// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace TexSnippets.Interop;

/// <summary>
/// Adds a transparent margin around a PNG.
/// </summary>
/// <remarks>
/// This cannot be done in the TeX toolchain: <c>dvipng -T tight</c> crops to the ink, so any
/// whitespace LaTeX adds is cropped straight back off, and <c>-bd</c> only tints the edge pixels
/// rather than growing the canvas. Doing it here keeps the tight crop - which is the only mode
/// that sizes multi-line environments correctly - and still leaves the equation room to breathe.
/// </remarks>
internal static class PngPadding
{
    /// <summary>Returns <paramref name="png"/> with <paramref name="margin"/> transparent pixels on every side.</summary>
    public static byte[] Expand(byte[] png, int margin)
    {
        ArgumentNullException.ThrowIfNull(png);

        if (margin <= 0)
        {
            return png;
        }

        var (width, height, bgra) = Decode(png);
        var paddedWidth = width + (margin * 2);
        var paddedHeight = height + (margin * 2);
        var padded = new byte[paddedWidth * paddedHeight * 4];

        // The new buffer starts fully zeroed, which in straight BGRA8 is transparent already;
        // only the original rows need copying, each shifted right and down by the margin.
        for (var y = 0; y < height; y++)
        {
            var source = y * width * 4;
            var destination = (((y + margin) * paddedWidth) + margin) * 4;
            System.Buffer.BlockCopy(bgra, source, padded, destination, width * 4);
        }

        return Encode(paddedWidth, paddedHeight, padded);
    }

    /// <summary>Decodes to straight BGRA8, matching what <see cref="ImageClipboard"/> does.</summary>
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

    private static byte[] Encode(int width, int height, byte[] bgra)
    {
        using var stream = new InMemoryRandomAccessStream();

        var encoder = BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream).AsTask().GetAwaiter().GetResult();
        encoder.SetPixelData(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Straight,
            (uint)width,
            (uint)height,
            96,
            96,
            bgra);
        encoder.FlushAsync().AsTask().GetAwaiter().GetResult();

        stream.Seek(0);
        var bytes = new byte[stream.Size];
        var reader = new DataReader(stream);
        reader.LoadAsync((uint)stream.Size).AsTask().GetAwaiter().GetResult();
        reader.ReadBytes(bytes);
        reader.DetachStream();

        return bytes;
    }
}
