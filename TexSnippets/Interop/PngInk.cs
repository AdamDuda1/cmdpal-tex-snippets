// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using TexSnippets.Latex;

namespace TexSnippets.Interop;

/// <summary>
/// Repaints a transparent-background PNG in another colour.
/// </summary>
/// <remarks>
/// Cheap because of how <c>dvipng -bg Transparent</c> works: every pixel carries the same ink
/// colour and only the alpha channel varies, so swapping the colour is a straight overwrite of the
/// three colour channels and the anti-aliasing survives untouched. That saves a second LaTeX run
/// when the details pane needs a light-theme and a dark-theme copy of the same preview.
/// </remarks>
internal static class PngInk
{
    public static byte[] Recolor(byte[] png, TexColor ink)
    {
        ArgumentNullException.ThrowIfNull(png);

        var (width, height, bgra) = PngImage.Decode(png);

        for (var i = 0; i < bgra.Length; i += 4)
        {
            // Fully transparent pixels have no colour to preserve, but writing them anyway keeps
            // the loop branchless and the result identical.
            bgra[i] = ink.B;
            bgra[i + 1] = ink.G;
            bgra[i + 2] = ink.R;
        }

        return PngImage.Encode(width, height, bgra);
    }
}
