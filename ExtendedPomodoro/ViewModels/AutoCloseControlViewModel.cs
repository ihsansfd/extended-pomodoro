using System;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
            CloseCommand = new RelayCommand(Close);
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

        public ICommand CloseCommand { get; }

        protected virtual void Close()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTickChanged;
            CloseRequested = true;
        }
    }
}
