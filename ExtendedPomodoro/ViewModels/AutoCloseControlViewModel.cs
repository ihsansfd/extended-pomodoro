using System;
using System.Windows.Documents;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExtendedPomodoro.ViewModels
{
    public partial class AutoCloseControlViewModel : ObservableObject
    {
        private readonly DispatcherTimer _timer;

        [ObservableProperty]
        private int _remainingTimeInSeconds;

        [ObservableProperty] 
        private bool _closeRequested;

        public int AutoCloseAfterInMilliseconds { get; set; } = 15000;

        public AutoCloseControlViewModel()
        {
            _timer = new DispatcherTimer();
        }

        public void Start()
        {
            RemainingTimeInSeconds = (int) TimeSpan.FromMilliseconds(AutoCloseAfterInMilliseconds)
                .TotalSeconds;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();
            _timer.Tick += OnTimerTickChanged;
        }

        private void OnTimerTickChanged(object? sender, EventArgs e)
        {
            RemainingTimeInSeconds -= (int)_timer.Interval.TotalSeconds;

            if (RemainingTimeInSeconds <= 0) Close();
        }

        public virtual void Close()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTickChanged;
            CloseRequested = true;
        }
    }
}
