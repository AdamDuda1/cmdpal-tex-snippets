// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Pages;
using TexSnippets.Settings;

namespace TexSnippets;

public sealed partial class TexSnippetsCommandsProvider : CommandProvider
{
    private readonly SettingsManager _settings = new();
    private readonly ICommandItem[] _commands;
    private readonly IFallbackCommandItem[] _fallbacks;

    public TexSnippetsCommandsProvider()
    {
        DisplayName = "Tex Snippets";
        Icon = Icons.Tex;

        Settings = _settings.Settings;

        _fallbacks = [new TexFallbackItem(_settings), new TexPngFallbackItem(_settings)];

        _commands =
        [
            new CommandItem(new TexSnippetsPage(_settings))
            {
                Title = DisplayName,

                MoreCommands =
                [
                    new CommandContextItem(_settings.Settings.SettingsPage)
                    {
                        Title = "Settings",
                        Icon = Icons.Settings,
                    },
                ],
            },
        ];
    }

    public override ICommandItem[] TopLevelCommands() => _commands;

    public override IFallbackCommandItem[] FallbackCommands() => _fallbacks;
}
