// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System;
using System.Globalization;
using System.IO;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TexSnippets.Latex;

namespace TexSnippets.Settings;

internal sealed partial class SettingsManager : JsonSettingsManager
{
    private const string Namespace = "texsnippets";

    private readonly ToggleSetting _livePreview = new(
        Key(nameof(LivePreview)),
        "Live typeset preview",
        "Render the snippet with LaTeX as you type. Each pause in typing starts a compile.",
        false);
    
    private readonly ChoiceSetSetting _dpi = new(
        Key(nameof(Dpi)),
        "Image resolution",
        "Image resolution of the copied PNG - higher looks better scaled up, and takes longer to render.",
        [
            new ChoiceSetSetting.Choice("100 dpi - small text", "100"),
            new ChoiceSetSetting.Choice("300 dpi - screen", "300"),
            new ChoiceSetSetting.Choice("600 dpi - default", "600"),
            new ChoiceSetSetting.Choice("1200 dpi - print", "1200"),
        ]);

    private readonly ToggleSetting _transparent = new(
        Key(nameof(TransparentBackground)),
        "Transparent background",
        "Either way a white-flattened copy also goes on the clipboard for apps that cannot read transparency.",
        true);

    //^^^
    //Leave the background of the image transparent. Turn this off to get a solid white background instead. 
    
    private readonly TextSetting _ink = new(
        Key(nameof(Ink)),
        "Text color",
        "Text color of the typeset maths in hex (for example #1F3A93).",
        "#000000")
    {
        Placeholder = "#000000",
        Multiline = false,
    };

    private readonly TextSetting _preamble = new(
        Key(nameof(Preamble)),
        "Extra preamble",
        "" + @"Extra preamble - lines added before \begin{document}, such as \usepackage lines or \newcommand "
        + "macros.\namsmath, amssymb and amsfonts are always loaded.",
        string.Empty)
    {
        Multiline = true,
        Placeholder = @"\usepackage{physics}",
    };

    private readonly TextSetting _texDirectory = new(
        Key(nameof(TexDirectory)),
        "LaTeX bin folder",
        "LaTeX bin folder holding latex.exe and dvipng.exe.\nLeave empty to search PATH and the usual "
        + "TeX Live and MiKTeX locations, which is enough for most installations.",
        string.Empty)
    {
        Placeholder = @"C:\texlive\2025\bin\windows",
    };
    
    // private readonly Text _needsInstallationDisclaimer 
    
    public SettingsManager()
    {
        FilePath = SettingsPath();

        // ChoiceSetSetting has no default-value constructor, so pick one before the stored
        // settings are read over the top.
        _dpi.Value = "600";

        Settings.Add(_livePreview);
        Settings.Add(_transparent);
        Settings.Add(_ink);
        Settings.Add(_preamble);
        Settings.Add(_texDirectory);
        Settings.Add(_dpi);

        Settings.SettingsChanged += (_, _) =>
        {
            SaveSettings();
            Changed?.Invoke(this, EventArgs.Empty);
        };

        LoadSettings();
    }

    /// <summary>Raised after any setting is edited, once the new values have been persisted.</summary>
    public event EventHandler? Changed;

    public bool LivePreview => _livePreview.Value;

    public int Dpi => int.TryParse(_dpi.Value, CultureInfo.InvariantCulture, out var dpi) ? dpi : 600;

    public bool TransparentBackground => _transparent.Value;

    public string Ink => _ink.Value ?? string.Empty;

    public string Preamble => _preamble.Value ?? string.Empty;

    public string TexDirectory => _texDirectory.Value ?? string.Empty;

    /// <summary>The compiler options implied by the current settings, for the clipboard image.</summary>
    public TexOptions ForClipboard() => new(Dpi, TransparentBackground, TexColor.Parse(Ink), Preamble, TexDirectory);

    /// <summary>
    /// The compiler options for the details-pane preview: always transparent and always black, because
    /// the ink is recoloured afterwards to suit the light or dark theme, and lower resolution because
    /// this one runs while the user is still typing.
    /// </summary>
    public TexOptions ForPreview() => new(TexOptions.PreviewDpi, true, TexColor.Black, Preamble, TexDirectory);

    private static string Key(string name) => $"{Namespace}.{char.ToLowerInvariant(name[0])}{name[1..]}";

    private static string SettingsPath()
    {
        var directory = Utilities.BaseSettingsPath("TexSnippets");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "settings.json");
    }
}
