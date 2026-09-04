// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Interop;
using TexSnippets.Latex;
using TexSnippets.Settings;

namespace TexSnippets.Commands;

/// <summary>
/// Typesets the snippet with the real LaTeX toolchain and puts the resulting image on the clipboard.
/// The compile happens here, on invoke, rather than while the user types. Takes about a second on my
/// mid-end machine.
/// </summary>
internal sealed partial class CopyPngCommand : InvokableCommand
{
    private readonly SettingsManager _settings;

    public CopyPngCommand(SettingsManager settings)
    {
        _settings = settings;
        Name = "Copy as PNG";
        Icon = Icons.Image;
    }

    public string Source { get; set; } = string.Empty;

    public override CommandResult Invoke()
    {
        if (string.IsNullOrWhiteSpace(Source))
            return CommandResult.KeepOpen();

        var (png, error) = TexPngCompiler.Compile(Source, _settings.ForClipboard());

        if (png is null)
            return Toast(error ?? "LaTeX failed.", CommandResult.KeepOpen());

        try
        {
            ImageClipboard.SetPng(png);
        }
        catch (InvalidOperationException ex)
        {
            return Toast(ex.Message, CommandResult.KeepOpen());
        }

        return Toast("Copied PNG to clipboard", CommandResult.Dismiss());
    }

    private static CommandResult Toast(string message, CommandResult next) =>
        CommandResult.ShowToast(new ToastArgs { Message = message, Result = next });
}
