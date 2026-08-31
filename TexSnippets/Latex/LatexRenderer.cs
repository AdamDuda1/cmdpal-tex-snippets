// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Text;

namespace TexSnippets.Latex;

/// <summary>The outcome of "compiling" a snippet: a Unicode preview, or the first syntax error found.</summary>
/// <param name="Preview">Best-effort Unicode rendering. Empty when <paramref name="Error"/> is set.</param>
/// <param name="Error">Human-readable syntax error, or <see langword="null"/> when the snippet is well-formed.</param>
internal readonly record struct LatexResult(string Preview, string? Error)
{
    public bool IsError => Error is not null;
}

/// <summary>
/// A deliberately tiny TeX "compiler": it validates the structural syntax of a math snippet
/// (braces, math delimiters, <c>\left</c>/<c>\right</c>, environments) and renders it to Unicode.
/// </summary>
internal static class LatexRenderer
{
    /// <summary>Heuristic used by the fallback command to decide whether a query is worth claiming.</summary>
    // TODO here: fallback doesnt always work
    public static bool LooksLikeTex(string query) =>
        !string.IsNullOrWhiteSpace(query) && query.AsSpan().IndexOfAny("\\$^_") >= 0;

    public static LatexResult Render(string snippet)
    {
        var parser = new Parser(snippet);
        var preview = parser.Run();
        return parser.Error is null ? new LatexResult(preview, null) : new LatexResult(string.Empty, parser.Error);
    }

    private sealed class Parser(string text)
    {
        private readonly Stack<string> _environments = new();
        private int _index;
        private int _openDelimiters;
        private int _dollars;

        public string? Error { get; private set; }

        
        // just the errors i often make. add yours too if you want :))
        public string Run()
        {
            var body = ParseBody();

            if (Error is null && _index < text.Length)
            {
                Fail("unmatched '}'");
            }

            if (Error is null && _dollars % 2 != 0)
            {
                Fail("unmatched '$'");
            }

            if (Error is null && _openDelimiters > 0)
            {
                Fail("\\left without a matching \\right");
            }

            if (Error is null && _environments.Count > 0)
            {
                Fail($"missing \\end{{{_environments.Peek()}}}");
            }

            return body;
        }

        private string ParseBody()
        {
            var sb = new StringBuilder();

            while (Error is null && _index < text.Length)
            {
                var c = text[_index];
                if (c == '}')
                    break;

                switch (c)
                {
                    case '{':
                        _index++;
                        sb.Append(ParseBody());
                        Expect('}');
                        break;
                    case '$':
                        _index++;
                        _dollars++;
                        break;
                    case '~':
                        _index++;
                        sb.Append(' ');
                        break;
                    case '&':
                        _index++;
                        sb.Append('\t');
                        break;
                    case '^':
                        _index++;
                        sb.Append(ToScript(ReadArgument(), LatexSymbols.Superscripts, '^'));
                        break;
                    case '_':
                        _index++;
                        sb.Append(ToScript(ReadArgument(), LatexSymbols.Subscripts, '_'));
                        break;
                    case '\\':
                        sb.Append(ReadCommand());
                        break;
                    default:
                        _index++;
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        private string ReadArgument()
        {
            SkipSpaces();

            if (_index >= text.Length)
            {
                Fail("missing argument");
                return string.Empty;
            }

            if (text[_index] == '{')
            {
                _index++;
                var body = ParseBody();
                Expect('}');
                return body;
            }

            if (text[_index] == '\\')
                return ReadCommand().TrimEnd(' ');

            return text.Substring(_index++, 1);
        }

        //renders the control sequence starting at the current backslash
        private string ReadCommand()
        {
            _index++;

            if (_index >= text.Length)
            {
                Fail("trailing backslash");
                return string.Empty;
            }

            var c = text[_index];
            if (!char.IsAsciiLetter(c))
            {
                _index++;
                return c switch
                {
                    '\\' => "\n",
                    ',' or ':' or ';' or ' ' => " ",
                    '!' => string.Empty,
                    '|' => "‖",
                    _ => text.Substring(_index - 1, 1), // escaped literals: \{ \} \$ \% \& \_ \# ...
                };
            }

            var start = _index;
            while (_index < text.Length && char.IsAsciiLetter(text[_index]))
                _index++;

            var name = text[start.._index];

            switch (name)
            {
                case "frac" or "dfrac" or "tfrac":
                    var numerator = ReadArgument();
                    var denominator = ReadArgument();
                    return Error is null ? $"{Group(numerator)}/{Group(denominator)}" : string.Empty;

                case "sqrt":
                    return "√" + Group(ReadArgument());

                case "text" or "textrm" or "mathrm" or "mathbf" or "mathit" or "mathsf" or "operatorname":
                    return ReadArgument();

                case "mathbb":
                    return ToBlackboard(ReadArgument());

                case "left":
                    _openDelimiters++;
                    return ReadDelimiter();

                case "right":
                    if (_openDelimiters == 0)
                    {
                        Fail("\\right without a matching \\left");
                        return string.Empty;
                    }

                    _openDelimiters--;
                    return ReadDelimiter();

                case "begin":
                    _environments.Push(ReadArgument());
                    return "\n";

                case "end":
                    var closed = ReadArgument();
                    if (_environments.Count == 0 || _environments.Pop() != closed)
                    {
                        Fail($"\\end{{{closed}}} has no matching \\begin");
                    }

                    return "\n";

                default:
                    // tex swallows the space after a control word for some reason
                    var spaced = SkipSpaces() ? " " : string.Empty;
                    return (LatexSymbols.Commands.TryGetValue(name, out var symbol) ? symbol : "\\" + name) + spaced;
            }
        }

        private string ReadDelimiter()
        {
            SkipSpaces();

            if (_index >= text.Length)
            {
                Fail("missing delimiter after \\left or \\right");
                return string.Empty;
            }

            if (text[_index] == '\\')
                return ReadCommand().TrimEnd(' ');

            var delimiter = text.Substring(_index++, 1);
            return delimiter == "." ? string.Empty : delimiter;
        }

        private void Expect(char expected)
        {
            if (_index < text.Length && text[_index] == expected)
            {
                _index++;
                return;
            }

            Fail($"missing '{expected}'");
        }

        private bool SkipSpaces()
        {
            var start = _index;

            while (_index < text.Length && text[_index] == ' ')
                _index++;

            return _index > start;
        }

        private void Fail(string reason) => Error ??= $"Syntax error at position {_index + 1}: {reason}.";

        private static string Group(string rendered) =>
            rendered.Length <= 1 ? rendered : $"({rendered})";

        private static string ToScript(string rendered, Dictionary<char, char> map, char marker)
        {
            var sb = new StringBuilder(rendered.Length);

            foreach (var c in rendered)
            {
                if (!map.TryGetValue(c, out var scripted))
                    return marker + Group(rendered);

                sb.Append(scripted);
            }

            return sb.ToString();
        }

        private static string ToBlackboard(string rendered)
        {
            var sb = new StringBuilder(rendered.Length);

            foreach (var c in rendered)
                sb.Append(LatexSymbols.Blackboard.TryGetValue(c, out var doubleStruck) ? doubleStruck : c.ToString());

            return sb.ToString();
        }
    }
}
