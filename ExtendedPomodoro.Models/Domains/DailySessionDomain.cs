namespace ExtendedPomodoro.Models.Domains
{

    // TODO: Refactor for too long parameter list, make properties
    public record class DailySessionDomain(
        DateOnly SessionDate, 
        TimeSpan TimeSpent, 
        int TotalPomodoroCompleted,
        int TotalShortBreaksCompleted,
        int TotalLongBreaksCompleted,
        int TotalTasksCompleted,
        DateTime CreatedAt,
        DateTime UpdatedAt
        );

    public record class UpsertDailySessionDomain
    {
        public DateOnly SessionDate { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
    }
        
    public record class SumDailySessionsDomain(
        DateTime fromDate,
        DateTime toDate,
        TimeSpan TotalTimeSpent,
        int TotalPomodoroCompleted,
        int TotalShortBreaksCompleted,
        int TotalLongBreaksCompleted,
        int TotalTasksCompleted
        );

    public record class DateRangeDailySessionsDomain(
        DateTime MinDate,
        DateTime MaxDate
        );

}
