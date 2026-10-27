using BankApp.Pull_The_Sack.Enum;

namespace BankApp.Pull_The_Sack.Data_Model;

/// <summary>
/// Immutable snapshot of the game's runtime state at a point in time.
/// </summary>
public record struct GameState(GamePhase GamePhase, decimal Balance, int SpinCount, SpinResult? SpinResult);