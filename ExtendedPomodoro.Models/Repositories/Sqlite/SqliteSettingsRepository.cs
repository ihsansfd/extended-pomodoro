using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExtendedPomodoro.Models.Repositories.Sqlite
{
    public class SqliteSettingsRepository : ISettingsRepository
    {
        private readonly SqliteDbConnectionFactory _connectionFactory;

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

        public SqliteSettingsRepository(SqliteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<SettingsDomain> GetSettings()
        {
            using (var db = _connectionFactory.Connect())
            {
                var record = await db.QuerySingleAsync<SqliteSettingsDTO>(SELECT_MAIN_SETTINGS_QUERY);

                return ConvertToSettingsDomain(record);
            }
        }

        public async Task UpdateSettings(SettingsDomain domain)
        {
            using (var db = _connectionFactory.Connect())
            {
                SqliteSettingsDTO data = new()
                {
                    PomodoroDuration = (int) domain.PomodoroDuration.TotalSeconds,
                    ShortBreakDuration = (int) domain.ShortBreakDuration.TotalSeconds,
                    LongBreakDuration = (int) domain.LongBreakDuration.TotalSeconds,
                    LongBreakInterval = domain.LongBreakInterval,
                    DailyPomodoroTarget = domain.DailyPomodoroTarget,
                    IsAutostart = domain.IsAutostart,
                    AlarmSound = (int) domain.AlarmSound,
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
                var data = await db.QuerySingleAsync<SqliteSettingsDTO>(SELECT_DEFAULT_SETTINGS_QUERY);

                await db.ExecuteAsync(UPDATE_MAIN_SETTINGS_QUERY, data);
            }
        }

        private static SettingsDomain ConvertToSettingsDomain(SqliteSettingsDTO dto)
        {
            return new SettingsDomain(
                TimeSpan.FromSeconds(dto.PomodoroDuration),
                TimeSpan.FromSeconds(dto.ShortBreakDuration),
                TimeSpan.FromSeconds(dto.LongBreakDuration),
                dto.LongBreakInterval,
                dto.DailyPomodoroTarget,
                dto.IsAutostart,
                (AlarmSound) dto.AlarmSound,
                dto.Volume,
                dto.IsRepeatForever,
                dto.PushNotificationEnabled,
                dto.DarkModeEnabled,
                dto.StartHotkey != null ? JsonSerializer.Deserialize<HotkeyDomain>(dto.StartHotkey) : null,
                dto.PauseHotkey != null ? JsonSerializer.Deserialize<HotkeyDomain>(dto.PauseHotkey) : null
                );
        }

        //public async Task ResetToDefaultSettings();



    }
}
