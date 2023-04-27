namespace ExtendedPomodoro.Models.Domains
{
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

    public record class UpsertDailySessionDomain(
        DateOnly SessionDate,
        int TotalPomodoroCompleted,
        int TotalShortBreaksCompleted,
        int TotalLongBreaksCompleted
        );

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
