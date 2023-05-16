using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Messages;
using System;
using ExtendedPomodoro.Services.Interfaces;

namespace ExtendedPomodoro.Services
{
    public class RemainingTimeChangedEventArgs : EventArgs
    {
        public TimeSpan RemainingTime { get; init; }
        public TimeSpan TimerSetFor { get; init; }
    }

    public class ExtendedTimer : IExtendedTimer
    {
        public event EventHandler<RemainingTimeChangedEventArgs>? RemainingTimeChanged;
        private readonly ITimer _timer;
        private TimeSpan _timerSetFor  = TimeSpan.Zero;
        private TimeSpan _remainingTime  = TimeSpan.Zero;
        
        public TimeSpan Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }
        
        public ExtendedTimer(ITimer timer)
        {
            _timer = timer;
            _timer.Tick += OnTimeChanged;
            _timer.Interval = TimeSpan.FromSeconds(1);
        }

        public void Initialize(TimeSpan timerSetFor)
        {
            _timerSetFor = timerSetFor;
            _remainingTime = timerSetFor;
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
        public void Stop()
        {
            _timer.Stop();
        }

        private void OnTimeChanged(object? sender, EventArgs e)
        {
            _remainingTime -= Interval;

            if (_remainingTime <= TimeSpan.Zero) _timer.Stop();

            RemainingTimeChanged?.Invoke(this, new RemainingTimeChangedEventArgs
            {
                RemainingTime = _remainingTime,
                TimerSetFor = _timerSetFor
            });
        }
        
        ~ExtendedTimer() {
            _timer.Tick -= OnTimeChanged;
        }
    }
}
