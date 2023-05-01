using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Messages;
using System;
using System.Windows.Threading;

namespace ExtendedPomodoro.Services
{
    public class RemainingTimeChangedEventArgs : EventArgs
    {
        public TimeSpan RemainingTime { get; set; }
        public TimeSpan TimerSetFor { get; set; }
    }

    public class ExtendedTimer
    {
        public event EventHandler<RemainingTimeChangedEventArgs>? RemainingTimeChanged;
        
        private static readonly TimeSpan _INTERVAL = TimeSpan.FromSeconds(1);
        private readonly DispatcherTimer _timer;

        private TimeSpan _timerSetFor  = TimeSpan.Zero;
        private TimeSpan _remainingTime  = TimeSpan.Zero;
        
        public ExtendedTimer()
        {
            _timer = new();
            _timer.Tick += OnTimeChanged;
        }

        public void Initialize(TimeSpan timerSetFor)
        {
            _timerSetFor = timerSetFor;
            _remainingTime = timerSetFor;
            _timer.Interval = _INTERVAL;
            
            RemainingTimeChanged?.Invoke(this, new()
            {
                RemainingTime = _remainingTime,
                TimerSetFor = _timerSetFor
            });
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Resume()
        {
            _timer.Start();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        private void OnTimeChanged(object? sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(_INTERVAL);

            RemainingTimeChanged?.Invoke(this, new()
            {
                RemainingTime = _remainingTime,
                TimerSetFor = _timerSetFor
            });
        }

        internal void Stop()
        {
            _timer.Stop();
        }
    }
}
