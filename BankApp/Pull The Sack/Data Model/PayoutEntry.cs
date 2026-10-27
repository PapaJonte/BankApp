using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

public record struct PayoutEntry(SlotSymbol Symbol, decimal Multiplier);
