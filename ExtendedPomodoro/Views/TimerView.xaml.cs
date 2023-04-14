using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Controls;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for TimerView.xaml
    /// </summary>
    public partial class TimerView : Page,
        IRecipient<TasksComboBoxAddNewButtonPressedMessage>,
        IRecipient<TimerSessionFinishInfoMessage>,
        IRecipient<SessionFinishBalloonMessage>,
        IRecipient<ModalContentSessionFinishMessage>,
        IRecipient<TimerActionChangeInfoMessage>
    {
        private readonly SoundService _soundService = new();
        private readonly TaskbarIcon _taskbarIcon = new();

        private ICloseableControl? _currentSessionFinishBalloon;
        private ICloseableControl _currentTimerManiputedByHotkeyBalloon;

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
                ShowSessionFinishBalloonTips(timerSessionFinishInfo.FinishedSession, timerSessionFinishInfo.NextSession);
                ShowSessionFinishModal(timerSessionFinishInfo.FinishedSession, timerSessionFinishInfo.NextSession);

                _soundService.Volume = timerSessionFinishInfo.AlarmVolume;
                _soundService.RepeatFor = 
                    timerSessionFinishInfo.IsAlarmSoundRepeatForever ? Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(15);
                _soundService.Play(timerSessionFinishInfo.AlarmSound);
            }
            
        }

        private void ShowSessionFinishModal(TimerSessionState finishedSession, TimerSessionState nextSession)
        {
            ModalSessionFinish.ClearValue(ContentProperty);
            ModalSessionFinish.IsShown = true;
            ModalSessionFinish.Content = new ModalContentSessionFinishUserControl()
            {
                FinishedSession = finishedSession,
                NextSession = nextSession,
            };
        }

        private void ShowSessionFinishBalloonTips(TimerSessionState finishedSession, TimerSessionState nextSession)
        {
            _taskbarIcon.CloseBalloon();
            _currentSessionFinishBalloon?.Close();

            var sessionFinishBalloon = new SessionFinishBalloonTipsUserControl()
            {
                FinishedSession = finishedSession,
                NextSession = nextSession,
                AutoCloseAfter = 15000 // in milliseconds
            };

            _currentSessionFinishBalloon = sessionFinishBalloon;


            _taskbarIcon.ShowCustomBalloon(sessionFinishBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 15000);
        }

        public void Receive(SessionFinishBalloonMessage message)
        {
            if(message.Closed)
            {
                ModalSessionFinish.ClearValue(ContentProperty);
                ModalSessionFinish.IsShown = false;
                _soundService.Stop();
            }
        }

        public void Receive(ModalContentSessionFinishMessage message)
        {
            if (message.Closed)
            {
                _currentSessionFinishBalloon?.Close();
                ModalSessionFinish.IsShown = false;
                _soundService.Stop();
            }
        }

        public void Receive(TimerActionChangeInfoMessage message)
        {
            _currentTimerManiputedByHotkeyBalloon?.Close();
            _taskbarIcon.CloseBalloon();

            if(message.TimerAction == TimerAction.Start || message.TimerAction == TimerAction.Pause)
            {
                _soundService.PlayMouseClickEffect();
            }

           if (message.TimerAction == TimerAction.Start)
            {
                if (!message.TriggeredByHotkey) return;

                var timerStartedBalloon = new TimerStartedBalloonTipsUserControl()
                {
                    CurrentSession = message.CurrentSession,
                    AutoCloseAfter = 5000 // in milliseconds
                };

                _currentTimerManiputedByHotkeyBalloon = timerStartedBalloon;

                _taskbarIcon.ShowCustomBalloon(timerStartedBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 5000);
            }

            else if (message.TimerAction == TimerAction.Pause)
            {

                if (!message.TriggeredByHotkey) return;

                var timerPausedBalloon = new TimerPausedBalloonTipsUserControl()
                {
                    CurrentSession = message.CurrentSession,
                    AutoCloseAfter = 5000 // in milliseconds
                };

                _currentTimerManiputedByHotkeyBalloon = timerPausedBalloon;

                _taskbarIcon.ShowCustomBalloon(timerPausedBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 5000);
            }
        }

        ~TimerView()
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
