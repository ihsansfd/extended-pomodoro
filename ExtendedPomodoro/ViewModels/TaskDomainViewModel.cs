namespace ExtendedPomodoro.ViewModels
{
    public record TaskDomainViewModel(
        int Id,
        string Name,
        string? Description,
        int? EstPomodoro,
        int ActPomodoro,
        string CreatedAt,
        int TaskStatus,
        double TimeSpentInMinutes
        );
}
