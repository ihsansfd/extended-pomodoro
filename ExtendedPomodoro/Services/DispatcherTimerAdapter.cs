using System;
using System.Windows.Threading;
using ExtendedPomodoro.Services.Interfaces;

namespace ExtendedPomodoro.Services;

public class DispatcherTimerAdapter : ITimer
{
    private readonly DispatcherTimer _timer = new();

    public event EventHandler? Tick
    {
        add => _timer.Tick += value;
        remove => _timer.Tick -= value;
    }

    public TimeSpan Interval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }
}