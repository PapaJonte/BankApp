using System;

namespace BankApp.Transaction_Layer.Service;

public class TransactionScheduler
{

    public float TimeScale { get; set; } = 1;
    private readonly int scheduleDurationInMinutes = 15;
    private readonly CancellationTokenSource ct = new();
    private Action? OnSchedule;

    /// <summary>
    /// Responsible for starting the transaction schedule.
    /// Subscribes the given listener and starts the recurring scheduling loop.
    /// </summary>
    /// <param name="">
    /// Action OnScheduleListener, the callback invoked each time the schedule triggers.
    /// </param>
    public void StartSchedule(Action OnScheduleListener)
    {
        OnSchedule += OnScheduleListener;
        _ = TransactionSchedule(scheduleDurationInMinutes * 1000, ct.Token);
    }

    /// <summary>
    /// Responsible for cancelling the transaction schedule.
    /// Cancels and disposes the underlying cancellation token source to stop the scheduling loop.
    /// </summary>
    public void CancelSchedule()
    {
        ct.Cancel();
        ct.Dispose();
    }

    /// <summary>
    /// Responsible for running the recurring scheduling loop.
    /// Waits for the given duration repeatedly and invokes the schedule callback until cancellation is requested.
    /// </summary>
    /// <param name="">
    /// int duration, the delay in milliseconds between each schedule trigger.
    /// CancellationToken ct, token used to stop the scheduling loop.
    /// </param>
    private async Task TransactionSchedule(int duration, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(duration, ct);
            OnSchedule?.Invoke();
        }
    }
}

