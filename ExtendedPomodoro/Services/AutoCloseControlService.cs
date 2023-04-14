using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows;

namespace ExtendedPomodoro.Services
{
    public interface ICloseableControl
    {
        void Close();
    }

    public class AutoCloseControlService
    {
        private TimeSpan _remainingTimeAlive;
        private readonly DispatcherTimer _aliveTimer;

        private readonly Run _remainingTimeInSecondsRun;

        private ICloseableControl _theControl;

        public int AutoCloseAfter { get; set; } = 15000;

        public AutoCloseControlService(ICloseableControl theControl, 
            Run remainingTimeInSecondsRun)
        {
            _theControl = theControl;

            _aliveTimer = new();

            _remainingTimeInSecondsRun = remainingTimeInSecondsRun;
        }

        public void Start()
        {
            _remainingTimeInSecondsRun.Text = TimeSpan.FromMilliseconds(AutoCloseAfter)
                .TotalSeconds.ToString();

            _remainingTimeAlive = TimeSpan.FromMilliseconds(AutoCloseAfter);

            _aliveTimer.Interval = TimeSpan.FromSeconds(1);
            _aliveTimer.Start();
            _aliveTimer.Tick += OnAliveTimerTickChanged;
        }

        private void OnAliveTimerTickChanged(object? sender, EventArgs e)
        {
            _remainingTimeAlive = _remainingTimeAlive.Subtract(TimeSpan.FromSeconds(1));

            _remainingTimeInSecondsRun.Text = _remainingTimeAlive.TotalSeconds.ToString();

            if (_remainingTimeAlive <= TimeSpan.Zero) Close();
        }

        public void Close()
        {
            _aliveTimer.Stop();
            _aliveTimer.Tick -= OnAliveTimerTickChanged;
            _theControl.Close();
        }
    }
}
