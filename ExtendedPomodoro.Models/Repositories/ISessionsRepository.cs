using ExtendedPomodoro.Models.Domains;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ISessionsRepository
    {
        Task UpsertDailySession(UpsertDailySessionDomain domain);
        Task UpsertDailySessionTimeSpent(DateOnly sessionDate, TimeSpan timeSpent);
        Task<int> GetDailySessionTotalPomodoroCompleted(DateOnly SessionDate);
        Task UpsertDailySessionTotalTasksCompleted(DateOnly sessionDate, int totalPomodoroCompleted);
        Task<SumDailySessionsDomain> GetSumDailySessions(DateTime from, DateTime to);
        IAsyncEnumerable<DailySessionDomain> GetDailySessions(DateTime from, DateTime to);
        Task<DateRangeDailySessionsDomain> GetDateRangeDailySessions();
    }
}
