using System.Collections.ObjectModel;
using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// Holds different mappings for the symbols and their graphic
/// </summary>
/// <param name=""></param>
/// <returns></returns>
public class SymbolCatalog(IReadOnlyDictionary<int, SlotSymbol> symbols)
{
    public IReadOnlyDictionary<int, SlotSymbol> Symbols { get; private set; } = symbols;
    public static IReadOnlyDictionary<SlotSymbol, string>? SymbolGraphic { get; set; }

    /// <summary>
    /// Returns graphic for symbol passed in.
    /// </summary>
    /// <param name="">
    /// SlotSymbol symbol the enum representing the symbol we want graphic for.
    /// </param>
    /// <returns></returns>
    public static string GetSymbolGraphicNullCheck(SlotSymbol symbol)
    {
        if(SymbolGraphic == null)
        {
            throw new NullReferenceException("Symbol Graphics are null!");
        }
        
        if (string.IsNullOrEmpty(SymbolGraphic[symbol]))
        {
            throw new FormatException($"Graphic for symbol {symbol} is null!");
        }

        return SymbolGraphic[symbol];
    }

}