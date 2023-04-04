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

        private const string CREATE_TASKS_TABLE = @"CREATE TABLE IF NOT EXISTS tblTasks (
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
            connection.Execute(CREATE_TASKS_TABLE);
        }
    }
}
