namespace ExtendedPomodoro.Models.Domains
{

    public enum TaskState
    {
        IN_PROGRESS = 0,
        COMPLETED = 1
    }

    public record CreateTaskDomain(
        string Name,
        string? Description = null,
        int? EstPomodoro = null
        );

    public record UpdateTaskDomain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? EstPomodoro { get; set; }
        public TaskState Taskstate { get; set; }
    }

    public record TaskDomain
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? EstPomodoro { get; set; }
        public int ActPomodoro { get; set; }
        public TaskState TaskState { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
        public DateTime? CompletedAt { get; set; }
        public TimeSpan TimeSpent { get; set; }
    }

    //public record TaskCompletionCountDomain
    //{
    //    public DateTime FromDate { get; set; }
    //    public DateTime ToDate { get; set; }
    //    public int CompletedTasksCount { get; set; }
    //    public int UncompletedTasksCount { get; set; }
    //}
}
