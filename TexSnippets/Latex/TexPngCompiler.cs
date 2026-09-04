// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using TexSnippets.Interop;

namespace TexSnippets.Latex;

/// <summary>The outcome of a real LaTeX run.</summary>
/// <param name="Png">PNG bytes, or <see langword="null"/> when the run failed.</param>
/// <param name="Error">Human-readable failure, or <see langword="null"/> on success.</param>
internal readonly record struct TexPngResult(byte[]? Png, string? Error);

/// <summary>
/// Renders a snippet with the locally installed TeX distribution - <c>latex</c> produces a DVI and
/// <c>dvipng</c> turns it into a bitmap.
/// </summary>
internal static class TexPngCompiler
{
    /// <summary>Directories to try when the TeX binaries are not on PATH.</summary>
    private static readonly string[] FallbackDirectories =
    [
        @"C:\texlive\2025\bin\windows",
        @"C:\texlive\2024\bin\windows",
        @"C:\Program Files\MiKTeX\miktex\bin\x64",
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\MiKTeX\miktex\bin\x64"),
    ];

    /// <summary>Environments that already open math mode, so the body must not be wrapped in '$'.</summary>
    private static readonly string[] MathEnvironments =
        ["align", "alignat", "displaymath", "eqnarray", "equation", "flalign", "gather", "multline"];

    private static readonly Lock ToolchainLock = new();

    /// <summary>
    /// Last resolved pair of binaries, together with the override directory it was resolved for.
    /// Locating them walks PATH, so the result is worth keeping - but only until the user points the
    /// setting somewhere else.
    /// </summary>
    private static (string Directory, string? Latex, string? Dvipng)? _toolchain;

    /// <summary>Whether a usable TeX toolchain was found on this machine.</summary>
    public static bool IsAvailable(string toolDirectory)
    {
        var (latex, dvipng) = Toolchain(toolDirectory);
        return latex is not null && dvipng is not null;
    }

    public static TexPngResult Compile(string snippet, TexOptions options)
    {
        if (Toolchain(options.ToolDirectory) is not ({ } latex, { } dvipng))
        {
            return new TexPngResult(null, "No LaTeX installation found (looked for latex.exe and dvipng.exe).");
        }

        var workingDirectory = Directory.CreateTempSubdirectory("TexSnippets");

        try
        {
            var source = Path.Combine(workingDirectory.FullName, "snippet.tex");
            File.WriteAllText(source, Document(snippet, options.Preamble), new UTF8Encoding(false));

            var (latexExit, log) = Run(latex, "-interaction=nonstopmode -halt-on-error snippet.tex", workingDirectory.FullName);
            if (latexExit != 0)
            {
                return new TexPngResult(null, FirstTexError(log));
            }

            var background = options.Transparent ? "Transparent" : "\"rgb 1 1 1\"";

            var (dvipngExit, dvipngLog) = Run(
                dvipng,
                $"-q -D {options.Dpi} -T tight -bg {background} -fg \"{options.Ink.ToDvipng()}\" -o snippet.png snippet.dvi",
                workingDirectory.FullName);

            var image = Path.Combine(workingDirectory.FullName, "snippet.png");
            if (dvipngExit != 0 || !File.Exists(image))
            {
                return new TexPngResult(null, $"dvipng failed: {FirstLine(dvipngLog)}");
            }

            // dvipng crops flush to the ink, which looks cramped once pasted. Scale the margin
            // with the resolution so the proportions hold whatever dpi the caller asks for.
            var margin = options.Dpi / 12;
            var fill = options.Transparent ? (TexColor?)null : new TexColor(255, 255, 255);

            return new TexPngResult(PngPadding.Expand(File.ReadAllBytes(image), margin, fill), null);
        }
        catch (IOException ex)
        {
            return new TexPngResult(null, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return new TexPngResult(null, ex.Message);
        }
        finally
        {
            TryDelete(workingDirectory);
        }
    }

    /// <summary>Wraps the snippet in the smallest document that still typesets it.</summary>
    /// <remarks>
    /// The preamble follows what the TeXit Discord bot does, because that is the look people
    /// recognise: stock Computer Modern - no font package anywhere - and <c>\everymath</c> forcing
    /// display style, so fractions, sums and integrals get their full-size shapes rather than the
    /// squashed inline ones. There is deliberately no <c>\everydisplay</c>: it makes the AMS
    /// multi-line environments fail with "Improper \halign inside $$'s".
    /// </remarks>
    private static string Document(string snippet, string preamble)
    {
        var body = Unwrap(snippet.Trim());

        // Environments such as align already carry their own math mode; everything else needs one.
        var math = IsOwnMathMode(body) ? body : $"${body}$";

        // Three '$' so that the doubled closing braces in \IfFileExists are literal text
        // rather than the end of an interpolation hole.
        var extra = string.IsNullOrWhiteSpace(preamble) ? string.Empty : preamble.Trim() + "\n";

        return $$$"""
            \documentclass[12pt]{article}
            \usepackage{amsmath}
            \usepackage{amssymb}
            \usepackage{amsfonts}
            \IfFileExists{mathtools.sty}{\usepackage{mathtools}}{}
            \everymath{\displaystyle}
            \pagestyle{empty}
            {{{extra}}}\begin{document}
            {{{math}}}
            \end{document}

            """;
    }

    /// <summary>Strips math delimiters the user may have typed, so they are not doubled up.</summary>
    private static string Unwrap(string body) => body switch
    {
        ['$', '$', .. var inner, '$', '$'] => inner,
        ['$', .. var inner, '$'] => inner,
        ['\\', '[', .. var inner, '\\', ']'] => inner,
        ['\\', '(', .. var inner, '\\', ')'] => inner,
        _ => body,
    };

    private static bool IsOwnMathMode(string body)
    {
        foreach (var environment in MathEnvironments)
        {
            if (body.StartsWith($@"\begin{{{environment}}}", StringComparison.Ordinal) ||
                body.StartsWith($@"\begin{{{environment}*}}", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static (int ExitCode, string Output) Run(string exe, string arguments, string workingDirectory)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo(exe, arguments)
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            },
        };

        process.Start();
        process.StandardInput.Close();
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();

        if (!process.WaitForExit(20_000))
        {
            process.Kill(entireProcessTree: true);
            return (-1, "timed out");
        }

        return (process.ExitCode, output);
    }

    /// <summary>Pulls the first '!' line out of a TeX log, plus the input it choked on.</summary>
    private static string FirstTexError(string log)
    {
        var lines = log.Split('\n');

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (!line.StartsWith('!'))
            {
                continue;
            }

            var message = line.TrimStart('!').Trim();

            // TeX follows the message with "l.<n> <the input up to the problem>".
            for (var j = i + 1; j < lines.Length && j < i + 6; j++)
            {
                var context = lines[j].Trim();
                var space = context.IndexOf(' ', StringComparison.Ordinal);

                if (context.StartsWith("l.", StringComparison.Ordinal) && space > 0)
                {
                    return $"{message} - near: {context[(space + 1)..].Trim()}";
                }
            }

            return message;
        }

        return $"LaTeX failed: {FirstLine(log)}";
    }

    private static string FirstLine(string text)
    {
        foreach (var line in text.Split('\n'))
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                return line.Trim();
            }
        }

        return "no output";
    }

    /// <summary>
    /// The two binaries, resolved for <paramref name="toolDirectory"/> and cached until it changes.
    /// </summary>
    private static (string? Latex, string? Dvipng) Toolchain(string? toolDirectory)
    {
        var directory = toolDirectory?.Trim() ?? string.Empty;

        lock (ToolchainLock)
        {
            if (_toolchain is { } cached && cached.Directory == directory) return (cached.Latex, cached.Dvipng);
            cached = (directory, Locate("latex.exe", directory), Locate("dvipng.exe", directory));
            _toolchain = cached;

            return (cached.Latex, cached.Dvipng);
        }
    }

    private static string? Locate(string fileName, string toolDirectory)
    {
        if (toolDirectory.Length > 0 && TryPath(toolDirectory, fileName, out var configured))
            return configured;

        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator))
        {
            if (directory.Length > 0 && TryPath(directory, fileName, out var onPath))
            {
                return onPath;
            }
        }

        foreach (var directory in FallbackDirectories)
        {
            if (TryPath(directory, fileName, out var known))
            {
                return known;
            }
        }

        return null;
    }

    private static bool TryPath(string directory, string fileName, out string? path)
    {
        path = null;

        try
        {
            var candidate = Path.Combine(directory, fileName);
            if (File.Exists(candidate))
            {
                path = candidate;
                return true;
            }
        }
        catch (ArgumentException)
        {
            // A malformed PATH entry is not worth reporting; just skip it.
        }

        return false;
    }

    private static void TryDelete(DirectoryInfo directory)
    {
        try
        {
            directory.Delete(recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
