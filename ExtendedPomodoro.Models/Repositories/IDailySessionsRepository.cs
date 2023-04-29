using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface IDailySessionsRepository
    {
        Task UpsertDailySession(UpsertDailySessionDTO dto);
        Task UpsertTimeSpent(UpsertTimeSpentDTO data);
        Task<int> GetTotalPomodoroCompleted(string SessionDate);
        Task UpsertTotalTasksCompleted(UpsertTotalTasksCompletedDTO dto);
        Task<SumDailySessionsDTO> GetSumDailySessions(DateTime from, DateTime to);
        Task<IEnumerable<DailySessionDTO>> GetDailySessions(DateTime from, DateTime to);
        Task<DateRangeDailySessionsDTO> GetDateRangeDailySessions();
    }
}
