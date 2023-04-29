using ExtendedPomodoro.Models.Domains;

namespace ExtendedPomodoro.Models.Services.Interfaces
{
    public interface IDailySessionsService
    {
        Task UpsertDailySession(UpsertDailySessionDomain domain);
        Task UpsertTimeSpent(DateOnly sessionDate, TimeSpan timeSpent);
        Task<int> GetTotalPomodoroCompleted(DateOnly SessionDate);
        Task UpsertTotalTasksCompleted(DateOnly sessionDate, int totalPomodoroCompleted);
        Task<SumDailySessionsDomain> GetSumDailySessions(DateTime from, DateTime to);
        IAsyncEnumerable<DailySessionDomain> GetDailySessions(DateTime from, DateTime to);
        Task<DateRangeDailySessionsDomain> GetDateRangeDailySessions();
    }
}
