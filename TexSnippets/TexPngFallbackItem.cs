// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Commands;
using TexSnippets.Latex;
using TexSnippets.Settings;

namespace TexSnippets;

/// <summary>
/// The second half of the fallback pair: where <see cref="TexFallbackItem"/> offers the instant
/// Unicode approximation, this one offers the real thing — typeset by LaTeX and copied as an image.
/// </summary>
internal sealed partial class TexPngFallbackItem : FallbackCommandItem
{
    private readonly CopyPngCommand _copyPng;
    private readonly SettingsManager _settings;

    public TexPngFallbackItem(SettingsManager settings)
        : this(new CopyPngCommand(settings), settings)
    {
    }

    private TexPngFallbackItem(CopyPngCommand copyPng, SettingsManager settings)
        : base(copyPng, "LaTeX as PNG", "TexSnippets.Fallback.Png")
    {
        _copyPng = copyPng;
        _settings = settings;

        Icon = Icons.Image;
        Subtitle = "Typeset with LaTeX and copy as an image";

        Title = string.Empty;
    }

    public override void UpdateQuery(string query)
    {
        var offer = TexPngCompiler.IsAvailable(_settings.TexDirectory)
            && LatexRenderer.LooksLikeTex(query)
            && !LatexRenderer.Render(query).IsError;

        Title = offer ? query : string.Empty;
        _copyPng.Source = offer ? query : string.Empty;
    }
}
