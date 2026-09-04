// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using System.Globalization;

namespace TexSnippets.Latex;

/// <summary>An RGB colour, in the two forms this extension needs it: dvipng's and BGRA8's.</summary>
internal readonly record struct TexColor(byte R, byte G, byte B)
{
    public static TexColor Black => new(0, 0, 0);

    /// <summary>Relative luminance, 0 (black) to 1 (white), using the usual sRGB weights.</summary>
    public double Luminance => ((0.2126 * R) + (0.7152 * G) + (0.0722 * B)) / 255.0;

    /// <summary>
    /// The same colour as it should appear on a dark background: a dark ink has its lightness
    /// mirrored while its hue and saturation are kept, so black comes back as near-white and a navy
    /// formula as a light blue rather than as a washed-out grey. Colours that are already light
    /// enough to read on a dark background are left alone.
    /// </summary>
    public TexColor OnDarkBackground()
    {
        var (hue, saturation, lightness) = ToHsl();

        if (lightness >= 0.5)
            return this;

        // Stopping just short of pure white keeps the brightest case from glaring.
        return FromHsl(hue, saturation, Math.Min(1.0 - lightness, 0.87));
    }

    /// <summary>Parses <c>#rgb</c> or <c>#rrggbb</c>, falling back to black on anything else.</summary>
    public static TexColor Parse(string? text)
    {
        var hex = (text ?? string.Empty).Trim().TrimStart('#');

        if (hex.Length == 3 && TryNibbles(hex, out var shorthand))
        {
            return shorthand;
        }

        if (hex.Length == 6
            && byte.TryParse(hex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r)
            && byte.TryParse(hex[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g)
            && byte.TryParse(hex[4..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
        {
            return new TexColor(r, g, b);
        }

        return Black;
    }

    /// <summary>A dvipng colour specification - the "rgb r g b" form, with components in 0..1.</summary>
    public string ToDvipng() => string.Create(
        CultureInfo.InvariantCulture,
        $"rgb {R / 255.0:0.###} {G / 255.0:0.###} {B / 255.0:0.###}");

    private (double Hue, double Saturation, double Lightness) ToHsl()
    {
        double r = R / 255.0, g = G / 255.0, b = B / 255.0;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var lightness = (max + min) / 2;

        if (max == min)
        {
            return (0, 0, lightness);
        }

        var span = max - min;
        var saturation = lightness > 0.5 ? span / (2 - max - min) : span / (max + min);

        var hue = max == r ? ((g - b) / span) + (g < b ? 6 : 0)
            : max == g ? ((b - r) / span) + 2
            : ((r - g) / span) + 4;

        return (hue / 6, saturation, lightness);
    }

    private static TexColor FromHsl(double hue, double saturation, double lightness)
    {
        if (saturation == 0)
        {
            var grey = Channel(lightness);
            return new TexColor(grey, grey, grey);
        }

        var q = lightness < 0.5 ? lightness * (1 + saturation) : lightness + saturation - (lightness * saturation);
        var p = (2 * lightness) - q;

        return new TexColor(
            Channel(Component(p, q, hue + (1.0 / 3))),
            Channel(Component(p, q, hue)),
            Channel(Component(p, q, hue - (1.0 / 3))));

        static double Component(double p, double q, double t)
        {
            t = t < 0 ? t + 1 : t > 1 ? t - 1 : t;

            return t < 1.0 / 6 ? p + ((q - p) * 6 * t)
                : t < 1.0 / 2 ? q
                : t < 2.0 / 3 ? p + ((q - p) * ((2.0 / 3) - t) * 6)
                : p;
        }

        static byte Channel(double value) => (byte)Math.Clamp(Math.Round(value * 255), 0, 255);
    }

    private static bool TryNibbles(string hex, out TexColor color)
    {
        color = Black;

        for (var i = 0; i < 3; i++)
        {
            if (!byte.TryParse(hex.AsSpan(i, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
            {
                return false;
            }
        }

        color = new TexColor(Doubled(hex[0]), Doubled(hex[1]), Doubled(hex[2]));
        return true;

        static byte Doubled(char nibble)
        {
            var value = byte.Parse(stackalloc char[] { nibble }, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return (byte)((value * 16) + value);
        }
    }
}

/// <summary>
/// What a single LaTeX run should produce. Assembled from the user's settings, and passed down
/// explicitly so that nothing in <see cref="TexPngCompiler"/> has to reach for global state.
/// </summary>
/// <param name="Dpi">Resolution handed to <c>dvipng</c>.</param>
/// <param name="Transparent">Whether the background stays transparent rather than white.</param>
/// <param name="Ink">Colour of the typeset maths.</param>
/// <param name="Preamble">Extra preamble lines, or empty.</param>
/// <param name="ToolDirectory">Folder to look in for the TeX binaries first, or empty to auto-detect.</param>
internal readonly record struct TexOptions(
    int Dpi,
    bool Transparent,
    TexColor Ink,
    string Preamble,
    string ToolDirectory)
{
    /// <summary>
    /// Resolution of the details-pane preview. Lower than the clipboard default on purpose: this one
    /// runs while the user is typing, and the pane never shows it larger than a few hundred pixels.
    /// </summary>
    public const int PreviewDpi = 300;
}
