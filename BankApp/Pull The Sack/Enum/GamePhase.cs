namespace BankApp.Pull_The_Sack.Enum;


/// <summary>
/// Different phases during slot game that is used for the state machine to route gameplay and views.
/// </summary>
public enum GamePhase
{
    Splash,
    Spinning,
    Resolving,
    Result,
    Exiting
}