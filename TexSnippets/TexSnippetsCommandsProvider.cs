// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Pages;

namespace TexSnippets;

public sealed partial class TexSnippetsCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;
    private readonly IFallbackCommandItem[] _fallbacks = [new TexFallbackItem(), new TexPngFallbackItem()];

    public TexSnippetsCommandsProvider()
    {
        DisplayName = "Tex Snippets";
        Icon = Icons.Tex;
        _commands = [new CommandItem(new TexSnippetsPage()) { Title = DisplayName }];
    }

    public override ICommandItem[] TopLevelCommands() => _commands;

    public override IFallbackCommandItem[] FallbackCommands() => _fallbacks;
}
