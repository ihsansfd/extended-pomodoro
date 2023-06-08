using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ExtendedPomodoro.Helpers;
using System.Windows;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Services.Interfaces;
using ExtendedPomodoro.ViewServices.Interfaces;
using ExtendedPomodoro.Core.Timeout;

namespace ExtendedPomodoro.ViewModels
{
    public partial class SettingsViewModel : ObservableValidator
    {
        private readonly ISettingsService _settingsService;
        private readonly IMessageBoxService _messageBox;
        private readonly ISettingsViewService _settingsViewService;
        private readonly IAppSettingsProvider _appSettingsProvider;

        private int _saveChangesWaitTimeoutCount = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        [Range(0.1, int.MaxValue, ErrorMessage = "Please specify value > 0")]
        private double _pomodoroDurationInMinutes;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        [Range(0.1, int.MaxValue, ErrorMessage = "Please specify value > 0")]
        private double _shortBreakDurationInMinutes;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        [Range(0.1, int.MaxValue, ErrorMessage = "Please specify value > 0")]
        private double _longBreakDurationInMinutes;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private int _longBreakInterval;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private int _dailyPomodoroTarget;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private bool _isAutostart;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private int _alarmSound;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private int _volume;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private bool _isRepeatForever;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private bool _pushNotificationEnabled;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Value cannot be empty", AllowEmptyStrings = false)]
        private bool _darkModeEnabled;

        [ObservableProperty]
        private Hotkey? _startHotkey;

        [ObservableProperty]
        private Hotkey? _pauseHotkey;

        [ObservableProperty] 
        private bool _isSuccessChangingNotificationOpen;

        public SettingsViewModel(
            ISettingsService settingsService,
            IMessageBoxService messageBoxService,
            ISettingsViewService settingsViewService,
            IAppSettingsProvider appSettingsProvider
        )
        {
            _settingsService = settingsService;
            _messageBox = messageBoxService;
            _settingsViewService = settingsViewService;
            _appSettingsProvider = appSettingsProvider;
        }

        [RelayCommand]
        private async Task ResetToDefaultSettings()
        {
            var confirmationRes = _messageBox.Show(
                "Are you sure to reset the Settings?", 
                "Confirmation",
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (confirmationRes != MessageBoxResult.Yes) return;

            await _settingsService.ResetToDefaultSettings();
            await Load();
        }

        [RelayCommand]
        private void PlayAlarmSoundTester()
        {
            _settingsViewService.PlaySound((AlarmSound)AlarmSound, Volume, TimeSpan.FromSeconds(3));
        }

        [RelayCommand]
        private async Task Load()
        {
            SettingsDomain settingsDomain = await _settingsService.GetSettings();
            PomodoroDurationInMinutes = settingsDomain.PomodoroDuration.TotalMinutes;
            ShortBreakDurationInMinutes = settingsDomain.ShortBreakDuration.TotalMinutes;
            LongBreakDurationInMinutes = settingsDomain.LongBreakDuration.TotalMinutes;
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
        private async Task UpdateSettings()
        {
            ValidateAllProperties();

            if(HasErrors) return;

            try
            {
                await _settingsService.UpdateSettings(new SettingsDomain()
                {
                    PomodoroDuration = TimeSpan.FromMinutes(PomodoroDurationInMinutes),
                    ShortBreakDuration = TimeSpan.FromMinutes(ShortBreakDurationInMinutes),
                    LongBreakDuration = TimeSpan.FromMinutes(LongBreakDurationInMinutes),
                    LongBreakInterval = LongBreakInterval,
                    DailyPomodoroTarget = DailyPomodoroTarget,
                    IsAutostart = IsAutostart,
                    AlarmSound = (AlarmSound)AlarmSound,
                    Volume = Volume,
                    IsRepeatForever = IsRepeatForever,
                    PushNotificationEnabled = PushNotificationEnabled,
                    DarkModeEnabled = DarkModeEnabled,
                    StartHotkeyDomain = StartHotkey.ConvertToHotkeyDomain(),
                    PauseHotkeyDomain = PauseHotkey.ConvertToHotkeyDomain()
                });

                OpenSuccessChangingNotification();
            }
            catch (Exception ex)
            {
                // ignored
            }

            await _appSettingsProvider.LoadSettings();
        }

        private void OpenSuccessChangingNotification()
        {
            IsSuccessChangingNotificationOpen = true;

            _saveChangesWaitTimeoutCount++;

            WaitTimeoutProvider.RegisterWaitTimeout(() =>
            {
                _saveChangesWaitTimeoutCount--;
                if (_saveChangesWaitTimeoutCount <= 0)
                {
                    IsSuccessChangingNotificationOpen = false;
                }

            }, TimeSpan.FromSeconds(3));
        }

    }
}
