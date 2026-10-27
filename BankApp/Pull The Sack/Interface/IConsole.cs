namespace BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Thin abstraction over the console so rendering can be tested
/// and so all writes flow through a single chokepoint.
/// </summary>
public interface IConsole
{
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

    /// <summary>Moves the cursor to an absolute position.</summary>
    void SetCursorPosition(int left, int top);

    /// <summary>Reads a single key press (non-echoing) from the console.</summary>
    ConsoleKeyInfo ReadKey();
}