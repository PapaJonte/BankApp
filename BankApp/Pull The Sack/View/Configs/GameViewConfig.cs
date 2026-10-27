namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>Configuration for the main gameplay layout (reels + balance).</summary>
public record GameViewConfig : ViewConfig
{
    /// <summary>Number of reels displayed on the payline.</summary>
    public int ReelCount { get; init; } = 3;

    public int ReelRows { get; init; } = 3;

    /// <summary>Number of glyphs drawn per reel.</summary>
    public int ReelHeight { get; init; } = 3;
    public int ReelWidth { get; init; } = 5;

    public int ReelGap { get; init; } = 2;

    public int ReelTop { get; init; } = 6;

    public int MinReelTargetSpinCount { get; init; } = 5;

    
}