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

namespace ExtendedPomodoro.Models.Repositories.Sqlite
{
    public class SqliteSettingsRepository : ISettingsRepository
    {
        private readonly SqliteDbConnectionFactory _connectionFactory;

        private const string SELECT_MAIN_SETTINGS =
            @"SELECT * FROM tblSettings WHERE SettingsType = 'MAIN' LIMIT 1";

        public SqliteSettingsRepository(SqliteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<SettingsDomain> GetSettings()
        {
            using (var db = _connectionFactory.Connect())
            {

                var record = await db.QuerySingleAsync<SqlSettingsDTO>(SELECT_MAIN_SETTINGS);

                return ConvertToSettingsDomain(record);
            }
        }

        private static SettingsDomain ConvertToSettingsDomain(SqlSettingsDTO dto)
        {
            return new SettingsDomain(
                dto.SettingsType == "MAIN" ? SettingsType.MAIN : SettingsType.DEFAULT,
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

        //private static Convert

        //public async Task ResetToDefaultSettings();

        //public async Task ChangeSettingsFor()

    }
}
