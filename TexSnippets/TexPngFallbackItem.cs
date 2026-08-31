// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Commands;
using TexSnippets.Latex;

namespace TexSnippets;

/// <summary>
/// The second half of the fallback pair: where <see cref="TexFallbackItem"/> offers the instant
/// Unicode approximation, this one offers the real thing — typeset by LaTeX and copied as an image.
/// </summary>
internal sealed partial class TexPngFallbackItem : FallbackCommandItem
{
    private readonly CopyPngCommand _copyPng;

    public TexPngFallbackItem()
        : this(new CopyPngCommand())
    {
    }

    private TexPngFallbackItem(CopyPngCommand copyPng)
        : base(copyPng, "LaTeX as PNG", "TexSnippets.Fallback.Png")
    {
        _copyPng = copyPng;

        Icon = Icons.Image;
        Subtitle = "Typeset with LaTeX and copy as an image";

        // An empty title keeps the item hidden until the query actually looks like TeX.
        Title = string.Empty;
    }

    public override void UpdateQuery(string query)
    {
        // No point offering the row without a TeX install, nor for a snippet that is already
        // malformed — the cheap structural check saves the user a second-long compile that would fail.
        var offer = TexPngCompiler.IsAvailable
            && LatexRenderer.LooksLikeTex(query)
            && !LatexRenderer.Render(query).IsError;

        Title = offer ? query : string.Empty;
        _copyPng.Source = offer ? query : string.Empty;
    }
}
