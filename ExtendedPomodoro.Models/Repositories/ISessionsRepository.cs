using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
