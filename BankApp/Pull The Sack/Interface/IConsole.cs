namespace BankApp.Pull_The_Sack.Interface;


/// <summary>
/// Interface to funnel all user input through one object. This is used to lock the console since we'll write from multiple threads.
/// </summary>
public interface IConsole
{
    IDisposable Lock();
    bool TryReadKey(out ConsoleKeyInfo key);
    int GetTerminalWidth();
    void Clear();
    void Write(string text);
    void WriteLine(string text = "");
    void WriteCentered(string text);
    void WriteCentered(string text, int width);
    void SetCursor(int left, int top);
    void WriteAtCursor(string text);
    void SetConsoleColor(ConsoleColor color);
    int Left { get; }
    int Top { get; }
    ConsoleKeyInfo ReadKey();
}