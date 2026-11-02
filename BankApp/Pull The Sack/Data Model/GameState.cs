using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// snapshot of current game runstate.
/// </summary>
public record struct GameState(GamePhase GamePhase, decimal Balance, int SpinCount, SpinResult? SpinResult);