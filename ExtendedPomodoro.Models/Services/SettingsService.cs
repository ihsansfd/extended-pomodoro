using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Services.Interfaces;
using System.Text.Json;

namespace ExtendedPomodoro.Models.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private const string SELECT_DEFAULT_SETTINGS_QUERY =
            @"SELECT * FROM tblSettings WHERE SettingsType = 'DEFAULT' LIMIT 1";

        private const string SELECT_MAIN_SETTINGS_QUERY =
            @"SELECT * FROM tblSettings WHERE SettingsType = 'MAIN' LIMIT 1";

        private const string UPDATE_MAIN_SETTINGS_QUERY =
            @"UPDATE tblSettings SET PomodoroDuration = @PomodoroDuration, ShortBreakDuration = @ShortBreakDuration,
            LongBreakDuration = @LongBreakDuration, LongBreakInterval = @LongBreakInterval, 
            DailyPomodoroTarget = @DailyPomodoroTarget, IsAutostart = @IsAutostart, AlarmSound = @AlarmSound,
            Volume = @Volume, IsRepeatForever = @IsRepeatForever, PushNotificationEnabled = @PushNotificationEnabled,
            DarkModeEnabled = @DarkModeEnabled, StartHotkey = @StartHotkey, PauseHotkey = @PauseHotkey
            WHERE SettingsType = 'MAIN'";

        public SettingsService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<SettingsDomain> GetSettings()
        {
            using (var db = _connectionFactory.Connect())
            {
                var record = await db.QuerySingleAsync<SettingsDTO>(SELECT_MAIN_SETTINGS_QUERY);

                return ConvertToSettingsDomain(record);
            }
        }

        public async Task UpdateSettings(SettingsDomain domain)
        {
            using (var db = _connectionFactory.Connect())
            {
                SettingsDTO data = new()
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

                await db.ExecuteAsync(UPDATE_MAIN_SETTINGS_QUERY, data);
            }
        }

        public async Task ResetToDefaultSettings()
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = await db.QuerySingleAsync<SettingsDTO>(SELECT_DEFAULT_SETTINGS_QUERY);

                await db.ExecuteAsync(UPDATE_MAIN_SETTINGS_QUERY, data);
            }
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
