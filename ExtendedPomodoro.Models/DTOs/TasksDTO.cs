using ExtendedPomodoro.Models.Domains;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DTOs
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? EstPomodoro { get; set; }
        public int ActPomodoro { get; set; }
        public int IsTaskCompleted { get; set; }
        public int TimeSpentInSeconds { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class GetTaskDTO
    {
        public int IsTaskCompleted { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }

    public class CreateTaskDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? EstPomodoro { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateTaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? EstPomodoro { get; set; }
        public int IsTaskCompleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateActPomodoroDTO
    {
        public int Id { get; set; }
        public int ActPomodoroIncrementBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
