using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Services.Interfaces;

namespace ExtendedPomodoro.Models.Services
{
    public class DailySessionsService : IDailySessionsService
    {
        private readonly IDailySessionsRepository _repository;

        public DailySessionsService(IDailySessionsRepository repository)
        {
            _repository = repository;
        }

        public async IAsyncEnumerable<DailySessionDomain>
            GetDailySessions(DateTime from, DateTime to)
        {
            var records = await _repository.GetDailySessions(from, to);

            foreach (var record in records) yield return ConvertToDailySessionDomain(record);
        }

        public async Task<SumDailySessionsDomain> GetSumDailySessions(DateTime from, DateTime to)
        {
            var res = await _repository.GetSumDailySessions(from, to);

            return ConvertToSumDailySessionsDomain(from, to, res);
        }

        public async Task<DateRangeDailySessionsDomain> GetDateRangeDailySessions()
        {
            var res = await _repository.GetDateRangeDailySessions();

            return ConvertToSumDailySessionsDomain(res);
        }

        public async Task UpsertDailySession(UpsertDailySessionDomain domain)
        {
            var data = ConvertToUpsertDailySessionDTO(domain);
            await _repository.UpsertDailySession(data);
        }

        public async Task UpsertTimeSpent(DateOnly sessionDate, TimeSpan timeSpent)
        {
            var now = DateTime.Now;

            var data = ConvertToUpsertTimeSpentDTO(sessionDate, timeSpent, now);
            await _repository.UpsertTimeSpent(data);
        }

        public async Task UpsertTotalTasksCompleted(DateOnly sessionDate, int totalTasksCompleted)
        {
            var now = DateTime.Now;

            var data = ConvertToUpsertTotalTasksCompletedDTO(sessionDate, totalTasksCompleted, now);
            await _repository.UpsertTotalTasksCompleted(data);
        }

        public async Task<int> GetTotalPomodoroCompleted(DateOnly SessionDate)
        {
            return await _repository.GetTotalPomodoroCompleted(SessionDate.ToString());
        }

        private static UpsertTotalTasksCompletedDTO ConvertToUpsertTotalTasksCompletedDTO(DateOnly sessionDate, int totalTasksCompleted, DateTime now)
        {
            return new()
            {
                SessionDate = sessionDate.ToString(),
                TotalTasksCompleted = totalTasksCompleted,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static UpsertTimeSpentDTO ConvertToUpsertTimeSpentDTO(DateOnly sessionDate, TimeSpan timeSpent, DateTime now)
        {
            return new()
            {
                SessionDate = sessionDate.ToString(),
                TimeSpentInSeconds = (int)timeSpent.TotalSeconds,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static UpsertDailySessionDTO ConvertToUpsertDailySessionDTO(UpsertDailySessionDomain domain)
        {
            var now = DateTime.Now;

            return new UpsertDailySessionDTO()
            {
                SessionDate = domain.SessionDate.ToString(),
                TotalPomodoroCompleted = domain.TotalPomodoroCompleted,
                TotalShortBreaksCompleted = domain.TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = domain.TotalLongBreaksCompleted,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static SumDailySessionsDomain ConvertToSumDailySessionsDomain(DateTime from, DateTime to, SumDailySessionsDTO dto)
        {
            return new(
                from,
                to,
                TimeSpan.FromSeconds(dto.TotalTimeSpentInSeconds),
                dto.TotalPomodoroCompleted,
                dto.TotalShortBreaksCompleted,
                dto.TotalLongBreaksCompleted,
                dto.TotalTasksCompleted
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