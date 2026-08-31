// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TexSnippets.Latex;

/// <summary>The outcome of a real LaTeX run: the rendered image, or the first error TeX reported.</summary>
/// <param name="Png">PNG bytes, or <see langword="null"/> when the run failed.</param>
/// <param name="Error">Human-readable failure, or <see langword="null"/> on success.</param>
internal readonly record struct TexPngResult(byte[]? Png, string? Error);

/// <summary>
/// Renders a snippet with the locally installed TeX distribution: <c>latex</c> produces a DVI and
/// <c>dvipng</c> turns it into a bitmap. That round trip costs about a second, so it is only ever
/// run when the user picks the command — never while they are typing.
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

    private static readonly Lazy<string?> LatexExe = new(() => Locate("latex.exe"));
    private static readonly Lazy<string?> DvipngExe = new(() => Locate("dvipng.exe"));

    /// <summary>Whether a usable TeX toolchain was found on this machine.</summary>
    public static bool IsAvailable => LatexExe.Value is not null && DvipngExe.Value is not null;

    public static TexPngResult Compile(string snippet, int dpi = 600)
    {
        if (LatexExe.Value is not { } latex || DvipngExe.Value is not { } dvipng)
        {
            return new TexPngResult(null, "No LaTeX installation found (looked for latex.exe and dvipng.exe).");
        }

        var workingDirectory = Directory.CreateTempSubdirectory("TexSnippets");

        try
        {
            var source = Path.Combine(workingDirectory.FullName, "snippet.tex");
            File.WriteAllText(source, Document(snippet), new UTF8Encoding(false));

            var (latexExit, log) = Run(latex, "-interaction=nonstopmode -halt-on-error snippet.tex", workingDirectory.FullName);
            if (latexExit != 0)
            {
                return new TexPngResult(null, FirstTexError(log));
            }

            var (dvipngExit, dvipngLog) = Run(
                dvipng,
                $"-q -D {dpi} -T tight -bg Transparent -o snippet.png snippet.dvi",
                workingDirectory.FullName);

            var image = Path.Combine(workingDirectory.FullName, "snippet.png");
            if (dvipngExit != 0 || !File.Exists(image))
            {
                return new TexPngResult(null, $"dvipng failed: {FirstLine(dvipngLog)}");
            }

            return new TexPngResult(File.ReadAllBytes(image), null);
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
    private static string Document(string snippet)
    {
        var body = Unwrap(snippet.Trim());

        // Environments such as align already carry their own math mode; everything else needs one.
        var math = IsOwnMathMode(body) ? body : $@"$\displaystyle {body}$";

        return $$"""
            \documentclass[12pt]{article}
            \usepackage{amsmath}
            \usepackage{amssymb}
            \pagestyle{empty}
            \begin{document}
            {{math}}
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
                    return $"{message} — near: {context[(space + 1)..].Trim()}";
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

    /// <summary>Finds a TeX binary on PATH, then in the usual TeX Live / MiKTeX locations.</summary>
    private static string? Locate(string fileName)
    {
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
