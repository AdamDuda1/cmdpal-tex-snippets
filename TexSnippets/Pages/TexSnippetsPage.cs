// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Commands;
using TexSnippets.Latex;

namespace TexSnippets.Pages;

/// <summary>
/// Live LaTeX scratchpad: renders whatever is typed, or offers a few starter snippets when empty.
/// This is the same compile step the fallback command uses, just with room for a details pane.
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

    private string _query = string.Empty;

    public TexSnippetsPage()
    {
        Icon = Icons.Tex;
        Title = "Tex Snippets";
        Name = "Open";
        PlaceholderText = @"Type a LaTeX snippet, e.g. \frac{\pi}{2}";
        ShowDetails = true;
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _query = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems() =>
        string.IsNullOrWhiteSpace(_query) ? Array.ConvertAll(Examples, Compile) : [Compile(_query)];

    /// <summary>Turns one snippet into a list item: preview plus copy commands, or the syntax error.</summary>
    private static ListItem Compile(string source)
    {
        var (preview, error) = LatexRenderer.Render(source);

        if (error is not null)
        {
            return new ListItem(new NoOpCommand())
            {
                Title = source,
                Subtitle = error,
                Icon = Icons.Error,
            };
        }

        return new ListItem(new CopyTextCommand(preview) { Name = "Copy preview" })
        {
            Title = preview,
            Subtitle = source,
            Icon = Icons.Tex,
            MoreCommands =
            [
                new CommandContextItem(new CopyTextCommand(source) { Name = "Copy LaTeX" }),
                new CommandContextItem(new CopyPngCommand { Source = source }),
            ],
            Details = new Details
            {
                Title = preview,
                Body = $"```latex\n{source}\n```",
            },
        };
    }
}
