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
                var record = await db.QuerySingleAsync<SqlSettingsDTO>(SELECT_MAIN_SETTINGS_QUERY);

                return ConvertToSettingsDomain(record);
            }
        }

        public async Task UpdateSettings(SettingsDomain settingsDomain)
        {
            using (var db = _connectionFactory.Connect())
            {
                SqlSettingsDTO data = new()
                {
                    PomodoroDuration = (int) settingsDomain.PomodoroDuration.TotalSeconds,
                    ShortBreakDuration = (int) settingsDomain.ShortBreakDuration.TotalSeconds,
                    LongBreakDuration = (int) settingsDomain.LongBreakDuration.TotalSeconds,
                    LongBreakInterval = settingsDomain.LongBreakInterval,
                    DailyPomodoroTarget = settingsDomain.DailyPomodoroTarget,
                    IsAutostart = settingsDomain.IsAutostart,
                    AlarmSound = (int) settingsDomain.AlarmSound,
                    Volume = settingsDomain.Volume,
                    IsRepeatForever = settingsDomain.IsRepeatForever,
                    PushNotificationEnabled = settingsDomain.PushNotificationEnabled,
                    DarkModeEnabled = settingsDomain.DarkModeEnabled,
                    StartHotkey = settingsDomain.StartHotkey != null ? JsonSerializer.Serialize(settingsDomain.StartHotkey) : null,
                    PauseHotkey = settingsDomain.PauseHotkey != null ? JsonSerializer.Serialize(settingsDomain.PauseHotkey) : null,
                };

                await db.ExecuteAsync(UPDATE_MAIN_SETTINGS_QUERY, data);
            }
        }

        public async Task ResetToDefaultSettings()
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = await db.QuerySingleAsync<SqlSettingsDTO>(SELECT_DEFAULT_SETTINGS_QUERY);

                await db.ExecuteAsync(UPDATE_MAIN_SETTINGS_QUERY, data);
            }
        }

        private static SettingsDomain ConvertToSettingsDomain(SqlSettingsDTO dto)
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
