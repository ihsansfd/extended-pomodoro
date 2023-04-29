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

        private static SettingsDTO ConvertToSettingsDTO(SettingsDomain domain)
        {
            return new()
            {
                PomodoroDuration = (int)domain.PomodoroDuration.TotalSeconds,
                ShortBreakDuration = (int)domain.ShortBreakDuration.TotalSeconds,
                LongBreakDuration = (int)domain.LongBreakDuration.TotalSeconds,
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

        public async Task ResetToDefaultSettings()
        {
            var data = await _repository.GetDefaultSettings();
            await _repository.UpdateSettings(data);
        }

        private static SettingsDomain ConvertToSettingsDomain(SettingsDTO dto)
        {
            return new SettingsDomain(
                TimeSpan.FromSeconds(dto.PomodoroDuration),
                TimeSpan.FromSeconds(dto.ShortBreakDuration),
                TimeSpan.FromSeconds(dto.LongBreakDuration),
                dto.LongBreakInterval,
                dto.DailyPomodoroTarget,
                dto.IsAutostart,
                (AlarmSound)dto.AlarmSound,
                dto.Volume,
                dto.IsRepeatForever,
                dto.PushNotificationEnabled,
                dto.DarkModeEnabled,
                dto.StartHotkey != null ? JsonSerializer.Deserialize<HotkeyDomain>(dto.StartHotkey) : null,
                dto.PauseHotkey != null ? JsonSerializer.Deserialize<HotkeyDomain>(dto.PauseHotkey) : null
                );
        }

    }
}
