namespace BankApp.Pull_The_Sack.Service;

using BankApp.Pull_The_Sack.Interface;


/// <summary>
/// Single shared console chokepoint. Serializes all console access so
/// rendering and input from different views don't interleave/corrupt.
/// </summary>
public class LockedConsole : IConsole
{

    /// <summary>
    /// Releaser class responsible for disposing of the locked object
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private sealed class Releaser(object Lock) : IDisposable
    {
        private object? _lock = Lock;
        public void Dispose()
        {
            /*
            ok so if I understand this correctly there are cases/bugs where two threads can hold the same 
            Releaser object ref, and if that happens we want to guard so that the Dispose calls after release does not 
            exit with a lock that has already been used for exit (since its already released). 
            We use Interlocked.Exchange which makes sure that only one thread gets the lock obbject back to call the Monitor.Exit.
            Interlocked.Exchange basically gives and sets to null in one call which prevents
            the second thread from accessing and returning the same object. 
            */
            var g = Interlocked.Exchange(ref _lock, null);
            if (g != null) Monitor.Exit(g);
        }
    }

    private readonly object consoleLock = new();

    /*
    Using try read key here because listening for input even from another thread seems to cause
    the host to write from buffer and flush it more frequently, causing flickering during
    the GameView slot machine graphic loop. I have no idea why this happens 
    and haven't found any comments on it on google. Asked GLM and it's solution was to 
    only check for available key and not hog the thread with read key. It works, and it's 
    explanation was this : 
    
    "why exactly the outstanding read degraded paint batching. 
    My "eager servicing of the active connection" explanation is the model that fits the observations — 
    the outstanding read keeps your process's console connection "hot" in the host's scheduling 
    so paints get serviced in smaller batches during your write burst. I flagged it as inference 
    last time and I'll keep it flagged: conhost's exact thread/paint scheduling under an 
    outstanding ReadConsoleInput isn't something I can cite documentation for."
    */
    public bool TryReadKey(out ConsoleKeyInfo key)
    {
        key = default;

        bool available;
        lock (consoleLock) { available = Console.KeyAvailable; }

        if (!available)
            return false;

        key = Console.ReadKey(intercept: true);
        return true;
    }

    /// <summary>
    /// So this was a later addition which I think mostly render the individual lock functionality redundant but I keep them
    /// since they make sure we still lock incase we miss using this lock function.
    /// Basically this allows us to lock a sequence of console logs instead of having every call lock individually.
    /// This prevent some flickering when we write a lot of graphic to the terminal in one long sequence.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public IDisposable Lock()
    {
        Monitor.Enter(consoleLock);
        return new Releaser(consoleLock);
    }

    /// <summary>
    /// Returns width of console window. If we can't fetch it return arbitrary value
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
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

    /// <summary>
    /// Lock and clear
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void Clear()
    {
        lock (consoleLock) { Console.Clear(); }
    }

    /// <summary>
    /// lock and write
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void Write(string text)
    {
        lock (consoleLock) { Console.Write(text); }
    }

    /// <summary>
    /// lock and write line
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void WriteLine(string text = "")
    {
        lock (consoleLock) { Console.WriteLine(text); }
    }

    /// <summary>
    /// Fetch console width, or set it to text length. Call WriteCentered overload
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void WriteCentered(string text)
    {
        int width;
        try { width = Console.WindowWidth; }
        catch (IOException) { width = text.Length; }
        WriteCentered(text, width);
    }

    /// <summary>
    /// Calculate left padding based on console width and width of text
    /// and add padding to left before writing.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void WriteCentered(string text, int width)
    {
        lock (consoleLock)
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

    /// <summary>
    /// SEt cursor position at left,top
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void SetCursor(int left, int top)
    {
        lock (consoleLock) { Console.SetCursorPosition(left, top); }
    }

    /// <summary>
    /// Write text at current cursor pos
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void WriteAtCursor(string text)
    {
        lock (consoleLock) { Console.Write(text); }
    }

    /// <summary>
    /// return cursor X axis
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public int Left
    {
        get { lock (consoleLock) { return Console.CursorLeft; } }
    }

    /// <summary>
    /// return cursor Y axis
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public int Top
    {
        get { lock (consoleLock) { return Console.CursorTop; } }
    }

    /// <summary>
    /// lock and readkey
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey(intercept: true);
    }

    /// <summary>
    /// lock and set console color
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public void SetConsoleColor(ConsoleColor color)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = color;
        }
    }

}


