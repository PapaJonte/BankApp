using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;


public record SpinResult(IReadOnlyList<SlotSymbol> Symbols,decimal PayoutMultiplier);
