// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Latex;
using Windows.Storage.Streams;

namespace TexSnippets.Interop;

/// <summary>
/// Wraps rendered PNG bytes as an <see cref="IconInfo"/> the host can show, without going near the
/// file system - Command Palette reads the image back through the stream reference on demand.
/// </summary>
internal static class PngIcon
{
    /// <summary>
    /// Builds the light and dark variants of one preview. Both come from the same LaTeX run; only
    /// the ink colour differs, so the dark-theme copy costs a recolour rather than a second compile.
    /// </summary>
    public static IconInfo FromPng(byte[] png, TexColor ink) =>
        new(Data(PngInk.Recolor(png, ink)), Data(PngInk.Recolor(png, ink.OnDarkBackground())));

    private static IconData Data(byte[] png)
    {
        // Deliberately not disposed: the reference below reads from this stream whenever the host
        // asks for the image, and it lives exactly as long as the IconInfo holding it.
        var stream = new InMemoryRandomAccessStream();

        var writer = new DataWriter(stream);
        writer.WriteBytes(png);
        writer.StoreAsync().AsTask().GetAwaiter().GetResult();
        writer.DetachStream();
        stream.Seek(0);

        return new IconData(RandomAccessStreamReference.CreateFromStream(stream));
    }
}
