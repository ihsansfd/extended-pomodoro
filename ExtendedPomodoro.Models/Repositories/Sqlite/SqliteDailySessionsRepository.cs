using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories.Sqlite;

public class SqliteDailySessionsRepository : IDailySessionsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    private const string UPSERT_DAILY_SESSION_QUERY =
        @"
              INSERT INTO tblDailySessions 
                (SessionDate, 
                TimeSpentInSeconds, 
                TotalPomodoroCompleted, 
                TotalShortBreaksCompleted, 
                TotalLongBreaksCompleted, 
                DailyPomodoroTarget,
                CreatedAt, 
                UpdatedAt)
              VALUES 
                (@SessionDate, 
                @TimeSpentInSeconds, 
                @TotalPomodoroCompleted, 
                @TotalShortBreaksCompleted, 
                @TotalLongBreaksCompleted, 
                @DailyPomodoroTarget,
                @CreatedAt, 
                @UpdatedAt)
              ON CONFLICT(SessionDate) 
              DO UPDATE SET
                TotalPomodoroCompleted = TotalPomodoroCompleted + @TotalPomodoroCompleted,
                TotalShortBreaksCompleted = TotalShortBreaksCompleted + @TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = TotalLongBreaksCompleted + @TotalLongBreaksCompleted,
                DailyPomodoroTarget = @DailyPomodoroTarget,
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

    private const string UPSERT_DAILY_SESSION_DAILY_POMODORO_TARGET_QUERY =
        @"
              INSERT INTO tblDailySessions 
                (SessionDate, DailyPomodoroTarget, CreatedAt, UpdatedAt)
              VALUES 
                (@SessionDate, @DailyPomodoroTarget, @CreatedAt, @UpdatedAt)
              ON CONFLICT(SessionDate) 
              DO UPDATE SET
                DailyPomodoroTarget = DailyPomodoroTarget + @DailyPomodoroTarget, 
                UpdatedAt = @UpdatedAt 
              WHERE SessionDate = @SessionDate;";


    private const string UPSERT_DAILY_SESSION_TOTAL_TASKS_COMPLETED_QUERY =
        @"
              INSERT INTO tblDailySessions 
                (SessionDate, TotalTasksCompleted, CreatedAt, UpdatedAt)
              VALUES 
                (@SessionDate, @TotalTasksCompleted, @CreatedAt, @UpdatedAt)
              ON CONFLICT(SessionDate) 
              DO UPDATE SET
                TotalTasksCompleted = TotalTasksCompleted + @TotalTasksCompleted, 
                UpdatedAt = @UpdatedAt 
              WHERE SessionDate = @SessionDate;";

    private const string SELECT_DAILY_SESSION_TOTAL_POMODORO_COMPLETED_QUERY =
        @"
            SELECT TotalPomodoroCompleted FROM tblDailySessions 
              WHERE SessionDate = @SessionDate LIMIT 1;";

    private const string SELECT_DAILY_SESSIONS_SUM_QUERY =
        @"
            SELECT 
                SUM(TimeSpentInSeconds) AS TotalTimeSpentInSeconds, 
                SUM(TotalPomodoroCompleted) AS TotalPomodoroCompleted, 
                SUM(TotalShortBreaksCompleted) AS TotalShortBreaksCompleted, 
                SUM(TotalLongBreaksCompleted) AS TotalLongBreaksCompleted,
                SUM(TotalTasksCompleted) AS TotalTasksCompleted
            FROM tblDailySessions
            WHERE
                CreatedAt >= @FromDate AND CreatedAt <= @ToDate
            LIMIT 1
            ";

    private const string SELECT_DAILY_SESSIONS_QUERY =
        @"
            SELECT * FROM tblDailySessions WHERE CreatedAt >= @FromDate AND CreatedAt <= @ToDate ORDER BY datetime(CreatedAt) DESC
            ";

    private const string SELECT_DATE_RANGE_DAILY_SESSIONS_QUERY =
        @"
            SELECT MIN(datetime(CreatedAt)) AS MinDate, 
            MAX(datetime(CreatedAt)) AS MaxDate FROM tblDailySessions;
            ";

    public SqliteDailySessionsRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<DailySessionDTO>> GetDailySessions(DateTime from, DateTime to)
    {
        using var db = _connectionFactory.Connect();
        var data = new
        {
            FromDate = from,
            ToDate = to
        };

        return await db.QueryAsync<DailySessionDTO>(SELECT_DAILY_SESSIONS_QUERY, data);
    }

    public async Task<SumDailySessionsDTO> GetSumDailySessions(DateTime from, DateTime to)
    {
        using var db = _connectionFactory.Connect();
        var data = new
        {
            FromDate = from,
            ToDate = to
        };

        return await db.QueryFirstAsync<SumDailySessionsDTO>(
            SELECT_DAILY_SESSIONS_SUM_QUERY, data);
    }

    public async Task<DateRangeDailySessionsDTO> GetDateRangeDailySessions()
    {
        using var db = _connectionFactory.Connect();
        return await db.QueryFirstAsync<DateRangeDailySessionsDTO>(
            SELECT_DATE_RANGE_DAILY_SESSIONS_QUERY);
    }

    public async Task UpsertDailySession(UpsertDailySessionDTO dto)
    {
        using var db = _connectionFactory.Connect();
        await db.ExecuteAsync(UPSERT_DAILY_SESSION_QUERY, dto);
    }

    public async Task UpsertTimeSpent(UpsertTimeSpentDTO dto)
    {
        using var db = _connectionFactory.Connect();
        await db.ExecuteAsync(UPSERT_DAILY_SESSION_TIME_SPENT_QUERY, dto);
    }

    public async Task UpsertDailyPomodoroTarget(UpsertDailyPomodoroTargetDTO dto)
    {
        using var db = _connectionFactory.Connect();
        await db.ExecuteAsync(UPSERT_DAILY_SESSION_DAILY_POMODORO_TARGET_QUERY, dto);
    }

    public async Task UpsertTotalTasksCompleted(UpsertTotalTasksCompletedDTO dto)
    {
        using var db = _connectionFactory.Connect();
        await db.ExecuteAsync(UPSERT_DAILY_SESSION_TOTAL_TASKS_COMPLETED_QUERY, dto);
    }

    public async Task<int> GetTotalPomodoroCompleted(string SessionDate)
    {
        using var db = _connectionFactory.Connect();
        var data = new
        {
            SessionDate = SessionDate
        };

        return await db.ExecuteScalarAsync<int>(SELECT_DAILY_SESSION_TOTAL_POMODORO_COMPLETED_QUERY, data);
    }
}