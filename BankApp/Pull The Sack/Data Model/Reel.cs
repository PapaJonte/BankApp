using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// A single spinning reel. Holds the target symbol, can be spun to recalculate new target symbol.
/// </summary>
public class Reel(SymbolCatalog catalog)
{
    private readonly SymbolCatalog _catalog = catalog;
    private readonly Random rnd = new();

    /// <summary>The symbol currently on the payline for this reel.</summary>
    public SlotSymbol CurrentSymbol { get; private set; } = catalog.Symbols[0];


    /// <summary>
    /// Roll a target symbol for this Reel
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void Spin()
    {
        int rolledSymbol = rnd.Next(0, _catalog.Symbols.Count);
        CurrentSymbol = _catalog.Symbols[rolledSymbol];
    }
}