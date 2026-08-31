// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TexSnippets.Commands;

/// <summary>
/// Copies a piece of text to the clipboard. Unlike the toolkit's <c>CopyTextCommand</c>, the text is
/// mutable, so a single instance can follow the query as the user types.
/// </summary>
internal sealed partial class CopyTexCommand : InvokableCommand
{
    public CopyTexCommand(string name)
    {
        Name = name;
        Icon = Icons.Copy;
    }

    public string Text { get; set; } = string.Empty;

    public override CommandResult Invoke()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return CommandResult.KeepOpen();
        }

        ClipboardHelper.SetText(Text);
        return CommandResult.ShowToast(new ToastArgs { Message = "Copied to clipboard", Result = CommandResult.Dismiss() });
    }
}
