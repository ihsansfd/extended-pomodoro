namespace ExtendedPomodoro.ViewModels
{
    public record TaskDomainViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? EstPomodoro { get; set; }
        public int ActPomodoro { get; set; }
        public string CreatedAt { get; set; } = null!;
        public string? CompletedAt { get; set; }
        public int TaskStatus { get; set; }
        public double TimeSpentInMinutes { get; set; }
    }
}
