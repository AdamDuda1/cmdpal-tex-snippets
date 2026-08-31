// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions.Toolkit;

namespace TexSnippets;

/// <summary>Icons shared by the extension's pages, items and commands.</summary>
internal static class Icons
{
    /// <summary>The extension's own logo.</summary>
    public static IconInfo Tex { get; } = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");

    /// <summary>Segoe Fluent "Copy".</summary>
    public static IconInfo Copy { get; } = new("");

    /// <summary>Segoe Fluent "Error badge".</summary>
    public static IconInfo Error { get; } = new("");

    /// <summary>Segoe Fluent "Photo", used for the PNG output.</summary>
    public static IconInfo Image { get; } = new("");
}
