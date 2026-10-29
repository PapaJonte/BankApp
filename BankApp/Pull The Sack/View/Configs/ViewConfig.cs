namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>
/// Base configuration shared by all views (dimensions / title / colors).
/// </summary>
public abstract record ViewConfig
{
    /// <summary>Width of the view in console columns.</summary>
    public int Width { get; init; } = 60;

    /// <summary>Height of the view in console rows.</summary>
    public int Height { get; init; } = 20;

    /// <summary>Optional title rendered at the top of the view.</summary>
    public string Title { get; init; } = string.Empty;
}