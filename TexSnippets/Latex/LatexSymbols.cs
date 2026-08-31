// Copyright (c) Adam Duda
// Licensed under the MIT license. See the LICENSE file in the project root for details.

using System.Collections.Generic;

namespace TexSnippets.Latex;

/// <summary>
/// Lookup tables used by <see cref="LatexRenderer"/> to turn TeX control sequences
/// into their Unicode equivalents. Anything missing here is simply left as-is.
/// </summary>
internal static class LatexSymbols
{
    /// <summary>Maps a control sequence name (without the leading backslash) to its Unicode form.</summary>
    public static readonly Dictionary<string, string> Commands = new(System.StringComparer.Ordinal)
    {
        // Greek, lowercase
        ["alpha"] = "α", ["beta"] = "β", ["gamma"] = "γ", ["delta"] = "δ",
        ["epsilon"] = "ϵ", ["varepsilon"] = "ε", ["zeta"] = "ζ", ["eta"] = "η",
        ["theta"] = "θ", ["vartheta"] = "ϑ", ["iota"] = "ι", ["kappa"] = "κ",
        ["lambda"] = "λ", ["mu"] = "μ", ["nu"] = "ν", ["xi"] = "ξ",
        ["pi"] = "π", ["varpi"] = "ϖ", ["rho"] = "ρ", ["varrho"] = "ϱ",
        ["sigma"] = "σ", ["varsigma"] = "ς", ["tau"] = "τ", ["upsilon"] = "υ",
        ["phi"] = "ϕ", ["varphi"] = "φ", ["chi"] = "χ", ["psi"] = "ψ", ["omega"] = "ω",

        // Greek, uppercase
        ["Gamma"] = "Γ", ["Delta"] = "Δ", ["Theta"] = "Θ", ["Lambda"] = "Λ",
        ["Xi"] = "Ξ", ["Pi"] = "Π", ["Sigma"] = "Σ", ["Upsilon"] = "Υ",
        ["Phi"] = "Φ", ["Psi"] = "Ψ", ["Omega"] = "Ω",

        // Binary operators
        ["times"] = "×", ["div"] = "÷", ["cdot"] = "⋅", ["pm"] = "±", ["mp"] = "∓",
        ["ast"] = "∗", ["star"] = "⋆", ["circ"] = "∘", ["bullet"] = "∙",
        ["oplus"] = "⊕", ["ominus"] = "⊖", ["otimes"] = "⊗", ["odot"] = "⊙",
        ["setminus"] = "∖", ["wedge"] = "∧", ["vee"] = "∨", ["land"] = "∧", ["lor"] = "∨",

        // Relations
        ["leq"] = "≤", ["le"] = "≤", ["geq"] = "≥", ["ge"] = "≥", ["neq"] = "≠", ["ne"] = "≠",
        ["approx"] = "≈", ["equiv"] = "≡", ["sim"] = "∼", ["simeq"] = "≃", ["cong"] = "≅",
        ["propto"] = "∝", ["ll"] = "≪", ["gg"] = "≫", ["perp"] = "⊥", ["parallel"] = "∥",

        // Sets and logic
        ["in"] = "∈", ["notin"] = "∉", ["ni"] = "∋", ["subset"] = "⊂", ["subseteq"] = "⊆",
        ["supset"] = "⊃", ["supseteq"] = "⊇", ["cup"] = "∪", ["cap"] = "∩",
        ["emptyset"] = "∅", ["varnothing"] = "∅", ["forall"] = "∀", ["exists"] = "∃",
        ["nexists"] = "∄", ["neg"] = "¬", ["lnot"] = "¬", ["therefore"] = "∴", ["because"] = "∵",

        // Arrows
        ["to"] = "→", ["rightarrow"] = "→", ["leftarrow"] = "←", ["leftrightarrow"] = "↔",
        ["Rightarrow"] = "⇒", ["Leftarrow"] = "⇐", ["Leftrightarrow"] = "⇔",
        ["mapsto"] = "↦", ["uparrow"] = "↑", ["downarrow"] = "↓", ["implies"] = "⟹", ["iff"] = "⟺",

        // Big operators and calculus
        ["sum"] = "∑", ["prod"] = "∏", ["coprod"] = "∐", ["int"] = "∫", ["iint"] = "∬",
        ["iiint"] = "∭", ["oint"] = "∮", ["partial"] = "∂", ["nabla"] = "∇",
        ["infty"] = "∞", ["lim"] = "𝑙𝑖𝑚", ["sup"] = "𝑠𝑢𝑝", ["inf"] = "𝑖𝑛𝑓",

        // Named functions
        ["sin"] = "𝑠𝑖𝑛", ["cos"] = "𝑐𝑜𝑠", ["tan"] = "𝑡𝑎𝑛", ["cot"] = "𝑐𝑜𝑡",
        ["sec"] = "𝑠𝑒𝑐", ["csc"] = "𝑐𝑠𝑐", ["arcsin"] = "𝑎𝑟𝑐𝑠𝑖𝑛", ["arccos"] = "𝑎𝑟𝑐𝑐𝑜𝑠",
        ["arctan"] = "𝑎𝑟𝑐𝑡𝑎𝑛", ["sinh"] = "𝑠𝑖𝑛ℎ", ["cosh"] = "𝑐𝑜𝑠ℎ", ["tanh"] = "𝑡𝑎𝑛ℎ",
        ["log"] = "𝑙𝑜𝑔", ["ln"] = "𝑙𝑛", ["exp"] = "𝑒𝑥𝑝", ["min"] = "𝑚𝑖𝑛", ["max"] = "𝑚𝑎𝑥",
        ["det"] = "𝑑𝑒𝑡", ["dim"] = "𝑑𝑖𝑚", ["ker"] = "𝑘𝑒𝑟", ["deg"] = "𝑑𝑒𝑔", ["gcd"] = "𝑔𝑐𝑑",

        // Miscellaneous
        ["ldots"] = "…", ["dots"] = "…", ["cdots"] = "⋯", ["vdots"] = "⋮", ["ddots"] = "⋱",
        ["angle"] = "∠", ["degree"] = "°", ["prime"] = "′", ["hbar"] = "ℏ", ["ell"] = "ℓ",
        ["Re"] = "ℜ", ["Im"] = "ℑ", ["aleph"] = "ℵ", ["surd"] = "√",
        ["langle"] = "⟨", ["rangle"] = "⟩", ["lfloor"] = "⌊", ["rfloor"] = "⌋",
        ["lceil"] = "⌈", ["rceil"] = "⌉", ["vert"] = "|", ["Vert"] = "‖",
    };
    
    /// <summary>Characters that have a dedicated Unicode superscript form.</summary>
    public static readonly Dictionary<char, char> Superscripts = new()
    {
        ['0'] = '⁰', ['1'] = '¹', ['2'] = '²', ['3'] = '³', ['4'] = '⁴',
        ['5'] = '⁵', ['6'] = '⁶', ['7'] = '⁷', ['8'] = '⁸', ['9'] = '⁹',
        ['+'] = '⁺', ['-'] = '⁻', ['='] = '⁼', ['('] = '⁽', [')'] = '⁾',
        ['n'] = 'ⁿ', ['i'] = 'ⁱ', ['a'] = 'ᵃ', ['b'] = 'ᵇ', ['c'] = 'ᶜ',
        ['d'] = 'ᵈ', ['e'] = 'ᵉ', ['k'] = 'ᵏ', ['m'] = 'ᵐ', ['p'] = 'ᵖ',
        ['t'] = 'ᵗ', ['x'] = 'ˣ', ['y'] = 'ʸ',
    };

    /// <summary>Characters that have a dedicated Unicode subscript form.</summary>
    public static readonly Dictionary<char, char> Subscripts = new()
    {
        ['0'] = '₀', ['1'] = '₁', ['2'] = '₂', ['3'] = '₃', ['4'] = '₄',
        ['5'] = '₅', ['6'] = '₆', ['7'] = '₇', ['8'] = '₈', ['9'] = '₉',
        ['+'] = '₊', ['-'] = '₋', ['='] = '₌', ['('] = '₍', [')'] = '₎',
        ['a'] = 'ₐ', ['e'] = 'ₑ', ['h'] = 'ₕ', ['i'] = 'ᵢ', ['j'] = 'ⱼ',
        ['k'] = 'ₖ', ['l'] = 'ₗ', ['m'] = 'ₘ', ['n'] = 'ₙ', ['o'] = 'ₒ',
        ['p'] = 'ₚ', ['r'] = 'ᵣ', ['s'] = 'ₛ', ['t'] = 'ₜ', ['u'] = 'ᵤ',
        ['v'] = 'ᵥ', ['x'] = 'ₓ',
    };

    /// <summary>Letters with a Unicode double-struck ("blackboard bold") form, for <c>\mathbb</c>.</summary>
    public static readonly Dictionary<char, string> Blackboard = new()
    {
        ['A'] = "𝔸", ['B'] = "𝔹", ['C'] = "ℂ", ['D'] = "𝔻", ['E'] = "𝔼", ['F'] = "𝔽",
        ['G'] = "𝔾", ['H'] = "ℍ", ['I'] = "𝕀", ['J'] = "𝕁", ['K'] = "𝕂", ['L'] = "𝕃",
        ['M'] = "𝕄", ['N'] = "ℕ", ['O'] = "𝕆", ['P'] = "ℙ", ['Q'] = "ℚ", ['R'] = "ℝ",
        ['S'] = "𝕊", ['T'] = "𝕋", ['U'] = "𝕌", ['V'] = "𝕍", ['W'] = "𝕎", ['X'] = "𝕏",
        ['Y'] = "𝕐", ['Z'] = "ℤ",
    }; // dont look good do they

    /// <summary>Letters with a Unicode bold serif form, for <c>\mathbf</c>.</summary>
    public static readonly Dictionary<char, string> Bold = new()
    {
        ['A'] = "𝐀", ['B'] = "𝐁", ['C'] = "𝐂", ['D'] = "𝐃", ['E'] = "𝐄", ['F'] = "𝐅",
        ['G'] = "𝐆", ['H'] = "𝐇", ['I'] = "𝐈", ['J'] = "𝐉", ['K'] = "𝐊", ['L'] = "𝐋",
        ['M'] = "𝐌", ['N'] = "𝐍", ['O'] = "𝐎", ['P'] = "𝐏", ['Q'] = "𝐐", ['R'] = "𝐑",
        ['S'] = "𝐒", ['T'] = "𝐓", ['U'] = "𝐔", ['V'] = "𝐕", ['W'] = "𝐖", ['X'] = "𝐗",
        ['Y'] = "𝐘", ['Z'] = "𝐙", ['a'] = "𝐚", ['b'] = "𝐛", ['c'] = "𝐜", ['d'] = "𝐝",
        ['e'] = "𝐞", ['f'] = "𝐟", ['g'] = "𝐠", ['h'] = "𝐡", ['i'] = "𝐢", ['j'] = "𝐣",
        ['k'] = "𝐤", ['l'] = "𝐥", ['m'] = "𝐦", ['n'] = "𝐧", ['o'] = "𝐨", ['p'] = "𝐩",
        ['q'] = "𝐪", ['r'] = "𝐫", ['s'] = "𝐬", ['t'] = "𝐭", ['u'] = "𝐮", ['v'] = "𝐯",
        ['w'] = "𝐰", ['x'] = "𝐱", ['y'] = "𝐲", ['z'] = "𝐳",
    };

    /// <summary>Letters with a Unicode italic serif form, for <c>\mathit</c>.</summary>
    public static readonly Dictionary<char, string> Italic = new()
    {
        ['A'] = "𝐴", ['B'] = "𝐵", ['C'] = "𝐶", ['D'] = "𝐷", ['E'] = "𝐸", ['F'] = "𝐹",
        ['G'] = "𝐺", ['H'] = "𝐻", ['I'] = "𝐼", ['J'] = "𝐽", ['K'] = "𝐾", ['L'] = "𝐿",
        ['M'] = "𝑀", ['N'] = "𝑁", ['O'] = "𝑂", ['P'] = "𝑃", ['Q'] = "𝑄", ['R'] = "𝑅",
        ['S'] = "𝑆", ['T'] = "𝑇", ['U'] = "𝑈", ['V'] = "𝑉", ['W'] = "𝑊", ['X'] = "𝑋",
        ['Y'] = "𝑌", ['Z'] = "𝑍", ['a'] = "𝑎", ['b'] = "𝑏", ['c'] = "𝑐", ['d'] = "𝑑",
        ['e'] = "𝑒", ['f'] = "𝑓", ['g'] = "𝑔", ['h'] = "ℎ", ['i'] = "𝑖", ['j'] = "𝑗",
        ['k'] = "𝑘", ['l'] = "𝑙", ['m'] = "𝑚", ['n'] = "𝑛", ['o'] = "𝑜", ['p'] = "𝑝",
        ['q'] = "𝑞", ['r'] = "𝑟", ['s'] = "𝑠", ['t'] = "𝑡", ['u'] = "𝑢", ['v'] = "𝑣",
        ['w'] = "𝑤", ['x'] = "𝑥", ['y'] = "𝑦", ['z'] = "𝑧",
    }; // todo normal letters as italics??

    /// <summary>Letters with a Unicode script form, for <c>\mathcal</c> and <c>\mathscr</c>.</summary>
    public static readonly Dictionary<char, string> Script = new()
    {
        ['A'] = "𝒜", ['B'] = "ℬ", ['C'] = "𝒞", ['D'] = "𝒟", ['E'] = "ℰ", ['F'] = "ℱ",
        ['G'] = "𝒢", ['H'] = "ℋ", ['I'] = "ℐ", ['J'] = "𝒥", ['K'] = "𝒦", ['L'] = "ℒ",
        ['M'] = "ℳ", ['N'] = "𝒩", ['O'] = "𝒪", ['P'] = "𝒫", ['Q'] = "𝒬", ['R'] = "ℛ",
        ['S'] = "𝒮", ['T'] = "𝒯", ['U'] = "𝒰", ['V'] = "𝒱", ['W'] = "𝒲", ['X'] = "𝒳",
        ['Y'] = "𝒴", ['Z'] = "𝒵", ['a'] = "𝒶", ['b'] = "𝒷", ['c'] = "𝒸", ['d'] = "𝒹",
        ['e'] = "ℯ", ['f'] = "𝒻", ['g'] = "ℊ", ['h'] = "𝒽", ['i'] = "𝒾", ['j'] = "𝒿",
        ['k'] = "𝓀", ['l'] = "𝓁", ['m'] = "𝓂", ['n'] = "𝓃", ['o'] = "ℴ", ['p'] = "𝓅",
        ['q'] = "𝓆", ['r'] = "𝓇", ['s'] = "𝓈", ['t'] = "𝓉", ['u'] = "𝓊", ['v'] = "𝓋",
        ['w'] = "𝓌", ['x'] = "𝓍", ['y'] = "𝓎", ['z'] = "𝓏",
    };

    /// <summary>Letters with a Unicode Fraktur form, for <c>\mathfrak</c>.</summary>
    public static readonly Dictionary<char, string> Fraktur = new()
    {
        ['A'] = "𝔄", ['B'] = "𝔅", ['C'] = "ℭ", ['D'] = "𝔇", ['E'] = "𝔈", ['F'] = "𝔉",
        ['G'] = "𝔊", ['H'] = "ℌ", ['I'] = "ℑ", ['J'] = "𝔍", ['K'] = "𝔎", ['L'] = "𝔏",
        ['M'] = "𝔐", ['N'] = "𝔑", ['O'] = "𝔒", ['P'] = "𝔓", ['Q'] = "𝔔", ['R'] = "ℜ",
        ['S'] = "𝔖", ['T'] = "𝔗", ['U'] = "𝔘", ['V'] = "𝔙", ['W'] = "𝔚", ['X'] = "𝔛",
        ['Y'] = "𝔜", ['Z'] = "ℨ", ['a'] = "𝔞", ['b'] = "𝔟", ['c'] = "𝔠", ['d'] = "𝔡",
        ['e'] = "𝔢", ['f'] = "𝔣", ['g'] = "𝔤", ['h'] = "𝔥", ['i'] = "𝔦", ['j'] = "𝔧",
        ['k'] = "𝔨", ['l'] = "𝔩", ['m'] = "𝔪", ['n'] = "𝔫", ['o'] = "𝔬", ['p'] = "𝔭",
        ['q'] = "𝔮", ['r'] = "𝔯", ['s'] = "𝔰", ['t'] = "𝔱", ['u'] = "𝔲", ['v'] = "𝔳",
        ['w'] = "𝔴", ['x'] = "𝔵", ['y'] = "𝔶", ['z'] = "𝔷",
    };

    /// <summary>Letters with a Unicode sans-serif form, for <c>\mathsf</c>.</summary>
    public static readonly Dictionary<char, string> SansSerif = new()
    {
        ['A'] = "𝖠", ['B'] = "𝖡", ['C'] = "𝖢", ['D'] = "𝖣", ['E'] = "𝖤", ['F'] = "𝖥",
        ['G'] = "𝖦", ['H'] = "𝖧", ['I'] = "𝖨", ['J'] = "𝖩", ['K'] = "𝖪", ['L'] = "𝖫",
        ['M'] = "𝖬", ['N'] = "𝖭", ['O'] = "𝖮", ['P'] = "𝖯", ['Q'] = "𝖰", ['R'] = "𝖱",
        ['S'] = "𝖲", ['T'] = "𝖳", ['U'] = "𝖴", ['V'] = "𝖵", ['W'] = "𝖶", ['X'] = "𝖷",
        ['Y'] = "𝖸", ['Z'] = "𝖹", ['a'] = "𝖺", ['b'] = "𝖻", ['c'] = "𝖼", ['d'] = "𝖽",
        ['e'] = "𝖾", ['f'] = "𝖿", ['g'] = "𝗀", ['h'] = "𝗁", ['i'] = "𝗂", ['j'] = "𝗃",
        ['k'] = "𝗄", ['l'] = "𝗅", ['m'] = "𝗆", ['n'] = "𝗇", ['o'] = "𝗈", ['p'] = "𝗉",
        ['q'] = "𝗊", ['r'] = "𝗋", ['s'] = "𝗌", ['t'] = "𝗍", ['u'] = "𝗎", ['v'] = "𝗏",
        ['w'] = "𝗐", ['x'] = "𝗑", ['y'] = "𝗒", ['z'] = "𝗓",
    };

    /// <summary>Letters with a Unicode monospace form, for <c>\mathtt</c>.</summary>
    public static readonly Dictionary<char, string> Monospace = new()
    {
        ['A'] = "𝙰", ['B'] = "𝙱", ['C'] = "𝙲", ['D'] = "𝙳", ['E'] = "𝙴", ['F'] = "𝙵",
        ['G'] = "𝙶", ['H'] = "𝙷", ['I'] = "𝙸", ['J'] = "𝙹", ['K'] = "𝙺", ['L'] = "𝙻",
        ['M'] = "𝙼", ['N'] = "𝙽", ['O'] = "𝙾", ['P'] = "𝙿", ['Q'] = "𝚀", ['R'] = "𝚁",
        ['S'] = "𝚂", ['T'] = "𝚃", ['U'] = "𝚄", ['V'] = "𝚅", ['W'] = "𝚆", ['X'] = "𝚇",
        ['Y'] = "𝚈", ['Z'] = "𝚉", ['a'] = "𝚊", ['b'] = "𝚋", ['c'] = "𝚌", ['d'] = "𝚍",
        ['e'] = "𝚎", ['f'] = "𝚏", ['g'] = "𝚐", ['h'] = "𝚑", ['i'] = "𝚒", ['j'] = "𝚓",
        ['k'] = "𝚔", ['l'] = "𝚕", ['m'] = "𝚖", ['n'] = "𝚗", ['o'] = "𝚘", ['p'] = "𝚙",
        ['q'] = "𝚚", ['r'] = "𝚛", ['s'] = "𝚜", ['t'] = "𝚝", ['u'] = "𝚞", ['v'] = "𝚟",
        ['w'] = "𝚠", ['x'] = "𝚡", ['y'] = "𝚢", ['z'] = "𝚣",
    };
}
