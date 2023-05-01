using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Services;
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
    public partial class SessionFinishBalloonTipsUserControl : UserControl, ICloseableControl
    {
        private readonly AutoCloseControlService _autoCloseService;
        private readonly IMessenger _messenger = MessengerFactory.Messenger;
        public event EventHandler<EventArgs>? Closed;

        public SessionFinishBalloonTipsUserControl()
        {
            InitializeComponent();
            DataContext = this;
            _autoCloseService = new AutoCloseControlService(this, PART_RemainingTimeInSecondsRun);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _autoCloseService.AutoCloseAfter = AutoCloseAfter;
            _autoCloseService.Start();
        }

        public int AutoCloseAfter
        {
            get { return (int)GetValue(AutoCloseAfterProperty); }
            set { SetValue(AutoCloseAfterProperty, value); }
        }

        public static readonly DependencyProperty AutoCloseAfterProperty =
            DependencyProperty.Register("AutoCloseAfter", 
                typeof(int), typeof(SessionFinishBalloonTipsUserControl), new PropertyMetadata(15000));

        public TimerSessionState FinishedSession
        {
            get { return (TimerSessionState)GetValue(FinishedSessionProperty); }
            set { SetValue(FinishedSessionProperty, value); }
        }

        public static readonly DependencyProperty FinishedSessionProperty =
            DependencyProperty.Register("FinishedSession", typeof(TimerSessionState), typeof(SessionFinishBalloonTipsUserControl));

        public TimerSessionState NextSession
        {
            get { return (TimerSessionState)GetValue(NextSessionProperty); }
            set { SetValue(NextSessionProperty, value); }
        }

        public static readonly DependencyProperty NextSessionProperty =
            DependencyProperty.Register("NextSession", typeof(TimerSessionState), typeof(SessionFinishBalloonTipsUserControl));

        public void Close()
        {
            Visibility = Visibility.Collapsed;
            Closed?.Invoke(this, new());
        }

        private void ButtonStartNextSession_Click(object sender, RoutedEventArgs e)
        {
            _messenger.Send(new StartSessionInfoMessage());
            Close();
        }
    }
}