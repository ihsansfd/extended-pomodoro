using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Services.Interfaces;
using System.Text.Json;

namespace ExtendedPomodoro.Models.Services
{
    public class SettingsService : ISettingsService
    {        private readonly ISettingsRepository _repository;

        public SettingsService(ISettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<SettingsDomain> GetSettings()
        {
            var res = await _repository.GetMainSettings();
            return ConvertToSettingsDomain(res);
        }

        public async Task UpdateSettings(SettingsDomain domain)
        {
                SettingsDTO data = ConvertToSettingsDTO(domain);
                await _repository.UpdateSettings(data);
        }

        public async Task ResetToDefaultSettings()
        {
            var data = await _repository.GetDefaultSettings();
            await _repository.UpdateSettings(data);
        }

        private static SettingsDTO ConvertToSettingsDTO(SettingsDomain domain)
        {
            return new()
            {
                PomodoroDurationInSeconds = (int)domain.PomodoroDuration.TotalSeconds,
                ShortBreakDurationInSeconds = (int)domain.ShortBreakDuration.TotalSeconds,
                LongBreakDurationInSeconds = (int)domain.LongBreakDuration.TotalSeconds,
                LongBreakInterval = domain.LongBreakInterval,
                DailyPomodoroTarget = domain.DailyPomodoroTarget,
                IsAutostart = domain.IsAutostart,
                AlarmSound = (int)domain.AlarmSound,
                Volume = domain.Volume,
                IsRepeatForever = domain.IsRepeatForever,
                PushNotificationEnabled = domain.PushNotificationEnabled,
                DarkModeEnabled = domain.DarkModeEnabled,
                StartHotkey = domain.StartHotkeyDomain != null ? JsonSerializer.Serialize(domain.StartHotkeyDomain) : null,
                PauseHotkey = domain.PauseHotkeyDomain != null ? JsonSerializer.Serialize(domain.PauseHotkeyDomain) : null,
            };
        }

        private static SettingsDomain ConvertToSettingsDomain(SettingsDTO dto)
        {
            return new SettingsDomain()
            {
                PomodoroDuration = TimeSpan.FromSeconds(dto.PomodoroDurationInSeconds),
                ShortBreakDuration = TimeSpan.FromSeconds(dto.ShortBreakDurationInSeconds),
                LongBreakDuration = TimeSpan.FromSeconds(dto.LongBreakDurationInSeconds),
                LongBreakInterval = dto.LongBreakInterval,
                DailyPomodoroTarget = dto.DailyPomodoroTarget,
                IsAutostart = dto.IsAutostart,
                AlarmSound = (AlarmSound)dto.AlarmSound,
                Volume = dto.Volume,
                IsRepeatForever = dto.IsRepeatForever,
                PushNotificationEnabled = dto.PushNotificationEnabled,
                DarkModeEnabled = dto.DarkModeEnabled,
                StartHotkeyDomain = dto.StartHotkey != null ? 
                                    JsonSerializer.Deserialize<HotkeyDomain>(dto.StartHotkey) : null,
                PauseHotkeyDomain = dto.PauseHotkey != null ? 
                                    JsonSerializer.Deserialize<HotkeyDomain>(dto.PauseHotkey) : null
            };
        }

    }
}
