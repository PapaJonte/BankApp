using System;

namespace BankApp.Transaction_Layer.Service;

public class TransactionScheduler
{

    public float TimeScale { get; set; } = 1;
    private readonly int scheduleDurationInMinutes = 15;
    private readonly CancellationTokenSource ct = new();
    private Action? OnSchedule;

    public void StartSchedule(Action OnScheduleListener)
    {
        OnSchedule += OnScheduleListener;
        _ = TransactionSchedule(scheduleDurationInMinutes * 1000, ct.Token);
    }

    public void CancelSchedule()
    {
        ct.Cancel();
        ct.Dispose();
    }

    private async Task TransactionSchedule(int duration, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(duration, ct);
            OnSchedule?.Invoke();
        }
    }
}
