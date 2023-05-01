using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Messages;
using System;
using System.Windows.Threading;

namespace ExtendedPomodoro.Services
{
    public class ExtendedTimerEventArgs : EventArgs
    {
        public TimeSpan CurrentTime { get; set; }
    }

    public class ExtendedTimer
    {
        private static readonly TimeSpan _INTERVAL = TimeSpan.FromSeconds(1);
        private readonly IMessenger _messenger = MessengerFactory.Messenger;
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
            _messenger.Send(new TimerTimeChangeInfoMessage(_timerSetFor, _remainingTime));
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

           _messenger.Send(new TimerTimeChangeInfoMessage(_timerSetFor, _remainingTime));
        }

        internal void Stop()
        {
            _timer.Stop();
        }
    }
}
