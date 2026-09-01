<div align="center">



# <img src="TexSnippets/Assets/StoreLogo.png" height="23" alt="Tex Snippets logo" /> Tex Snippets

A [Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview) extension. **Write LaTeX math without leaving the launcher.**<br />
Type a snippet, see it rendered, copy it as Unicode, source, or a typeset image.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Platform: Windows](https://img.shields.io/badge/Platform-Windows%2010%2B-0078D4.svg)](#requirements)
[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4.svg)](https://dotnet.microsoft.com/)

</div>

---

## What it does

* **Live preview** 

  Open *Tex Snippets* and type. Each keystroke is parsed and rendered to a Unicode approximation. Malformed input shows the syntax error instead of a preview.


* **Fallback commands**

  Type LaTeX straight into the Command Palette search box and two rows appear without opening the extension first: *LaTeX preview* and *LaTeX as PNG*.


* **Three ways to copy** 

  The Unicode preview (`α + β ≤ γ`), the raw LaTeX source, or a properly typeset PNG placed on the clipboard.


### Example

```
\frac{-b \pm \sqrt{b^2 - 4ac}}{2a}      →   (-b ± √(b² - 4ac))/(2a)
\sum_{n=1}^{\infty} \frac{1}{n^2}       →   ∑ₙ₌₁^∞ 1/(n²)
\int_0^\infty e^{-x^2}\,dx              →   ∫₀^∞e^(-x²) dx
x \in \mathbb{R} \setminus \mathbb{Q}   →   x ∈ ℝ ∖ ℚ
```

## How it works

Two rendering paths, picked to match what you're doing:

**Unicode preview (instant)** - a deliberately small parser validates the structure of a snippet
(braces, math delimiters, `\left`/`\right` pairs, environments) and maps known control sequences to
their Unicode equivalents.

**PNG (the real thing)** - hands the snippet to your LaTeX installation, then to `dvipng`. Only runs when
you actually invoke *Copy as PNG*. The image goes on the clipboard in two formats at once: PNG with
transparency for browsers, Slack, Word and Notion, and a white-flattened `CF_DIB` for older image editors.

## Requirements

- Windows 10 version 2004 (build 19041) or later, x64 or ARM64
- [Command Palette](https://learn.microsoft.com/windows/powertoys/command-palette/overview) (ships with PowerToys)
- **For *Copy as PNG* only:** a LaTeX installation providing `latex.exe` and `dvipng.exe` —
  [TeX Live](https://tug.org/texlive/) or [MiKTeX](https://miktex.org/). Both are looked up on `PATH`
  and in the usual install locations. Without one, the PNG row simply doesn't appear and everything
  else works as normal.

## Building

Requires the .NET 10 SDK and the Windows SDK build tools.

```powershell
dotnet build TexSnippets.sln -c Release -p:Platform=x64
```

To run it inside Command Palette, deploy the MSIX package (the *TexSnippets (Package)* profile in
Visual Studio, or `dotnet publish` with `Properties/PublishProfiles/win-x64.pubxml`), then restart
Command Palette (or use the `reload` command) so it picks up the new extension.

I recommend installing Visual Studio >2026 with ".NET desktop apps" and "WinUI packages" options
and using Build > Deploy TexSnippets option.

## Contributing

Issues and pull requests are welcome.

- **Found a bug?** Open an issue with the snippet that triggered it. The exact LaTeX source is
  usually enough to reproduce.
- **Adding symbols?** `TexSnippets/Latex/LatexSymbols.cs` holds the control-sequence -> Unicode tables.
  New entries are the easiest possible contribution and always welcome.
- **Changing the parser?** `TexSnippets/Latex/LatexRenderer.cs` is a single-pass recursive descent
  that stops at the first error. Please try keep it cheap, it runs on every keystroke.

Before opening a PR, please make sure `dotnet build TexSnippets.sln -c Release -p:Platform=x64`
succeeds with no new warnings. Match the surrounding style; the project builds with .NET analyzers on
`Recommended` and treats trim/AOT warnings as errors in release.

## License

MIT - see [LICENSE](LICENSE).

Scaffolded from the Microsoft Command Palette extension template, also MIT.

## AI usage

I user Claude Opus 5 for basic research, unicode symbols and code refactoring.