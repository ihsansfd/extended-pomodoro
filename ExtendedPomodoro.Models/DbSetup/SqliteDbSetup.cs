using Dapper;
using ExtendedPomodoro.Models.DbConnections;

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
                TimeSpentInSeconds INT DEFAULT 0 NOT NULL,
                CreatedAt TEXT,
                UpdatedAt TEXT
            );";

        private const string CREATE_SETTINGS_TABLE_QUERY =
            @"
            CREATE TABLE IF NOT EXISTS tblSettings (
                SettingsType VARCHAR(20) UNIQUE NOT NULL,
                PomodoroDurationInSeconds INT NOT NULL,
                ShortBreakDurationInSeconds INT NOT NULL,
                LongBreakDurationInSeconds INT NOT NULL,
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
            );";

        private const string INSERT_DEFAULT_SETTINGS_QUERY =
            @"
            INSERT OR IGNORE INTO tblSettings VALUES ('MAIN',1500,300,900,4,10,0,0,50,0,1,0,
            '{""ModifierKeys"":5, ""Key"":62}', '{""ModifierKeys"":5, ""Key"":59}');

             INSERT OR IGNORE INTO tblSettings VALUES ('DEFAULT',1500,300,900,4,10,0,0,50,0,1,0,
            '{""ModifierKeys"":5, ""Key"":62}', '{""ModifierKeys"":5, ""Key"":59}'
            );";

        private const string CREATE_DAILY_SESSIONS_TABLE_QUERY = 
            @"CREATE TABLE IF NOT EXISTS tblDailySessions (
                SessionDate TEXT PRIMARY KEY NOT NULL,
                TimeSpentInSeconds INT DEFAULT 0 NOT NULL,
                TotalPomodoroCompleted INT DEFAULT 0 NOT NULL,
                TotalShortBreaksCompleted INT DEFAULT 0 NOT NULL,
                TotalLongBreaksCompleted INT DEFAULT 0 NOT NULL,
                TotalTasksCompleted INT DEFAULT 0 NOT NULL,
                CreatedAt TEXT,
                UpdatedAt TEXT
            );";

        public SqliteDbSetup(SqliteDbConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public async Task Setup()
        {
            var query = CREATE_TASKS_TABLE_QUERY +
                        CREATE_SETTINGS_TABLE_QUERY +
                        INSERT_DEFAULT_SETTINGS_QUERY +
                        CREATE_DAILY_SESSIONS_TABLE_QUERY;

            using var connection = _connectionFactory.Connect();
            await connection.ExecuteAsync(query);
        }
    }
}
