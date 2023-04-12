using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Domains
{
    public record class DailySessionDomain(
        DateOnly SessionDate, 
        TimeSpan TimeSpent, 
        int TotalPomodoroCompleted,
        int TotalShortBreaksCompleted,
        int TotalLongBreaksCompleted,
        DateTime CreatedAt,
        DateTime UpdatedAt
        );

    public record class UpsertDailySessionDomain(
        DateOnly SessionDate,
        TimeSpan TimeSpent,
        int TotalPomodoroCompleted,
        int TotalShortBreaksCompleted,
        int TotalLongBreaksCompleted
        );

}
