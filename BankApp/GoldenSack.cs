using System;
using BankApp.Pull_The_Sack.Interface;

namespace BankApp;

/// <summary>
/// ASCII art branding for the app. The logo is rendered as stacked
/// ASCII block letters, one word per line, so it stays legible
/// and pops in the terminal.
/// </summary>
public static class GoldenSack
{
    /// <summary>
    /// "THE / GOLDEN / SACK" rendered as stacked ASCII block letters in a
    /// single multi-line string. Use <see cref="WriteLogo"/> to print it.
    /// </summary>
    public const string Logo =
@"             ████████╗██╗  ██╗███████╗
             ╚══██╔══╝██║  ██║██╔════╝
                ██║   ███████║█████╗
                ██║   ██╔══██║██╔══╝
                ██║   ██║  ██║███████╗

 ██████╗  ██████╗ ██╗     ██████╗ ███████╗███╗   ██╗
██╔════╝ ██╔═══██╗██║     ██╔══██╗██╔════╝████╗  ██║
██║  ███╗██║   ██║██║     ██║  ██║█████╗  ██╔██╗ ██║
██║   ██║██║   ██║██║     ██║  ██║██╔══╝  ██║╚██╗██║
╚██████╔╝╚██████╔╝███████╗██████╔╝███████╗██║ ╚████║

          ███████╗ █████╗  ██████╗██╗  ██╗
          ██╔════╝██╔══██╗██╔════╝██║ ██╔╝
          ███████╗███████║██║     █████╔╝
          ╚════██║██╔══██║██║     ██╔═██╗
          ███████║██║  ██║╚██████╗██║  ██╗

          Sponsored by LiveForGambling.com
";

    /// <summary>
    /// Writes the logo centered in gold. Routes through the shared
    /// <see cref="IConsole"/> when supplied (so it respects the console
    /// lock), otherwise writes straight to <see cref="Console"/>.
    /// </summary>
    public static void WriteLogo()
    {   
        LogoConsole.SetConsoleColor(ConsoleColor.Yellow);
        LogoConsole.WriteCentered(Logo);
        LogoConsole.SetConsoleColor(ConsoleColor.Gray);
    }
}

