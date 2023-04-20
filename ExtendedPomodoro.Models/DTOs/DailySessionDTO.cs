using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DTOs
{
    public class SqliteUpsertDailySessionDTO
    {
        public string SessionDate { get; set; }
        public int TimeSpentInSeconds { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public record class DailySessionDTO
    {
        public DateTime SessionDate { get; set; }
        public int TimeSpentInSeconds { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public int TotalTasksCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SumDailySessionsDTO
    {
        public int TotalTimeSpentInSeconds { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public int TotalTasksCompleted { get; set; }
    }
}
