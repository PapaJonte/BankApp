namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>Configuration for the main gameplay layout (reels + balance).</summary>
public record GameViewConfig : ViewConfig
{
    /// <summary>Number of reels displayed on the payline.</summary>
    public int ReelCount { get; init; } = 3;

    /// <summary>Number of glyphs drawn per reel.</summary>
    public int ReelHeight { get; init; } = 1;
}