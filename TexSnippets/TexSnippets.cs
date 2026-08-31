// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CommandPalette.Extensions;

namespace TexSnippets;

/// <summary>Extension entry point. The GUID must match the CLSID in Package.appxmanifest.</summary>
[Guid("d996289e-92cc-454f-bf99-9a10d357a20e")]
public sealed partial class TexSnippets(ManualResetEvent extensionDisposedEvent) : IExtension, IDisposable
{
    private readonly TexSnippetsCommandsProvider _provider = new();

    public object? GetProvider(ProviderType providerType) => providerType switch
    {
        ProviderType.Commands => _provider,
        _ => null,
    };

    public void Dispose() => extensionDisposedEvent.Set();
}
