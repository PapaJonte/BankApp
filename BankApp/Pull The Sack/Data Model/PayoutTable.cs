using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

public class PayoutTable
{
    public IReadOnlyList<PayoutEntry>? Entries { get; set; }
    public PayoutTable() => GenerateTable();

    public decimal GetPayoutValue(SlotSymbol symbol) => Entries!.FirstOrDefault(s=>s.Symbol == symbol).Multiplier;

    private void GenerateTable()
    {
        Entries = [
            new PayoutEntry(SlotSymbol.Sackpot, 5),
            new PayoutEntry(SlotSymbol.Awesome,4),
            new PayoutEntry(SlotSymbol.Coin,3.5m),
            new PayoutEntry(SlotSymbol.Star,3m),
            new PayoutEntry(SlotSymbol.Bar,2.5m),
            new PayoutEntry(SlotSymbol.Stash,2.3m),
            new PayoutEntry(SlotSymbol.Ampersand,2m),    
        ];
    }
}
