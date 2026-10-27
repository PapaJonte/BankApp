namespace BankApp.Pull_The_Sack.View;

using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Shared rendering helpers (centering, clearing, drawing rows) used by all views.
/// </summary>
public static class ViewHelper
{
    /// <summary>Clears the console via the abstraction.</summary>
    public static void Clear(IConsole console) => console.Clear();

    /// <summary>Writes <paramref name="text"/> centered within <paramref name="width"/> columns.</summary>
    public static void WriteCentered(IConsole console, string text, int width)
    {
        var pad = Math.Max(0, (width - text.Length) / 2);
        console.WriteLine(new string(' ', pad) + text);
    }
}