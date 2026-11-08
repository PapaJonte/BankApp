namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>Configuration for the result / outcome screen.</summary>
public record ResultViewConfig : ViewConfig
{
    public double HoldSeconds { get; init; } = 3.0;

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