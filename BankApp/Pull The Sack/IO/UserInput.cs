namespace BankApp.Pull_The_Sack.IO;

using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Reads user input through the <see cref="IConsole"/> abstraction so that
/// input can be mocked/tested and flows through the shared console chokepoint.
/// </summary>
public class UserInput(IConsole console)
{
    /// <summary>Blocks until the user presses a key and returns just the key.</summary>
    public void ReadKeyCode(ConsoleKey key)
    {
        while (true)
        {
            ConsoleKey inputKey = console.ReadKey().Key;

            if (inputKey == key)
                break;

        }
    }

    public bool TryReadKeyCode(ConsoleKey key)
    {
        while (console.TryReadKey(out var info))
        {
            if (info.Key == key)
                return true;
        }
        return false;
    }
}