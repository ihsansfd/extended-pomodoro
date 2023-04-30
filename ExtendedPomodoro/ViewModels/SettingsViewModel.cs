﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExtendedPomodoro.FrameworkExtensions.Extensions;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Helpers;
using System.Windows;
using ExtendedPomodoro.ViewServices;
using ExtendedPomodoro.Models.Services.Interfaces;

namespace ExtendedPomodoro.ViewModels
{
    public partial class SettingsViewModel : ObservableValidator
    {
        private readonly ISettingsService _repository;
        private readonly MessageBoxService _messageBox;
        private readonly IMessenger _messenger;
        private readonly SettingsViewService _settingsViewService;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Pomodoro duration is required")]
        [Range(0.1, int.MaxValue, ErrorMessage = "Please specify value > 0")]
        private double _pomodoroDurationInMinutes;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Short break duration is required")]
        [Range(0.1, int.MaxValue, ErrorMessage = "Please specify value > 0")]
        private double _shortBreakDurationInMinutes;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Long break duration is required")]
        [Range(0.1, int.MaxValue, ErrorMessage = "Please specify value > 0")]
        private double _longBreakDurationInMinutes;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Long break interval is required")]
        private int _longBreakInterval;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Daily pomodoro target is required")]
        private int _dailyPomodoroTarget;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Is autostart is required")]
        private bool _isAutostart;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Alarm Sound is required")]
        private int _alarmSound;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Volume is required")]
        private int _volume;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Is repeat forever is required")]
        private bool _isRepeatForever;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Is push notification enabled is required")]
        private bool _pushNotificationEnabled;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Is dark mode enabled is required")]
        private bool _darkModeEnabled;

        [ObservableProperty]
        private Hotkey? _startHotkey;

        [ObservableProperty]
        private Hotkey? _pauseHotkey;

        [RelayCommand]
        public async Task ResetToDefaultSettings()
        {
            var confirmationRes = _messageBox.Show("Are you sure to reset the Settings?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmationRes != MessageBoxResult.Yes) return;

            await _repository.ResetToDefaultSettings();
            await Initialize();
        }

        [RelayCommand]
        public void PlayAlarmSoundTester()
        {
            _settingsViewService.PlaySound((AlarmSound)AlarmSound, Volume, TimeSpan.FromSeconds(3));
        }

        public SettingsViewModel(
            ISettingsService repository, 
            MessageBoxService messageBoxService,
            IMessenger messenger,
            SettingsViewService settingsViewService
            )
        {
            _repository = repository;
            _messageBox = messageBoxService;
            _messenger = messenger;
            _settingsViewService = settingsViewService;
        }

        public async Task Initialize()
        {
            SettingsDomain settingsDomain = await _repository.GetSettings();
            PomodoroDurationInMinutes = settingsDomain.PomodoroDuration.TotalSeconds.SecondsToMinutes();
            ShortBreakDurationInMinutes = settingsDomain.ShortBreakDuration.TotalSeconds.SecondsToMinutes();
            LongBreakDurationInMinutes = settingsDomain.LongBreakDuration.TotalSeconds.SecondsToMinutes();
            LongBreakInterval = settingsDomain.LongBreakInterval;
            DailyPomodoroTarget = settingsDomain.DailyPomodoroTarget;
            IsAutostart = settingsDomain.IsAutostart;
            AlarmSound = (int) settingsDomain.AlarmSound;
            Volume = settingsDomain.Volume;
            IsRepeatForever = settingsDomain.IsRepeatForever;
            PushNotificationEnabled = settingsDomain.PushNotificationEnabled;
            DarkModeEnabled = settingsDomain.DarkModeEnabled;
            StartHotkey = settingsDomain.StartHotkeyDomain.ConvertToHotkey();
            PauseHotkey = settingsDomain.PauseHotkeyDomain.ConvertToHotkey();
        }

        [RelayCommand]
        public async Task UpdateSettings()
        {
            ValidateAllProperties();

            if(HasErrors) return;

        await _repository.UpdateSettings(new SettingsDomain()
        {
            PomodoroDuration = TimeSpan.FromMinutes(PomodoroDurationInMinutes),
            ShortBreakDuration = TimeSpan.FromMinutes(ShortBreakDurationInMinutes),
            LongBreakDuration = TimeSpan.FromMinutes(LongBreakDurationInMinutes),
            LongBreakInterval = LongBreakInterval,
            DailyPomodoroTarget = DailyPomodoroTarget,
            IsAutostart = IsAutostart,
            AlarmSound = (AlarmSound) AlarmSound,
            Volume = Volume,
            IsRepeatForever = IsRepeatForever,
            PushNotificationEnabled = PushNotificationEnabled,
            DarkModeEnabled = DarkModeEnabled,
            StartHotkeyDomain = StartHotkey.ConvertToHotkeyDomain(),
            PauseHotkeyDomain = PauseHotkey.ConvertToHotkeyDomain()
        });

            _messenger.Send(new SettingsUpdateInfoMessage(this));
        }

    }
}
