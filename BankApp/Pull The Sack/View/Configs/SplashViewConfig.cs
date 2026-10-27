using System.Collections.ObjectModel;

namespace BankApp.Pull_The_Sack.View.Configs;

/// <summary>Configuration for the splash / branding screen.</summary>
public record SplashViewConfig : ViewConfig
{
    /// <summary>Sponsor line shown on the splash screen.</summary>
    public string SponsorText { get; init; } = "Sponsored by LiveForGambling.com";

    /// <summary>
    /// Pre-computed splash animation frames. Each frame is the full ASCII art for the
    /// screen (banner + hanging sack + a spinning coin). The coin is the only part that
    /// changes between frames, so looping the list produces a continuous spin.
    /// </summary>
    public ReadOnlyCollection<string> Frames { get; } = new(
    [
        Frame1,
        Frame2,
        Frame3,
        Frame4,
    ]);

    public string SplashGraphics => Head;

    private const string Banner =
        @"   ____        _ _     _____ _            ____             _    " + "\n" +
        @"  |  _ \ _   _| | |   |_   _| |__   ___  / ___|  __ _  ___| | __" + "\n" +
        @"  | |_) | | | | | |     | | | '_ \ / _ \ \___ \ / _` |/ __| |/ /" + "\n" +
        @"  |  __/| |_| | | |     | | | | | |  __/  ___) | (_| | (__|   < " + "\n" +
        @"  |_|    \__,_|_|_|     |_| |_| |_|\___| |____/ \__,_|\___|_|\_\";

    // Shared top portion: banner, empty space, then the rope/sack hanging down.
    private const string Head =
        Banner + "\n" +
        @"                |                                      " + "\n" +
        @"                |                                      " + "\n" +
        @"             .-``-.                                   " + "\n" +
        @"            /       \                                  " + "\n" +
        @"           |  $$$$$  |                                 " + "\n" +
        @"           | $$$$$$$ |                                 " + "\n" +
        @"           |  $$$$$  |                                 " + "\n" +
        @"            \       /                                  " + "\n" +
        @"             '-...-'                                   " + "\n";
    private const string Frame1 =
        Head + "\n" +
        @"             (    )                                    " + "\n" +
        @"            (  ()  )                                   " + "\n" +
        @"             (    )                                    ";

    private const string Frame2 =
        Head + "\n" +
        @"              ( | )                                    " + "\n" +
        @"              (   )                                    " + "\n" +
        @"              ( | )                                    ";

    private const string Frame3 =
        Head + "\n" +
        @"             ( -- )                                    " + "\n" +
        @"            (      )                                   " + "\n" +
        @"             ( -- )                                    ";

    private const string Frame4 =
        Head + "\n" +
        @"              ( | )                                    " + "\n" +
        @"              (   )                                    " + "\n" +
        @"              ( | )                                    ";
}