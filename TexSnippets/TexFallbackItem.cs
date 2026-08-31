// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Commands;
using TexSnippets.Latex;

namespace TexSnippets;

/// <summary>
/// Catch-all item that compiles whatever LaTeX the user types straight in the search box —
/// the same shape as the built-in Calculator fallback. Enter copies the Unicode preview,
/// and the context menu copies the LaTeX source instead.
/// </summary>
internal sealed partial class TexFallbackItem : FallbackCommandItem
{
    private readonly CopyTexCommand _copyPreview;
    private readonly CopyTexCommand _copySource;

    public TexFallbackItem()
        : this(new CopyTexCommand("Copy preview"), new CopyTexCommand("Copy LaTeX"))
    {
    }

    private TexFallbackItem(CopyTexCommand copyPreview, CopyTexCommand copySource)
        : base(copyPreview, "LaTeX preview", "TexSnippets.Fallback")
    {
        _copyPreview = copyPreview;
        _copySource = copySource;

        Icon = Icons.Tex;
        MoreCommands = [new CommandContextItem(copySource)];

        // the empty title keeps the item hidden until the query actually looks like tex
        Title = string.Empty;
    }

    public override void UpdateQuery(string query)
    {
        if (!LatexRenderer.LooksLikeTex(query))
        {
            Show(string.Empty, string.Empty, Icons.Tex, preview: string.Empty, source: string.Empty);
            return;
        }

        var (preview, error) = LatexRenderer.Render(query);

        if (error is not null)
        {
            Show(query, error, Icons.Error, preview: string.Empty, source: query);
            return;
        }

        Show(preview, query, Icons.Tex, preview, source: query);
    }

    private void Show(string title, string subtitle, IconInfo icon, string preview, string source)
    {
        Title = title;
        Icon = icon;
        Subtitle = subtitle;
        _copyPreview.Text = preview;
        _copySource.Text = source;
    }
}
