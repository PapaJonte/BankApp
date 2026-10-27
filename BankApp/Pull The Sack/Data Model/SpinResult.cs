using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;


public record SpinResult(IReadOnlyList<string> Symbols,decimal PayoutMultiplier);
