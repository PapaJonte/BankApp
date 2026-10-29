using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// Record of the value(multiplier) each symbol has. Used to calculate final payout.
/// </summary>
/// <param name=""></param>
/// <returns></returns>
public record struct PayoutEntry(SlotSymbol Symbol, decimal Multiplier);
