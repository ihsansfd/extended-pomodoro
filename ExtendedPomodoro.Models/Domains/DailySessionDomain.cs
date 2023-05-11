namespace ExtendedPomodoro.Models.Domains
{
    public record DailySessionDomain()
    {
        public DateOnly SessionDate { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public int TotalTasksCompleted { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
    }

    public record UpsertDailySessionDomain
    {
        public DateOnly SessionDate { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
    }

    public record SumDailySessionsDomain
    {
        public TimeSpan TotalTimeSpent { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public int TotalTasksCompleted { get; set; }
    };

    public record DateRangeDailySessionsDomain(
        DateTime MinDate,
        DateTime MaxDate
        );

}
