// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace TexSnippets.Interop;

/// <summary>
/// The straight-BGRA8 round trip shared by everything here that edits a PNG in memory. Straight
/// rather than premultiplied alpha because that is what <see cref="ImageClipboard"/> expects, and
/// because it keeps dvipng's anti-aliased edges recolourable.
/// </summary>
internal static class PngImage
{
    public static (int Width, int Height, byte[] Bgra) Decode(byte[] png)
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

    public static byte[] Encode(int width, int height, byte[] bgra)
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
