// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using TexSnippets.Latex;

namespace TexSnippets.Interop;

/// <summary>
/// Adds a margin around a PNG.
/// </summary>
/// <remarks>
/// This cannot be done in the TeX toolchain: <c>dvipng -T tight</c> crops to the ink, so any
/// whitespace LaTeX adds is cropped straight back off, and <c>-bd</c> only tints the edge pixels
/// rather than growing the canvas. Doing it here keeps the tight crop - which is the only mode
/// that sizes multi-line environments correctly - and still leaves the equation room to breathe.
/// </remarks>
internal static class PngPadding
{
    /// <summary>
    /// Returns <paramref name="png"/> with <paramref name="margin"/> extra pixels on every side,
    /// transparent unless <paramref name="background"/> says otherwise.
    /// </summary>
    public static byte[] Expand(byte[] png, int margin, TexColor? background = null)
    {
        ArgumentNullException.ThrowIfNull(png);

        if (margin <= 0)
        {
            return png;
        }

        var (width, height, bgra) = PngImage.Decode(png);
        var paddedWidth = width + (margin * 2);
        var paddedHeight = height + (margin * 2);
        var padded = new byte[paddedWidth * paddedHeight * 4];

        // A zeroed buffer is already transparent in straight BGRA8, so only an opaque background
        // needs painting before the original rows are copied in.
        if (background is { } fill)
        {
            for (var i = 0; i < padded.Length; i += 4)
            {
                padded[i] = fill.B;
                padded[i + 1] = fill.G;
                padded[i + 2] = fill.R;
                padded[i + 3] = 255;
            }
        }

        // Each original row is shifted right and down by the margin.
        for (var y = 0; y < height; y++)
        {
            var source = y * width * 4;
            var destination = (((y + margin) * paddedWidth) + margin) * 4;
            System.Buffer.BlockCopy(bgra, source, padded, destination, width * 4);
        }

        return PngImage.Encode(paddedWidth, paddedHeight, padded);
    }
}
