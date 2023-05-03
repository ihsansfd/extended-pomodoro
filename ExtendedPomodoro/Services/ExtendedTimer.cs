using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Messages;
using System;
using System.Windows.Threading;

namespace ExtendedPomodoro.Services
{
    public class RemainingTimeChangedEventArgs : EventArgs
    {
        public TimeSpan RemainingTime { get; init; }
        public TimeSpan TimerSetFor { get; init; }
    }

    public class ExtendedTimer
    {
        public event EventHandler<RemainingTimeChangedEventArgs>? RemainingTimeChanged;
        private readonly DispatcherTimer _timer;
        private TimeSpan _interval = TimeSpan.FromSeconds(1);
        private TimeSpan _timerSetFor  = TimeSpan.Zero;
        private TimeSpan _remainingTime  = TimeSpan.Zero;
        
        public TimeSpan Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                _timer.Interval = _interval;
            }
        }
        
        public ExtendedTimer()
        {
            _timer = new();
            _timer.Tick += OnTimeChanged;
            _timer.Interval = Interval;
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

        // public void SetInterval(TimeSpan timeSpan) => _interval = timeSpan; 

        private void OnTimeChanged(object? sender, EventArgs e)
        {
            _remainingTime = _remainingTime.Subtract(Interval);

            RemainingTimeChanged?.Invoke(this, new()
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
