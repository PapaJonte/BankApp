namespace BankApp.Pull_The_Sack.Service;

using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Single shared console chokepoint. Serializes all console access so
/// rendering and input from different views don't interleave/corrupt.
/// </summary>
public class LockedConsole : IConsole
{
    private readonly object _gate = new();
    
    public int GetTerminalWidth()
    {
        int width;
        try { width = Console.WindowWidth; }
        catch (IOException)
        {
            width = 120; //just hardcode a number that would seem like it could fit a terminal
        }
        
        return width;
    }

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
    }

    /// <inheritdoc />
    public void SetCursor(int left, int top)
    {
        lock (_gate) { Console.SetCursorPosition(left, top); }
    }

    /// <inheritdoc />
    public void WriteAtCursor(string text)
    {
        lock (_gate) { Console.Write(text); }
    }

    /// <inheritdoc />
    public int Left
    {
        get { lock (_gate) { return Console.CursorLeft; } }
    }

    /// <inheritdoc />
    public int Top
    {
        get { lock (_gate) { return Console.CursorTop; } }
    }

    /// <inheritdoc />
    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(intercept: true);
    }
}


