using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories.Sqlite
{
    public class SqliteSessionsRepository : ISessionsRepository
    {
        private readonly SqliteDbConnectionFactory _connectionFactory;

        private const string UPSERT_DAILY_SESSION_QUERY =
            @"
              INSERT INTO tblDailySessions 
                (SessionDate, TimeSpentInSeconds, TotalPomodoroCompleted, 
                TotalShortBreaksCompleted, TotalLongBreaksCompleted, CreatedAt, UpdatedAt)
              VALUES 
                (@SessionDate, @TimeSpentInSeconds, @TotalPomodoroCompleted, 
                @TotalShortBreaksCompleted, @TotalLongBreaksCompleted, @CreatedAt, @UpdatedAt)
              ON CONFLICT(SessionDate) 
              DO UPDATE SET
                TimeSpentInSeconds = TimeSpentInSeconds + @TimeSpentInSeconds, 
                TotalPomodoroCompleted = TotalPomodoroCompleted + @TotalPomodoroCompleted,
                TotalShortBreaksCompleted = TotalShortBreaksCompleted + @TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = TotalLongBreaksCompleted + @TotalLongBreaksCompleted,
                UpdatedAt = @UpdatedAt 
              WHERE SessionDate = @SessionDate;";

        private const string UPSERT_DAILY_SESSION_TIME_SPENT_QUERY =
            @"
              INSERT INTO tblDailySessions 
                (SessionDate, TimeSpentInSeconds, CreatedAt, UpdatedAt)
              VALUES 
                (@SessionDate, @TimeSpentInSeconds, @CreatedAt, @UpdatedAt)
              ON CONFLICT(SessionDate) 
              DO UPDATE SET
                TimeSpentInSeconds = TimeSpentInSeconds + @TimeSpentInSeconds, 
                UpdatedAt = @UpdatedAt 
              WHERE SessionDate = @SessionDate;";

        private const string UPSERT_DAILY_SESSION_TASK_LINK_QUERY =
          @"
            INSERT INTO tblDailySessionTaskLinks (SessionDate, TaskId, IsTaskCompletedInThisSession)
            VALUES (@SessionDate, @TaskId, @IsTaskCompletedInThisSession) 
            ON CONFLICT(SessionDate, TaskId) 
            DO UPDATE SET IsTaskCompletedInThisSession = @IsTaskCompletedInThisSession;";


        public SqliteSessionsRepository(SqliteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task UpsertDailySession(UpsertDailySessionDomain domain)
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = ConvertToUpsertSqliteDailySessionDTO(domain);
                await db.ExecuteAsync(UPSERT_DAILY_SESSION_QUERY, data);
            }
        }

        public async Task UpsertDailySessionTimeSpent(DateOnly sessionDate, TimeSpan timeSpent)
        {
            var now = DateTime.Now;

            using (var db = _connectionFactory.Connect())
            {
                var data = new
                {
                    SessionDate = sessionDate.ToString(),
                    TimeSpentInSeconds = timeSpent.TotalSeconds,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await db.ExecuteAsync(UPSERT_DAILY_SESSION_TIME_SPENT_QUERY, data);
            }
        }

        public async Task UpsertDailySessionTaskLink(DailySessionTaskLinkDomain domain)
        {
            using (var db = _connectionFactory.Connect())
            {
                await db.ExecuteAsync(UPSERT_DAILY_SESSION_TASK_LINK_QUERY, domain);
            }
        }

        private static SqliteUpsertDailySessionDTO ConvertToUpsertSqliteDailySessionDTO(UpsertDailySessionDomain domain)
        {
            var now = DateTime.Now;

            return new SqliteUpsertDailySessionDTO()
            {
                SessionDate = domain.SessionDate.ToString(),
                TimeSpentInSeconds = (int)domain.TimeSpent.TotalSeconds,
                TotalPomodoroCompleted = domain.TotalPomodoroCompleted,
                TotalShortBreaksCompleted = domain.TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = domain.TotalLongBreaksCompleted,
                CreatedAt = now,
                UpdatedAt = now
            };
        }
     }
}