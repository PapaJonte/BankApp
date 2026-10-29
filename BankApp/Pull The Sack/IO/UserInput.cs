namespace BankApp.Pull_The_Sack.IO;

using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Reads user input using the locked console object shared across slot game.
/// </summary>
/// <param name="console"></param>
public class UserInput(IConsole console)
{

    /// <summary>
    /// Runs loop listening for specific user input. Break loop when condition is met.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void ReadKeyCode(ConsoleKey key)
    {
        while (true)
        {
            ConsoleKey inputKey = console.ReadKey().Key;

            if (inputKey == key)
                break;

        }
    }


    //Look at TryReadKey comment for more detail as to why this is important instead of only using the ReadKeyCode.
    /// <summary>
    /// poll the input buffer for key press this tick, once the buffer is clean exit the loop. Preventing indefinite blocking by listening for console input. 
    /// </summary>
    /// <param name="">
    /// ConsoleKey key is the key we want to listen for this tick.
    /// </param>
    /// <returns></returns>
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