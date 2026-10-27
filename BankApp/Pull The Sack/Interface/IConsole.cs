namespace BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Thin abstraction over the console so rendering can be tested
/// and so all writes flow through a single chokepoint.
/// </summary>
public interface IConsole
{
    int GetTerminalWidth();
    /// <summary>Clears the console.</summary>
    void Clear();

    /// <summary>Writes a string without a trailing newline.</summary>
    void Write(string text);

    /// <summary>Writes a line of text.</summary>
    void WriteLine(string text = "");

    /// <summary>Writes a line of text horizontally centered within the console window.</summary>
    void WriteCentered(string text);

    /// <summary>Writes a line of text horizontally centered within the given width.</summary>
    void WriteCentered(string text, int width);

    /// <summary>Moves the cursor to an absolute position without writing anything.</summary>
    void SetCursor(int left, int top);

    /// <summary>
    /// Writes <paramref name="text"/> at the current cursor position. Does not emit a
    /// newline and does not advance to the next line; placement is the caller's concern.
    /// </summary>
    void WriteAtCursor(string text);

    /// <summary>Current cursor column (0-based, from the left).</summary>
    int Left { get; }

    /// <summary>Current cursor row (0-based, from the top).</summary>
    int Top { get; }

    /// <summary>Reads a single key press (non-echoing) from the console.</summary>
    ConsoleKeyInfo ReadKey();
}