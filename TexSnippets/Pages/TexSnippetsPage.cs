// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Commands;
using TexSnippets.Latex;
using TexSnippets.Settings;

namespace TexSnippets.Pages;

/// <summary>
/// Live LaTeX scratchpad: renders whatever is typed, or offers a few starter snippets when empty.
/// This is the same compile step the fallback command uses, just with room for a details pane -
/// and, when the setting is on, a typeset preview of the snippet alongside the Unicode one.
/// </summary>
internal sealed partial class TexSnippetsPage : DynamicListPage
{
    private static readonly string[] Examples =
    [
        @"\frac{-b \pm \sqrt{b^2 - 4ac}}{2a}",
        @"\int_0^\infty e^{-x^2}\,dx = \frac{\sqrt{\pi}}{2}",
        @"\sum_{n=1}^{\infty} \frac{1}{n^2} = \frac{\pi^2}{6}",
        @"\forall \epsilon > 0\ \exists \delta > 0",
        @"x \in \mathbb{R} \setminus \mathbb{Q}",
    ];

    private readonly SettingsManager _settings;
    private readonly LivePreview _preview;

    private string _query = string.Empty;

    public TexSnippetsPage(SettingsManager settings)
    {
        _settings = settings;
        _preview = new LivePreview(settings, () => RaiseItemsChanged());

        Icon = Icons.Tex;
        Title = "Tex Snippets";
        Name = "Open";
        PlaceholderText = @"Type a LaTeX snippet, e.g. \frac{\pi}{2}";
        ShowDetails = true; // !!!

        settings.Changed += (_, _) =>
        {
            _preview.Invalidate();
            RaiseItemsChanged();
        };
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _query = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        if (string.IsNullOrWhiteSpace(_query))
        {
            IsLoading = false;
            return Array.ConvertAll(Examples, source => Compile(source, default));
        }

        // only the snippet the user is actually working on is worth typesetting
        // the starter examples above would be five LaTeX runs for something nobody asked to see.
        var preview = _preview.Request(_query);
        IsLoading = _preview.IsBusy;

        return [Compile(_query, preview)];
    }

    /// <summary>Turns one snippet into a list item: preview plus copy commands, or the syntax error.</summary>
    private ListItem Compile(string source, PreviewResult preview)
    {
        var (unicode, error) = LatexRenderer.Render(source);

        if (error is not null)
        {
            return new ListItem(new NoOpCommand())
            {
                Title = source,
                Subtitle = error,
                Icon = Icons.Error,
            };
        }

        var details = new Details
        {
            Title = unicode,
            Body = Body(source, preview.Error),
        };

        if (preview.Image is { } image)
        {
            details.HeroImage = image;
        }

        return new ListItem(new CopyTextCommand(unicode) { Name = "Copy preview" })
        {
            Title = unicode,
            Subtitle = source,
            Icon = Icons.Tex,
            MoreCommands =
            [
                new CommandContextItem(new CopyPngCommand(_settings) { Source = source }),
                new CommandContextItem(new CopyTextCommand(source) { Name = "Copy LaTeX" })
            ],
            Details = details,
        };
    }

    private static string Body(string source, string? previewError)
    {
        var body = $"Main action - copy unicode,\n\nSecondary action - copy PNG.\n```latex\n{source}\n```";

        return previewError is null ? body : $"{body}\nPreview: {previewError}";
    }
}
