namespace ExtendedPomodoro.Models.Domains
{

    // TODO: Refactor for too long parameter list, make properties

    public enum TaskState
    {
        IN_PROGRESS = 0,
        COMPLETED = 1
    }

    public record class CreateTaskDomain(
        string Name,
        string? Description = null,
        int? EstPomodoro = null
        );

    public record class UpdateTaskDomain
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? EstPomodoro { get; set; }
        public TaskState Taskstate { get; set; }
    }
       
    public record class TaskDomain(
        int Id,
        string Name, 
        string? Description,
        int? EstPomodoro,
        int ActPomodoro,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        TaskState TaskState,
        TimeSpan TimeSpent
        );
}
