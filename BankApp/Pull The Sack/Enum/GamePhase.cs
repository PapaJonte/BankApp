namespace BankApp.Pull_The_Sack.Enum;

/// <summary>
/// Represents the high-level phase the slot machine game is currently in.
/// Drives which view is active and what input is accepted.
/// </summary>
public enum GamePhase
{
    /// <summary>Intro / branding screen shown before play begins.</summary>
    Splash,

    /// <summary>A spin is in progress (reels animating).</summary>
    Spinning,

    /// <summary>Reels have settled; result is being revealed.</summary>
    Resolving,

    /// <summary>Final outcome screen (win / lose / neutral).</summary>
    Result,

    /// <summary>Game has ended and control returns to the caller.</summary>
    Exiting
}