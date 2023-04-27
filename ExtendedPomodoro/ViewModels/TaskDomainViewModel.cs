namespace ExtendedPomodoro.ViewModels
{
    public record class TaskDomainViewModel(
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
