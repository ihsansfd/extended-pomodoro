using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Domains
{

    public enum TaskState
    {
        IN_PROGRESS = 0,
        COMPLETED = 1
    }

    public record class CreateTaskDomain(
        string Name,
        string? Description,
        int? EstPomodoro
        );

    public record class UpdateTaskDomain(
        int Id,
        string Name,
        string? Description,
        int? EstPomodoro,
        TaskState Taskstate
       );

    public record class TaskDomain(
        int Id,
        string Name, 
        string Description,
        int EstPomodoro,
        int ActPomodoro,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        TaskState TaskState,
        TimeSpan TimeSpent
        );
}
