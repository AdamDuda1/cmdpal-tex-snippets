// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Interop;
using TexSnippets.Latex;
using TexSnippets.Settings;

namespace TexSnippets.Pages;

/// <summary>What the details pane knows about one snippet: the typeset image, or why there isn't one.</summary>
/// <param name="Image">The rendered preview, or <see langword="null"/> while it is still unknown.</param>
/// <param name="Error">The LaTeX failure, or <see langword="null"/>.</param>
internal readonly record struct PreviewResult(IconInfo? Image, string? Error);

/// <summary>Runs the real LaTeX toolchain behind the typing and hands the result to the details pane.</summary>
internal sealed class LivePreview(SettingsManager settings, Action onChanged)
{
    private const int DebounceMilliseconds = 400;
    private const int CacheLimit = 32;
    private readonly Lock _gate = new();
    private readonly Dictionary<string, PreviewResult> _cache = new(StringComparer.Ordinal);

    private long _generation;

    private volatile bool _busy;
    private string _stamp = string.Empty;
    private string? _inFlight;

    // Only for the progress bar:
    public bool IsBusy => _busy;

    public PreviewResult Request(string source)
    {
        if (!settings.LivePreview || string.IsNullOrWhiteSpace(source))
        {
            Supersede();
            return default;
        }

        var options = settings.ForPreview();

        lock (_gate)
        {
            Restamp(options);

            if (_cache.TryGetValue(source, out var cached))
            {
                Supersede();
                return cached;
            }

            if (_busy && string.Equals(_inFlight, source, StringComparison.Ordinal))
                return default;

            _inFlight = source;
        }

        Schedule(source, options);
        return default;
    }

    /// <summary>Drops everything rendered so far, for when the settings behind it have changed.</summary>
    public void Invalidate()
    {
        lock (_gate)
        {
            _cache.Clear();
            _stamp = string.Empty;
        }

        Supersede();
    }

    /// <summary>Anything that changes what a compile would produce empties the cache.</summary>
    private void Restamp(TexOptions options)
    {
        var stamp = string.Create(
            CultureInfo.InvariantCulture,
            $"{options.Dpi}{settings.Ink}{options.Preamble}{options.ToolDirectory}");

        if (_stamp == stamp) return;
        _stamp = stamp;
        _cache.Clear();
    }

    private void Schedule(string source, TexOptions options)
    {
        var generation = Interlocked.Increment(ref _generation);
        _busy = true;

        _ = Task.Run(async () =>
        {
            await Task.Delay(DebounceMilliseconds).ConfigureAwait(false);

            if (Interlocked.Read(ref _generation) != generation)
                return;

            var result = Compile(source, options);

            if (Interlocked.Read(ref _generation) != generation)
                return;

            lock (_gate)
            {
                if (_cache.Count >= CacheLimit)
                    _cache.Clear();

                _cache[source] = result;
            }

            _busy = false;
            onChanged();
        });
    }

    private PreviewResult Compile(string source, TexOptions options)
    {
        if (!TexPngCompiler.IsAvailable(options.ToolDirectory))
            return new PreviewResult(null, "No LaTeX installation found, so there is nothing to typeset with.");

        var (png, error) = TexPngCompiler.Compile(source, options);

        return png is null
            ? new PreviewResult(null, error)
            : new PreviewResult(PngIcon.FromPng(png, TexColor.Parse(settings.Ink)), null);
    }

    /// <summary>Makes any scheduled compile give up quietly the next time it looks.</summary>
    private void Supersede()
    {
        Interlocked.Increment(ref _generation);
        _inFlight = null;
        _busy = false;
    }
}
