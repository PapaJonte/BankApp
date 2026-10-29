namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>Configuration for the result / outcome screen.</summary>
public record ResultViewConfig : ViewConfig
{
    /// <summary>How long the result screen lingers before auto-advancing (seconds).</summary>
    public double HoldSeconds { get; init; } = 3.0;

    /// <summary>Sponsor / brand logo rendered at the top of the result screen (narrow ASCII, ~63 cols).</summary>
    public string LogoArt { get; init; } =
        "  ##      ####  ##   ##  ######     ######   ####    #####\n" +
        "  ##       ##   ##   ##  ##         ##      ##  ##   ##  ##\n" +
        "  ##       ##   ##   ##  #####      #####   ##  ##   #####\n" +
        "  ##       ##   ##   ##  ##         ##      ##  ##   ## ##\n" +
        "  ######  ####   #####   ######     ##       ####    ##  ##\n" +
        "  ---------------------------------------------------------\n" +
        "           L i v e F o r G a m b l i n g . c o m\n" +
        "  ---------------------------------------------------------";
}