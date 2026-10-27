namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>Configuration for the main gameplay layout (reels + balance).</summary>
public record GameViewConfig : ViewConfig
{

    public string Banner =
       @"   ____        _ _     _____ _            ____             _    " + "\n" +
       @"  |  _ \ _   _| | |   |_   _| |__   ___  / ___|  __ _  ___| | __" + "\n" +
       @"  | |_) | | | | | |     | | | '_ \ / _ \ \___ \ / _` |/ __| |/ /" + "\n" +
       @"  |  __/| |_| | | |     | | | | | |  __/  ___) | (_| | (__|   < " + "\n" +
       @"  |_|    \__,_|_|_|     |_| |_| |_|\___| |____/ \__,_|\___|_|\_\";

    public string SponsorText { get; init; } = "Sponsored by LiveForGambling.com";

    /// <summary>Fancy border drawn above the slot machine.</summary>
    public string TopArt { get; init; } =
        @"╔═══════════════════════════════════════════════════╗" + "\n" +
        @"║         ~ T H E  G O L D E N  R E E L S ~         ║" + "\n" +
        @"╠═══════════════════════════════════════════════════╣" + "\n" +
        @"║    ╔═╗╔═╗╔═╗  ╠═══ P A Y L I N E ═══╣  ╔═╗╔═╗╔═╗  ║" + "\n" +
        @"║    ╚═╝╚═╝╚═╝  ╠═════════════════════╣  ╚═╝╚═╝╚═╝  ║" + "\n" +
        @"╚═══════════════════════════════════════════════════╝";

    /// <summary>Fancy border drawn below the slot machine.</summary>
    public string BottomArt { get; init; } =
        @"╔═════════════════════════════════════════════════╗" + "\n" +
        @"║  ╔═════╗       S P I N  T O  W I N     ╔═════╗  ║" + "\n" +
        @"║  ╚═════╝                               ╚═════╝  ║" + "\n" +
        @"╠═════════════════════════════════════════════════╣" + "\n" +
        @"║    ~ G O O D  L U C K ,  H I G H  R O L L E R ~ ║" + "\n" +
        @"╚═════════════════════════════════════════════════╝";

    /// <summary>Number of reels displayed on the payline.</summary>
    public int ReelCount { get; init; } = 3;
    public int ReelRows { get; init; } = 3;

    /// <summary>Number of glyphs drawn per reel.</summary>
    public int ReelHeight { get; init; } = 3;
    public int ReelWidth { get; init; } = 5;

    public int ReelGap { get; init; } = 2;

    public int ReelTop { get; init; } = 10;

    public int MinReelTargetSpinCount { get; init; } = 5;


}