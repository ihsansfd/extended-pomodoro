namespace ExtendedPomodoro.Models.Domains
{
    public record SumDailyPomodoroTargetDomain()
    {
        public int TotalActual { get; set; }
        public int TotalTarget { get; set; }

        public string SuccessRateFormatted => (TotalTarget == 0 ? 1
            : Math.Min((double)TotalActual / (double)TotalTarget, 1)).ToString("P");
    }

    public record DailySessionDomain()
    {
        public DateOnly SessionDate { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public int TotalTasksCompleted { get; set; }
        public int DailyPomodoroTarget { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
    }

    public record UpsertDailySessionDomain
    {
        public DateOnly SessionDate { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public int DailyPomodoroTarget { get; set; } 
    }

    public record SumDailySessionsDomain
    {
        public TimeSpan TotalTimeSpent { get; set; }
        public int TotalPomodoroCompleted { get; set; }
        public int TotalShortBreaksCompleted { get; set; }
        public int TotalLongBreaksCompleted { get; set; }
        public int TotalTasksCompleted { get; set; }
        public SumDailyPomodoroTargetDomain SumDailyPomodoroTarget { get; set; } = new();
    };

    public record ComparedSumDailySessionsDomain
    {
        public double? TotalTimeSpent { get; set; }
        public double? TotalPomodoroCompleted { get; set; }
        public double? TotalShortBreaksCompleted { get; set; }
        public double? TotalLongBreaksCompleted { get; set; }
        public double? TotalTasksCompleted { get; set; }
    }

    public record DateRangeDailySessionsDomain(
        DateTime MinDate,
        DateTime MaxDate
        );


}
