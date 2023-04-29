using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories.Sqlite
{
    public class SqliteSettingsRepository : ISettingsRepository
    {
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

        private readonly IDbConnectionFactory _connectionFactory;

        public SqliteSettingsRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<SettingsDTO> GetMainSettings()
        {
            using (var db = _connectionFactory.Connect())
            {
                return await db.QuerySingleAsync<SettingsDTO>(SELECT_MAIN_SETTINGS_QUERY);
            }
        }

        public async Task<SettingsDTO> GetDefaultSettings()
        {
            using (var db = _connectionFactory.Connect())
            {
                return await db.QuerySingleAsync<SettingsDTO>(SELECT_DEFAULT_SETTINGS_QUERY);
            }
        }

        public async Task UpdateSettings(SettingsDTO dto)
        {
            using (var db = _connectionFactory.Connect())
            {
                await db.ExecuteAsync(UPDATE_MAIN_SETTINGS_QUERY, dto);
            }
        }

    }
}
