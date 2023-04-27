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

namespace ExtendedPomodoro.Views.Components
{
    /// <summary>
    /// Interaction logic for TimerPausedBalloonTipsUserControl.xaml
    /// </summary>
    public partial class TimerPausedBalloonTipsUserControl : UserControl, ICloseableControl
    {
        private AutoCloseControlService _autoCloseService;

        public TimerPausedBalloonTipsUserControl()
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

        public void Close()
        {
            Visibility = Visibility.Collapsed;
        }

        public int AutoCloseAfter
        {
            get { return (int)GetValue(AutoCloseAfterProperty); }
            set { SetValue(AutoCloseAfterProperty, value); }
        }

        public static readonly DependencyProperty AutoCloseAfterProperty =
            DependencyProperty.Register("AutoCloseAfter",
                typeof(int), typeof(TimerPausedBalloonTipsUserControl), new PropertyMetadata(5000));

        public TimerSessionState CurrentSession
        {
            get { return (TimerSessionState)GetValue(CurrentSessionProperty); }
            set { SetValue(CurrentSessionProperty, value); }
        }

        public static readonly DependencyProperty CurrentSessionProperty =
            DependencyProperty.Register("CurrentSession", typeof(TimerSessionState), typeof(TimerPausedBalloonTipsUserControl));
    }
}
