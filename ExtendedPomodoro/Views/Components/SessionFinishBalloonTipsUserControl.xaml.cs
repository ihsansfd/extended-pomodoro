using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ExtendedPomodoro.Views.Components
{
    /// <summary>
    /// Interaction logic for SessionFinishBalloonTipsUserControl.xaml
    /// </summary>
    public partial class SessionFinishBalloonTipsUserControl : UserControl
    {

        private TimeSpan _remainingTimeAlive;
        private DispatcherTimer _aliveTimer;

        private Run _remainingTimeInSecondsRunText;

        public SessionFinishBalloonTipsUserControl()
        {
            InitializeComponent();
            DataContext = this;

            _remainingTimeInSecondsRunText = (Run)FindName("RemainingTimeInSecondsRunText");
            _remainingTimeInSecondsRunText.Text = TimeSpan.FromMilliseconds(AutoCloseAfter).TotalSeconds.ToString();

            _remainingTimeAlive = TimeSpan.FromMilliseconds(AutoCloseAfter);

            _aliveTimer = new();
            _aliveTimer.Interval = TimeSpan.FromSeconds(1);
            _aliveTimer.Start();
            _aliveTimer.Tick += OnAliveTimerTickChanged;
        }

        private void OnAliveTimerTickChanged(object? sender, EventArgs e)
        {
            _remainingTimeAlive = _remainingTimeAlive.Subtract(TimeSpan.FromSeconds(1));

            _remainingTimeInSecondsRunText.Text = _remainingTimeAlive.TotalSeconds.ToString();

            if(_remainingTimeAlive <= TimeSpan.Zero) Close();
        }

        public int AutoCloseAfter
        {
            get { return (int)GetValue(AutoCloseAfterProperty); }
            set { SetValue(AutoCloseAfterProperty, value); }
        }

        public static readonly DependencyProperty AutoCloseAfterProperty =
            DependencyProperty.Register("AutoCloseAfter", typeof(int), typeof(SessionFinishBalloonTipsUserControl), new PropertyMetadata(15000));

        public TimerSessionState FinishedSession
        {
            get { return (TimerSessionState)GetValue(FinishedSessionProperty); }
            set { SetValue(FinishedSessionProperty, value); }
        }

        public static readonly DependencyProperty FinishedSessionProperty =
            DependencyProperty.Register("FinishedSession", typeof(TimerSessionState), typeof(SessionFinishBalloonTipsUserControl));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(SessionFinishBalloonTipsUserControl));

        public TimerSessionState NextSession
        {
            get { return (TimerSessionState)GetValue(NextSessionProperty); }
            set { SetValue(NextSessionProperty, value); }
        }

        public static readonly DependencyProperty NextSessionProperty =
            DependencyProperty.Register("NextSession", typeof(TimerSessionState), typeof(SessionFinishBalloonTipsUserControl));

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Close()
        {
            _aliveTimer.Stop();
            _aliveTimer.Tick -= OnAliveTimerTickChanged;
            Visibility = Visibility.Collapsed;
        }

        private void ButtonStartNextSession_Click(object sender, RoutedEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new StartSessionInfoMessage());
            Close();
        }
    }
}
