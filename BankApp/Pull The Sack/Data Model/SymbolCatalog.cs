using System.Collections.ObjectModel;
using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// Injected configuration holding the set of symbols the reels can land on
/// and any weighted-distribution information.
/// </summary>
public class SymbolCatalog
{
    /// <summary>The ordered set of symbols available to the reels.</summary>
    public IReadOnlyDictionary<int, SlotSymbol> Symbols { get; private set; }
    public static IReadOnlyDictionary<SlotSymbol, string>? SymbolGraphic { get; set; }

    public SymbolCatalog(IReadOnlyDictionary<int, SlotSymbol> symbols)
    {
        Symbols = symbols;
    }

    public static string GetSymbolGraphicNullCheck(SlotSymbol symbol)
    {
        if (string.IsNullOrEmpty(SymbolGraphic[symbol]))
        {
            throw new FormatException($"Graphic for symbol {symbol} is null!");
        }

        return SymbolGraphic[symbol];
    }

}