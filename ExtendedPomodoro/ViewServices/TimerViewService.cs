using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.ViewModels.Components;
using ExtendedPomodoro.ViewServices.Interfaces;

namespace ExtendedPomodoro.ViewServices
{
    public class TimerViewService : ITimerViewService
    {
        public AlarmSound AlarmSound
        {
            set => _alarmSoundService.AlarmSound = value;
        }

        public bool IsAlarmRepeatForever
        {
            set => _alarmSoundService.RepeatForeverOrDefault = value;
        }

        public int SoundVolume
        {
            set
            {
                _alarmSoundService.Volume = value;
                _mouseClickSoundService.Volume = value;
            }
        }

        private readonly DialogWindowService _dialogWindowService;
        private readonly AlarmSoundService _alarmSoundService;
        private readonly MouseClickSoundService _mouseClickSoundService;
        private readonly TaskbarIcon _taskbarIcon = new();
        private readonly IMessenger _messenger;
        private Window? _currentSessionFinishDialog;
        private SessionFinishBalloonViewModel? _currentSessionFinishBalloonViewModel;
        private AutoCloseControlViewModel? _currentBalloonViewModel;

        public TimerViewService(
            DialogWindowService dialogWindowService, 
            AlarmSoundService alarmSoundService,
            MouseClickSoundService mouseClickSoundService,
            IMessenger messenger
            )
        {
            _dialogWindowService = dialogWindowService;
            _alarmSoundService = alarmSoundService;
            _mouseClickSoundService = mouseClickSoundService;
            _messenger = messenger;
        }

        public void PlayMouseClickEffectSound() => _mouseClickSoundService.Play();

        public void PlayAlarmSound() => _alarmSoundService.Play();

        public void StopAlarmSound() => _alarmSoundService.Stop();

        public void ShowSessionFinishDialog(TimerSessionState finishedSession,
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
            TimerSessionState nextSession, int autoCloseAfterInMillSeconds)
        {
            CloseAllBalloons();

            var vm = new SessionFinishBalloonViewModel(_messenger)
            {
                NextSession = nextSession,
                FinishedSession = finishedSession,
                AutoCloseAfterInMilliseconds = autoCloseAfterInMillSeconds
            };

            vm.Closed += OnCloseSessionFinishBalloon;

            _currentSessionFinishBalloonViewModel = vm;

            var sessionFinishBalloon = new SessionFinishBalloonTipsUserControl()
            {
                DataContext = vm
            };

            vm.Start();

            _taskbarIcon.ShowCustomBalloon(sessionFinishBalloon,
                System.Windows.Controls.Primitives.PopupAnimation.Slide, 15000);
        }

        public void ShowTimerStartedBalloonTips(TimerSessionState currentSession, 
            int autoCloseAfterInMillSeconds = 5000)
        {
            CloseAllBalloons();

            var vm = new TimerStartedBalloonViewModel()
            {
                AutoCloseAfterInMilliseconds = autoCloseAfterInMillSeconds,
                CurrentSession = currentSession
            };

            _currentBalloonViewModel = vm;

            var timerStartedBalloon = new TimerStartedBalloonTipsUserControl()
            {
                DataContext = vm
            };

            vm.Start();

            _taskbarIcon.ShowCustomBalloon(timerStartedBalloon,
                System.Windows.Controls.Primitives.PopupAnimation.Slide, autoCloseAfterInMillSeconds);
        }

        public void ShowTimerPausedBalloonTips(TimerSessionState currentSession, int autoCloseAfterInMillSeconds = 5000)
        {
            CloseAllBalloons();

            var vm = new TimerPausedBalloonViewModel()
            {
                AutoCloseAfterInMilliseconds = autoCloseAfterInMillSeconds,
                CurrentSession = currentSession
            };

            var timerPausedBalloon = new TimerPausedBalloonTipsUserControl()
            {
                DataContext = vm
            };

            vm.Start();

            _taskbarIcon.ShowCustomBalloon(timerPausedBalloon,
                System.Windows.Controls.Primitives.PopupAnimation.Slide, autoCloseAfterInMillSeconds);
        }

        public void CloseCurrentSessionFinishDialog()
        {
            _currentSessionFinishDialog?.Close();

        }

        public void CloseCurrentSessionFinishBalloon()
        {
            CloseSessionFinishBalloon();
        }

        private void OnCloseSessionFinishBalloon(object? sender, EventArgs? args)
        {
            if (_currentSessionFinishBalloonViewModel != null)
            {
                _currentSessionFinishBalloonViewModel.Closed -= OnCloseSessionFinishBalloon;
            }
        }

        private void OnCloseSessionFinishDialog(object sender, System.Windows.RoutedEventArgs e)
        {
            _messenger.Send(new FinishDialogCloseClickedMessage());

            if (_currentSessionFinishDialog != null)
            {
                _currentSessionFinishDialog.Unloaded -= OnCloseSessionFinishDialog;
                _currentSessionFinishDialog = null;
            }
        }

        private void CloseSessionFinishBalloon()
        {
            if (_currentSessionFinishBalloonViewModel != null)
            {
                _currentSessionFinishBalloonViewModel.CloseRequested = true;
                _currentSessionFinishBalloonViewModel = null;
            }

            _taskbarIcon.CloseBalloon();
        }

        private void CloseAllBalloons()
        {
            if (_currentBalloonViewModel != null)
            {
                _currentBalloonViewModel.CloseRequested = true;
                _currentBalloonViewModel = null;
            }
            _taskbarIcon.CloseBalloon();
        }
    }
}
