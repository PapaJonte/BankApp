namespace BankApp.Pull_The_Sack.Service;

using System.Diagnostics;
using BankApp.Pull_The_Sack.Interface;

/// <summary>
/// Async game loop that drives the view updates.
/// </summary>
/// <param name="updater"></param>
/// <param name="framesPerSecond"></param>
public class GameLoop(IViewUpdater updater, int framesPerSecond = 4)
{
    private CancellationTokenSource _glts = new();
    private Stopwatch _gameTime = new();

    /// <summary>
    /// Start the async gameplay loop.
    /// </summary>
    public void StartGameLoopAsync()
    {
        int delayMs = (int)Math.Round(1000.0 / framesPerSecond);
        _ = RunAsync(delayMs, _glts.Token);
    }

    /// <summary>
    /// Stops the async gameplay loop
    /// </summary>
    public void StopGameLoopAsync() => ResetCancellationToken();

    /// <summary>Runs the loop until the provided token is cancelled.</summary>
    private async Task RunAsync(int delay, CancellationToken ct)
    {
        _gameTime.Start();
        long prevTick = _gameTime.ElapsedMilliseconds;

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(delay, ct);
            long currentTick = _gameTime.ElapsedMilliseconds;
            double deltaTime = currentTick - prevTick;
            prevTick = currentTick;
            updater.Tick(deltaTime);
        }
        _gameTime.Reset();
    }

    /// <summary>
    /// cancel and dispose of current token, creates a new after.
    /// </summary>
    private void ResetCancellationToken()
    {
        if (_glts != null)
        {
            _glts.Cancel();
            _glts.Dispose();
        }

        _glts = new();
    }
}

