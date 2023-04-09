using CommunityToolkit.Mvvm.ComponentModel;
using ExtendedPomodoro.Core.Extensions;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtendedPomodoro.ViewModels
{
    public partial class SettingsViewModel : ObservableValidator
    {
        private ISettingsRepository _repository;

        [ObservableProperty]
        private double _pomodoroDurationInMinutes;

        [ObservableProperty]
        private double _shortBreakDurationInMinutes;

        [ObservableProperty]
        private double _longBreakDurationInMinutes;

        [ObservableProperty]
        private int _longBreakInterval;

        [ObservableProperty]
        private int _dailyPomodoroTarget;

        [ObservableProperty]
        private bool _isAutoStart;

        [ObservableProperty]
        private int _alarmSound;

        [ObservableProperty]
        private int _volume;

        [ObservableProperty]
        private bool _isRepeatForever;

        [ObservableProperty]
        private bool _pushNotificationEnabled;

        [ObservableProperty]
        private bool _darkModeEnabled;

        [ObservableProperty]
        private Hotkey? _startHotkey;

        [ObservableProperty]
        private Hotkey? _pauseHotkey;

        public SettingsViewModel(ISettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task LoadSettings()
        {
            SettingsDomain settingsDomain = await _repository.GetSettings();
            PomodoroDurationInMinutes = settingsDomain.PomodoroDuration.TotalSeconds.SecondsToMinutes();
            ShortBreakDurationInMinutes = settingsDomain.ShortBreakDuration.TotalSeconds.SecondsToMinutes();
            LongBreakDurationInMinutes = settingsDomain.LongBreakDuration.TotalSeconds.SecondsToMinutes();
            LongBreakInterval = settingsDomain.LongBreakInterval;
            DailyPomodoroTarget = settingsDomain.DailyPomodoroTarget;
            IsAutoStart = settingsDomain.IsAutostart;
            AlarmSound = (int) settingsDomain.AlarmSound;
            Volume = settingsDomain.Volume;
            IsRepeatForever = settingsDomain.IsRepeatForever;
            PushNotificationEnabled = settingsDomain.PushNotificationEnabled;
            DarkModeEnabled = settingsDomain.DarkModeEnabled;
            StartHotkey = ConvertToHotkey(settingsDomain.StartHotkey);
            PauseHotkey = ConvertToHotkey(settingsDomain.PauseHotkey);
        }

        private Hotkey? ConvertToHotkey(HotkeyDomain? hotkeyDomain)
        {
            if(hotkeyDomain == null) return null;

            return new Hotkey((Key) hotkeyDomain.Key, (ModifierKeys) hotkeyDomain.ModifierKeys);
        }
    }
}
