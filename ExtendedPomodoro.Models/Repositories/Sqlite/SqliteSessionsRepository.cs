using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;

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

        public SqliteSessionsRepository(SqliteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        
        public async IAsyncEnumerable<DailySessionDomain>
            GetDailySessions(DateTime from, DateTime to)
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = new
                {
                    FromDate = from,
                    ToDate = to
                };

                var records = await db.QueryAsync<DailySessionDTO>
                    (SELECT_DAILY_SESSIONS_QUERY, data);

                foreach (var record in records) yield return ConvertToDailySessionDomain(record);
            }
        }

        public async Task<SumDailySessionsDomain> GetSumDailySessions(DateTime from, DateTime to)
        {

            using (var db = _connectionFactory.Connect())
            {
                var data = new
                {
                    FromDate = from,
                    ToDate = to
                };

                var res = await db.QueryFirstAsync<SumDailySessionsDTO>(
                    SELECT_DAILY_SESSIONS_SUM_QUERY, data);

                return ConvertToSumDailySessionsDomain(from, to, res);
            }
        }

        public async Task<DateRangeDailySessionsDomain> GetDateRangeDailySessions() {
            using (var db = _connectionFactory.Connect())
            {
                var res = await db.QueryFirstAsync<DateRangeDailySessionsDTO>(
                    SELECT_DATE_RANGE_DAILY_SESSIONS_QUERY);

                return ConvertToSumDailySessionsDomain(res);
            }
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

        public async Task UpsertDailySessionTotalTasksCompleted(DateOnly sessionDate, int totalTasksCompleted)
        {
            var now = DateTime.Now;

            using (var db = _connectionFactory.Connect())
            {
                var data = new
                {
                    SessionDate = sessionDate.ToString(),
                    TotalTasksCompleted = totalTasksCompleted,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await db.ExecuteAsync(UPSERT_DAILY_SESSION_TOTAL_TASKS_COMPLETED_QUERY, data);
            }
        }

        public async Task<int> GetDailySessionTotalPomodoroCompleted(DateOnly SessionDate)
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = new
                {
                    SessionDate = SessionDate.ToString()
                };

                return await db.ExecuteScalarAsync<int>(SELECT_DAILY_SESSION_TOTAL_POMODORO_COMPLETED_QUERY, data);
            }
        }

        private static SqliteUpsertDailySessionDTO ConvertToUpsertSqliteDailySessionDTO(UpsertDailySessionDomain domain)
        {
            var now = DateTime.Now;

            return new SqliteUpsertDailySessionDTO()
            {
                SessionDate = domain.SessionDate.ToString(),
                TotalPomodoroCompleted = domain.TotalPomodoroCompleted,
                TotalShortBreaksCompleted = domain.TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = domain.TotalLongBreaksCompleted,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static SumDailySessionsDomain ConvertToSumDailySessionsDomain(DateTime from, DateTime to, SumDailySessionsDTO res)
        {
            return new(
                from,
                to,
                TimeSpan.FromSeconds(res.TotalTimeSpentInSeconds),
                res.TotalPomodoroCompleted,
                res.TotalShortBreaksCompleted,
                res.TotalLongBreaksCompleted,
                res.TotalTasksCompleted
                );
        }
       
        private DailySessionDomain ConvertToDailySessionDomain(DailySessionDTO record)
        {
            return new(
                DateOnly.FromDateTime(record.SessionDate),
                TimeSpan.FromSeconds(record.TimeSpentInSeconds),
                record.TotalPomodoroCompleted,
                record.TotalShortBreaksCompleted,
                record.TotalLongBreaksCompleted,
                record.TotalTasksCompleted,
                record.CreatedAt,
                record.UpdatedAt
                );
        }

        private DateRangeDailySessionsDomain ConvertToSumDailySessionsDomain(DateRangeDailySessionsDTO dto)
        {
            return new(dto.MinDate, dto.MaxDate);
        }

    }
}