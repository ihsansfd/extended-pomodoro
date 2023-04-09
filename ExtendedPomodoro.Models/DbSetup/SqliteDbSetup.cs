using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DbSetup
{
    public class SqliteDbSetup : IDatabaseSetup
    {
        private readonly SqliteDbConnectionFactory _connectionFactory;

        private const string CREATE_TASKS_TABLE_QUERY = @"CREATE TABLE IF NOT EXISTS tblTasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                Name VARCHAR(255) NOT NULL,
                Description TEXT,
                EstPomodoro INT,
                ActPomodoro INT DEFAULT 0 NOT NULL,
                IsTaskCompleted INT(1) DEFAULT 0 NOT NULL,
                TimeSpent INT DEFAULT 0 NOT NULL,
                CreatedAt TEXT,
                UpdatedAt TEXT
            )";

        private const string DEFAULT_SETTINGS_CONFIGURATION_QUERY =
            @"
            CREATE TABLE IF NOT EXISTS tblSettings (
                SettingsType VARCHAR(20) UNIQUE NOT NULL,
                PomodoroDuration INT NOT NULL,
                ShortBreakDuration INT NOT NULL,
                LongBreakDuration INT NOT NULL,
                LongBreakInterval INT NOT NULL,
                DailyPomodoroTarget INT NOT NULL,
                IsAutostart INT(1) NOT NULL,
                AlarmSound INT(10) NOT NULL,
                Volume INT(100) NOT NULL,
                IsRepeatForever INT(1) NOT NULL,
                PushNotificationEnabled INT(1) NOT NULL,
                DarkModeEnabled INT(1) NOT NULL,
                StartHotkey VARCHAR(50),
                PauseHotkey VARCHAR(50)
            );

            INSERT OR IGNORE INTO tblSettings VALUES ('MAIN',1500,300,900,4,10,0,0,50,0,1,0,
            '{""ModifierKeys"":5, ""Key"":62}', '{""ModifierKeys"":5, ""Key"":59}');

             INSERT OR IGNORE INTO tblSettings VALUES ('DEFAULT',1500,300,900,4,10,0,0,50,0,1,0,
            '{""ModifierKeys"":5, ""Key"":62}', '{""ModifierKeys"":5, ""Key"":59}');
            ";

        //private const string CREATE_SESSIONS_TABLE = @"CREATE TABLE IF NOT EXISTS tblTasks (
        //        Id TEXT PRIMARY KEY, 
        //        TimeSpentInSeconds INT,
        //        CreatedAt TEXT,
        //        UpdatedAt TEXT
        //    )";

        //private const string CREATE_POMODORO_TABLE = @"CREATE TABLE IF NOT EXISTS tblTasks (
        //        Id TEXT PRIMARY KEY,
        //        Duration INT
        //        TaskId INT
        //        CreatedAt TEXT
        //        FOREIGN KEY (TaskId)
        //            REFERENCES tblTasks (Id) 
        //    )";

        //private const string CREATE_SHORTBREAKS_TABLE = @"CREATE TABLE IF NOT EXISTS tblTasks (
        //        Id TEXT PRIMARY KEY,
        //        Duration INT
        //        CreatedAt TEXT
        //    )";

        //private const string CREATE_LONGBREAKS_TABLE = @"CREATE TABLE IF NOT EXISTS tblTasks (
        //        Id TEXT PRIMARY KEY,
        //        Duration INT
        //        CreatedAt TEXT
        //    )";

        public SqliteDbSetup(SqliteDbConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public void Setup()
        {
            using var connection = _connectionFactory.Connect();
            connection.Execute(CREATE_TASKS_TABLE_QUERY);
            connection.Execute(DEFAULT_SETTINGS_CONFIGURATION_QUERY);
        }
    }
}
