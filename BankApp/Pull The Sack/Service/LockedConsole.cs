namespace BankApp.Pull_The_Sack.Service;

using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Single shared console chokepoint. Serializes all console access so
/// rendering and input from different views don't interleave/corrupt.
/// </summary>
public class LockedConsole : IConsole
{
    private readonly object _gate = new();

    /// <inheritdoc />
    public void Clear()
    {
        lock (_gate) { Console.Clear(); }
    }

    /// <inheritdoc />
    public void Write(string text)
    {
        lock (_gate) { Console.Write(text); }
    }

    /// <inheritdoc />
    public void WriteLine(string text = "")
    {
        lock (_gate) { Console.WriteLine(text); }
    }

    /// <inheritdoc />
    public void WriteCentered(string text)
    {
        int width;
        try { width = Console.WindowWidth; }
        catch (IOException) { width = text.Length; }
        WriteCentered(text, width);
    }

    /// <inheritdoc />
    public void WriteCentered(string text, int width)
    {
        lock (_gate)
        {
            int left = Math.Max(0, (width - text.Length) / 2);
            Console.Write(new string(' ', left));
            Console.WriteLine(text);
        }
    }

    /// <inheritdoc />
    public void SetCursorPosition(int left, int top)
    {
        lock (_gate) { Console.SetCursorPosition(left, top); }
    }

    /// <inheritdoc />
    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(intercept: true);
    }
}
