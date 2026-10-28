namespace BankApp.Pull_The_Sack.Service;

using BankApp.Pull_The_Sack.Interface;


/// <summary>
/// Single shared console chokepoint. Serializes all console access so
/// rendering and input from different views don't interleave/corrupt.
/// </summary>
public class LockedConsole : IConsole
{

    private sealed class Releaser(object gate) : IDisposable
    {
        private object? _gate = gate;
        public void Dispose()
        {
            var g = Interlocked.Exchange(ref _gate, null);
            if (g is not null) Monitor.Exit(g);
        }
    }

    private readonly object _gate = new();

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
        lock (_gate) { available = Console.KeyAvailable; }

        if (!available)
            return false;

        key = Console.ReadKey(intercept: true); // key is guaranteed present → returns instantly
        return true;
    }



    public IDisposable Lock()
    {
        Monitor.Enter(_gate);
        return new Releaser(_gate);
    }

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

    public void SetConsoleColor(ConsoleColor color)
    {
        lock (_gate)
        {
            Console.ForegroundColor = color;
        }
    }

}


