using System;

namespace BankApp;

public class LogoConsole
{
    public static void WriteCentered(string text)
    {
        int width;
        try { width = Console.WindowWidth; }
        catch (IOException) { width = text.Length; }
        WriteCentered(text, width);
    }

    /// <inheritdoc />
    private static void WriteCentered(string text, int width)
    {
        var lines = text.Split('\n');
        int blockWidth = lines.Max(l => l.TrimEnd().Length);
        int left = Math.Max(0, (width - blockWidth) / 2);
        string pad = new(' ', left);

        foreach (var line in lines)
        {
            Console.Write(pad);
            Console.WriteLine(line.TrimEnd());
        }
    }

    public static void SetConsoleColor(ConsoleColor color)
    {
        Console.ForegroundColor = color;
    }
}
