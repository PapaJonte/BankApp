using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.View;

/// <summary>
/// Maps logical symbols to their ASCII/Unicode glyph representation
/// for display on the reels.
/// </summary>
public class GlyphMap
{
    private readonly Dictionary<SlotSymbol, string> _glyphs = new();

    /// <summary>Registers a glyph for a symbol.</summary>
    public void Register(SlotSymbol symbol, string glyph) => _glyphs[symbol] = glyph;

    /// <summary>Resolves the glyph for a symbol, falling back to the symbol itself.</summary>
    public string Resolve(SlotSymbol symbol) => _glyphs.TryGetValue(symbol, out var g) ? g : symbol.ToString();
}