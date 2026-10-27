namespace BankApp.Pull_The_Sack.View;

/// <summary>
/// Maps logical symbols to their ASCII/Unicode glyph representation
/// for display on the reels.
/// </summary>
public class GlyphMap
{
    private readonly Dictionary<string, string> _glyphs = new();

    /// <summary>Registers a glyph for a symbol.</summary>
    public void Register(string symbol, string glyph) => _glyphs[symbol] = glyph;

    /// <summary>Resolves the glyph for a symbol, falling back to the symbol itself.</summary>
    public string Resolve(string symbol) => _glyphs.TryGetValue(symbol, out var g) ? g : symbol;
}