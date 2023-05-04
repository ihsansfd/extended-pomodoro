using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Helpers;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Proxies
{

    public class AppSettingsProvider : IAppSettingsProvider
    {
        public AppSettings AppSettings { get; set; }

        private readonly ISettingsService _settingsService;
        private readonly IMessenger _messenger;

        public AppSettingsProvider(ISettingsService settingsService, IMessenger messenger)
        {
            _settingsService = settingsService;
            _messenger = messenger;
        }

        public async Task Initialize()
        {
            await LoadSettingsFromDB();
        }

        public async Task<AppSettings> LoadSettings()
        {
            await LoadSettingsFromDB();
            _messenger.Send(new SettingsUpdateInfoMessage(AppSettings));

            return AppSettings;
        }

        private async Task LoadSettingsFromDB()
        {
            SettingsDomain settingsDomain = await _settingsService.GetSettings();
            AppSettings = ConvertToAppSettings(settingsDomain);
        }

        private static AppSettings ConvertToAppSettings(SettingsDomain settingsDomain)
        {
            return new()
            {
                PomodoroDuration = settingsDomain.PomodoroDuration,
                ShortBreakDuration = settingsDomain.ShortBreakDuration,
                LongBreakDuration = settingsDomain.LongBreakDuration,
                LongBreakInterval = settingsDomain.LongBreakInterval,
                DailyPomodoroTarget = settingsDomain.DailyPomodoroTarget,
                IsAutostart = settingsDomain.IsAutostart,
                AlarmSound = settingsDomain.AlarmSound,
                Volume = settingsDomain.Volume,
                IsRepeatForever = settingsDomain.IsRepeatForever,
                PushNotificationEnabled = settingsDomain.PushNotificationEnabled,
                DarkModeEnabled = settingsDomain.DarkModeEnabled,
                StartHotkey = settingsDomain.StartHotkeyDomain.ConvertToHotkey(),
                PauseHotkey = settingsDomain.PauseHotkeyDomain.ConvertToHotkey()
            };
        }
    }
}
