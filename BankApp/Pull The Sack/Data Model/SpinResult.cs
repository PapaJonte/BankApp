using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// record for keeping the result from spinning the slot machine. The rolled symbols per reel as well as payout multiplier
/// </summary>
/// <param name="">
/// IReadOnlyList<SlotSymbol> Symbols the rolled symbols per reel, one for each reel
/// decimal PayoutMultiplier the payout multiplier based on the reel rolls.
/// </param>
/// <returns></returns>
public record SpinResult(IReadOnlyList<SlotSymbol> Symbols,decimal PayoutMultiplier);
