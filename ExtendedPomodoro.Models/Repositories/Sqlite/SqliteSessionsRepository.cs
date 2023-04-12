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
                SessionDate = domain.SessionDate,
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


    //public class SqliteSessionsRepository
    //{
    //    private readonly SqliteDbConnectionFactory _connectionFactory;

    //    private const string CREATE_DAILY_SESSION_QUERY =
    //       @"INSERT OR IGNORE INTO tblDailySessions (SessionDate, TimeSpentInSeconds, TotalPomodoroCompleted, 
    //        TotalShortBreaksCompleted, TotalLongBreaksCompleted, CreatedAt, UpdatedAt)
    //        VALUES (@SessionDate, @TimeSpentInSeconds, @TotalPomodoroCompleted, 
    //        @TotalShortBreaksCompleted, @TotalLongBreaksCompleted, @CreatedAt, @UpdatedAt)";

    //    private const string UPDATE_DAILY_SESSION_QUERY =
    //      @"UPDATE tblDailySessions SET 
    //            TimeSpentInSeconds = TimeSpentInSeconds + @IncrementTimeSpentInSecondsBy, 
    //            TotalPomodoroCompleted = TotalPomodoroCompleted + @IncrementTotalPomodoroCompletedBy,
    //            TotalShortBreaksCompleted = TotalShortBreaksCompleted + @IncrementTotalShortBreaksCompletedBy,
    //            TotalLongBreaksCompleted = TotalLongBreaksCompleted + IncrementTotalLongBreaksCompletedBy,
    //            UpdatedAt = @UpdatedAt WHERE SessionDate = @SessionDate";

    //    private const string CREATE_DAILY_SESSION_TASK_LINK_QUERY =
    //      @"INSERT tblDailySessions SET 
    //            TimeSpentInSeconds = TimeSpentInSeconds + @IncrementTimeSpentInSecondsBy, 
    //            TotalPomodoroCompleted = TotalPomodoroCompleted + @IncrementTotalPomodoroCompletedBy,
    //            TotalShortBreaksCompleted = TotalShortBreaksCompleted + @IncrementTotalShortBreaksCompletedBy,
    //            TotalLongBreaksCompleted = TotalLongBreaksCompleted + IncrementTotalLongBreaksCompletedBy,
    //            UpdatedAt = @UpdatedAt WHERE SessionDate = @SessionDate";


    //    public SqliteSessionsRepository(SqliteDbConnectionFactory connectionFactory)
    //    {
    //        _connectionFactory = connectionFactory;
    //    }

    //    // TODO: REFACTORING TO CREATEORINCREMENT https://stackoverflow.com/a/50718957/11934785
    //    public async Task CreateOrUpdateDailySession(CreateOrUpdateDailySessionDomain domain)
    //    {
    //        using (var db = _connectionFactory.Connect())
    //        {
    //            bool successfullyCreated = await TryCreateDailySession(domain, db);

    //            if (successfullyCreated) return;

    //            await UpdateDailySession(domain, db);
    //        }
    //    }

    //    public async Task CreateOrUpdateDailySessionTaskLink(DailySessionTaskLinkDomain domain)
    //    {
    //        using (var db = _connectionFactory.Connect())
    //        {
    //            bool successfullyCreated = await TryCreateDailySession(domain, db);

    //            if (successfullyCreated) return;

    //            await UpdateDailySession(domain, db);
    //        }
    //    }

    //    private async Task<bool> TryCreateDailySession(CreateOrUpdateDailySessionDomain domain, IDbConnection db)
    //    {
    //        var data = ConvertToCreateSqliteDailySessionDTO(domain);

    //        int rowsAffected = await db.ExecuteAsync(CREATE_DAILY_SESSION_QUERY, data);

    //        return rowsAffected > 0;
    //    }

    //    private async Task UpdateDailySession(CreateOrUpdateDailySessionDomain domain, IDbConnection db)
    //    {
    //        var data = ConvertToUpdateSqliteDailySessionDTO(domain);
    //        await db.ExecuteAsync(UPDATE_DAILY_SESSION_QUERY, data);
    //    }

    //    private static CreateSqliteDailySessionDTO ConvertToCreateSqliteDailySessionDTO(CreateOrUpdateDailySessionDomain domain)
    //    {
    //        var now = DateTime.Now;

    //        return new CreateSqliteDailySessionDTO()
    //        {
    //            SessionDate = domain.SessionDate,
    //            TimeSpentInSeconds = (int)domain.TimeSpent.TotalSeconds,
    //            TotalPomodoroCompleted = domain.TotalPomodoroCompleted,
    //            TotalShortBreaksCompleted = domain.TotalShortBreaksCompleted,
    //            TotalLongBreaksCompleted = domain.TotalLongBreaksCompleted,
    //            CreatedAt = now,
    //            UpdatedAt = now
    //        };
    //    }

    //    private static UpdateSqliteDailySessionDTO ConvertToUpdateSqliteDailySessionDTO(CreateOrUpdateDailySessionDomain domain)
    //    {
    //        var now = DateTime.Now;

    //        return new UpdateSqliteDailySessionDTO()
    //        {
    //            SessionDate = domain.SessionDate,
    //            IncrementTimeSpentInSecondsBy = (int)domain.TimeSpent.TotalSeconds,
    //            IncrementTotalPomodoroCompletedBy = domain.TotalPomodoroCompleted,
    //            IncrementTotalShortBreaksCompletedBy = domain.TotalShortBreaksCompleted,
    //            IncrementTotalLongBreaksCompletedBy = domain.TotalLongBreaksCompleted,
    //            UpdatedAt = now
    //        };
    //    }
    //}
}
