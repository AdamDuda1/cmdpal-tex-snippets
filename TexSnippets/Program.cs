// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using System.Threading;
using Microsoft.CommandPalette.Extensions;
using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT;

namespace TexSnippets;

public static class Program
{
    [MTAThread]
    public static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] != "-RegisterProcessAsComServer")
        {
            Console.WriteLine("Not being launched as an extension... exiting.");
            return;
        }

        using var extensionDisposedEvent = new ManualResetEvent(false);

        // A single extension instance is handed out every time the host asks for the IExtension
        // object; the process exits as soon as that instance is disposed.
        var extension = new TexSnippets(extensionDisposedEvent);

        var server = new ComServer();
        server.RegisterClass<TexSnippets, IExtension>(() => extension);
        server.Start();

        extensionDisposedEvent.WaitOne();

        server.Stop();
        server.UnsafeDispose();
    }
}