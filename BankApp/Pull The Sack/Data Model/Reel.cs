using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// A single spinning reel. Holds the current symbol and can be spun
/// to select a new one via the injected RNG.
/// </summary>
public class Reel
{
    private readonly SymbolCatalog _catalog;
    private readonly IRng _rng;

    /// <summary>The symbol currently on the payline for this reel.</summary>
    public SlotSymbol CurrentSymbol { get; private set; }

    public Reel(SymbolCatalog catalog, IRng rng)
    {
        _catalog = catalog;
        _rng = rng;
        CurrentSymbol = catalog.Symbols[0];
    }

    /// <summary>Spins this reel and updates <see cref="CurrentSymbol"/>.</summary>
    public void Spin()
    {
        int rolledSymbol = _rng.Next(0, _catalog.Symbols.Count);
        CurrentSymbol = _catalog.Symbols[rolledSymbol];
    }
}