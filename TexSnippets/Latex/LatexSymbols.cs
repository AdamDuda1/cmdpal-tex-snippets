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
}
