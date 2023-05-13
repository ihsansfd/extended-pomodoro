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

            foreach (var record in records) 
                yield return ConvertToDailySessionDomain(record);
        }

        public async Task<SumDailySessionsDomain> GetSumDailySessions(DateTime from, DateTime to)
        {
            var res = await _repository.GetSumDailySessions(from, to);

            return ConvertToSumDailySessionsDomain(res);
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

            var data = 
                ConvertToUpsertTotalTasksCompletedDTO(sessionDate, totalTasksCompleted, now);
            await _repository.UpsertTotalTasksCompleted(data);
        }

        public async Task UpsertDailyPomodoroTarget(DateOnly sessionDate, int dailyPomodoroTarget)
        {
            var now = DateTime.Now;
            var data = 
                ConvertToUpsertDailyPomodoroTargetDTO(sessionDate, dailyPomodoroTarget, now);

            await _repository.UpsertDailyPomodoroTarget(data);
        }

        public async Task<int> GetTotalPomodoroCompleted(DateOnly SessionDate)
        {
            return await _repository.GetTotalPomodoroCompleted(SessionDate.ToString());
        }

        private static UpsertTotalTasksCompletedDTO ConvertToUpsertTotalTasksCompletedDTO(
            DateOnly sessionDate, int totalTasksCompleted, DateTime now)
        {
            return new UpsertTotalTasksCompletedDTO()
            {
                SessionDate = sessionDate.ToString(),
                TotalTasksCompleted = totalTasksCompleted,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static UpsertDailyPomodoroTargetDTO ConvertToUpsertDailyPomodoroTargetDTO(
            DateOnly sessionDate, int dailyPomodoroTarget, DateTime now)
        {
            return new UpsertDailyPomodoroTargetDTO()
            {
                SessionDate = sessionDate.ToString(),
                DailyPomodoroTarget = dailyPomodoroTarget,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static UpsertTimeSpentDTO 
            ConvertToUpsertTimeSpentDTO(DateOnly sessionDate, TimeSpan timeSpent, DateTime now)
        {
            return new UpsertTimeSpentDTO()
            {
                SessionDate = sessionDate.ToString(),
                TimeSpentInSeconds = (int)timeSpent.TotalSeconds,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static UpsertDailySessionDTO 
            ConvertToUpsertDailySessionDTO(UpsertDailySessionDomain domain)
        {
            var now = DateTime.Now;

            return new UpsertDailySessionDTO()
            {
                SessionDate = domain.SessionDate.ToString(),
                TotalPomodoroCompleted = domain.TotalPomodoroCompleted,
                TotalShortBreaksCompleted = domain.TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = domain.TotalLongBreaksCompleted,
                DailyPomodoroTarget = domain.DailyPomodoroTarget,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        private static SumDailySessionsDomain 
            ConvertToSumDailySessionsDomain(SumDailySessionsDTO dto)
        {
            return new SumDailySessionsDomain()
            {
                TotalTimeSpent = TimeSpan.FromSeconds(dto.TotalTimeSpentInSeconds),
                TotalPomodoroCompleted = dto.TotalPomodoroCompleted,
                TotalShortBreaksCompleted = dto.TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = dto.TotalLongBreaksCompleted,
                TotalTasksCompleted = dto.TotalTasksCompleted
            };
        }

        private static DailySessionDomain ConvertToDailySessionDomain(DailySessionDTO record)
        {
            return new DailySessionDomain()
            {
                SessionDate = DateOnly.FromDateTime(record.SessionDate),
                TimeSpent = TimeSpan.FromSeconds(record.TimeSpentInSeconds),
                TotalPomodoroCompleted = record.TotalPomodoroCompleted,
                TotalShortBreaksCompleted = record.TotalShortBreaksCompleted,
                TotalLongBreaksCompleted = record.TotalLongBreaksCompleted,
                TotalTasksCompleted = record.TotalTasksCompleted,
                DailyPomodoroTarget = record.DailyPomodoroTarget,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }

        private static DateRangeDailySessionsDomain 
            ConvertToSumDailySessionsDomain(DateRangeDailySessionsDTO dto)
        {
            return new DateRangeDailySessionsDomain(dto.MinDate, dto.MaxDate);
        }
    }

    public static class DailySessionsExtensions
    {
        public static SumDailySessionsDomain SumEach(this IEnumerable<DailySessionDomain> domains)
        {
            var totalPomodoroCompleted = 0;
            var totalShortBreaksCompleted = 0;
            var totalLongBreaksCompleted = 0;
            var totalTasksCompleted = 0;
            var totalTimeSpent = TimeSpan.Zero;

            var sumDailyPomodoroTarget = new SumDailyPomodoroTargetDomain();

            foreach (var record in domains)
            {
                totalPomodoroCompleted += record.TotalPomodoroCompleted;
                totalShortBreaksCompleted += record.TotalShortBreaksCompleted;
                totalLongBreaksCompleted += record.TotalLongBreaksCompleted;
                totalTasksCompleted += record.TotalTasksCompleted;
                totalTimeSpent += record.TimeSpent;
                sumDailyPomodoroTarget.TotalTarget += 1;
                if (record.TotalPomodoroCompleted >= record.DailyPomodoroTarget)
                {
                    sumDailyPomodoroTarget.TotalActual += 1;
                }

            }

            return new SumDailySessionsDomain()
            {
                TotalPomodoroCompleted = totalPomodoroCompleted,
                TotalShortBreaksCompleted = totalShortBreaksCompleted,
                TotalLongBreaksCompleted = totalLongBreaksCompleted,
                TotalTasksCompleted = totalTasksCompleted,
                TotalTimeSpent = totalTimeSpent,
                SumDailyPomodoroTarget = sumDailyPomodoroTarget
            };
        }
    }
}