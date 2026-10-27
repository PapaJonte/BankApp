namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>Configuration for the result / outcome screen.</summary>
public record ResultViewConfig : ViewConfig
{
    /// <summary>How long the result screen lingers before auto-advancing (seconds).</summary>
    public double HoldSeconds { get; init; } = 3.0;
}