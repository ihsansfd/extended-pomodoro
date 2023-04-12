using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using Hardcodet.Wpf.TaskbarNotification;
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

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for TimerView.xaml
    /// </summary>
    public partial class TimerView : Page,
        IRecipient<TasksComboBoxAddNewButtonPressedMessage>,
        IRecipient<TimerSessionFinishInfoMessage>
    {
        public TimerView()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(TasksComboBoxAddNewButtonPressedMessage message)
        {
            ModalCreateTask.IsShown = true;
            TasksComboBox.IsDropDownOpen = false;
        }

        public void Receive(TimerSessionFinishInfoMessage timerSessionFinishInfo)
        {
            if(timerSessionFinishInfo.PushNotificationEnabled)
            {
                ShowCompletedBalloonTips(timerSessionFinishInfo.FinishedSession, timerSessionFinishInfo.NextSession);
            }   
        }

        private void ShowCompletedBalloonTips(TimerSessionState finishedSession, TimerSessionState nextSession)
        {
            TaskbarIcon tbi = new TaskbarIcon();

            var theBalloonTips = new SessionFinishBalloonTipsUserControl()
            {
                FinishedSession = finishedSession,
                NextSession = nextSession,
                AutoCloseAfter = 15000 // in milliseconds
            };

            tbi.ShowCustomBalloon(theBalloonTips, System.Windows.Controls.Primitives.PopupAnimation.Slide, 15000);
        }

        ~TimerView()
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
