using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;

namespace ExtendedPomodoro.ViewServices
{
    public class TimerViewService
    {
        public bool AutoCloseDialogAndSound { get; set; }

        private readonly DialogWindowService _dialogWindowService;
        private readonly SoundService _soundService = new();
        private readonly TaskbarIcon _taskbarIcon = new();
        private Window _currentSessionFinishDialog;
        private SessionFinishBalloonTipsUserControl? _currentSessionFinishBalloon;
        private ICloseableControl? _currentTimerManiputedByHotkeyBalloon;

        public TimerViewService(DialogWindowService dialogWindowService)
        {
            _dialogWindowService = dialogWindowService;
        }

        public void PlayMouseClickEffectSound(int volume = 50)
        {
            _soundService.Volume = volume;
            _soundService.PlayMouseClickEffect();
        }

        public void OpenSessionFinishDialog(TimerSessionState finishedSession,
            TimerSessionState nextSession)
        {
            var userControl = new ModalContentSessionFinishUserControl()
            {
                NextSession = nextSession,
                FinishedSession = finishedSession,
            };
            var dialog = _dialogWindowService.GenerateClosableDialogWindow(
                new(320, 260, "Session Has Finished", userControl));
            _currentSessionFinishDialog = dialog;
            dialog.Show();
            dialog.Focus();
            dialog.Activate();
            dialog.Unloaded += OnCloseSessionFinishDialog;
        }

        public void ShowSessionFinishBalloonTips(TimerSessionState finishedSession,
            TimerSessionState nextSession)
        {
            _taskbarIcon.CloseBalloon();
            _currentSessionFinishBalloon?.Close();

            var sessionFinishBalloon = new SessionFinishBalloonTipsUserControl()
            {
                FinishedSession = finishedSession,
                NextSession = nextSession,
                AutoCloseAfter = 15000 // in milliseconds
            };

            sessionFinishBalloon.Closed += OnCloseSessionFinishBalloon;

            _currentSessionFinishBalloon = sessionFinishBalloon;

            _taskbarIcon.ShowCustomBalloon(sessionFinishBalloon,
                System.Windows.Controls.Primitives.PopupAnimation.Slide, 15000);
        }

        public void PlaySound(AlarmSound alarmSound, int volume, TimeSpan repeatFor)
        {
            _soundService.Volume = volume;
            _soundService.RepeatFor = repeatFor;
            _soundService.Play(alarmSound);
        }

        public void ShowTimerStartedBalloonTips(TimerSessionState currentSession, int autoCloseAfter = 5000)
        {
            CloseTimerActionBalloon();

            var timerStartedBalloon = new TimerStartedBalloonTipsUserControl()
            {
                CurrentSession = currentSession,
                AutoCloseAfter = autoCloseAfter // in milliseconds
            };

            _currentTimerManiputedByHotkeyBalloon = timerStartedBalloon;

            _taskbarIcon.ShowCustomBalloon(timerStartedBalloon,
                System.Windows.Controls.Primitives.PopupAnimation.Slide, autoCloseAfter);
        }

        public void ShowTimerPausedBalloonTips(TimerSessionState currentSession, int autoCloseAfter = 5000)
        {
            CloseTimerActionBalloon();

            var timerPausedBalloon = new TimerPausedBalloonTipsUserControl()
            {
                CurrentSession = currentSession,
                AutoCloseAfter = autoCloseAfter // in milliseconds
            };

            _currentTimerManiputedByHotkeyBalloon = timerPausedBalloon;

            _taskbarIcon.ShowCustomBalloon(timerPausedBalloon,
                System.Windows.Controls.Primitives.PopupAnimation.Slide, autoCloseAfter);
        }

        public void OnCloseSessionFinishBalloon(object? sender, EventArgs e)
        {
            if(AutoCloseDialogAndSound)
            {
                _currentSessionFinishDialog?.Close();
                _soundService.Stop();
            }

            if (_currentSessionFinishBalloon != null)
            {
                _currentSessionFinishBalloon.Closed -= OnCloseSessionFinishBalloon;
            }

        }

        private void OnCloseSessionFinishDialog(object sender, System.Windows.RoutedEventArgs e)
        {
            _currentSessionFinishBalloon?.Close();
            _soundService.Stop();

            if (_currentSessionFinishDialog != null)
            {
                _currentSessionFinishDialog.Unloaded -= OnCloseSessionFinishDialog;
            }
        }

        private void CloseTimerActionBalloon()
        {
            _currentTimerManiputedByHotkeyBalloon?.Close();
            _taskbarIcon.CloseBalloon();
        }
    }
}
