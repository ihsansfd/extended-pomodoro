using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ITasksRepository
    {
        Task CreateTask(CreateTaskDomain task);
        Task UpdateTask(UpdateTaskDomain task);
        Task UpdateTaskState(int taskId, TaskState taskIntendedState);
        Task UpdateTaskTimeSpent(int taskId, TimeSpan timeSpent);
        Task UpdateTaskActPomodoro(int taskId, int ActPomodoroIncrementBy);
        IAsyncEnumerable<TaskDomain> GetTasks(TaskState taskState = TaskState.IN_PROGRESS, int page = 1, int limit = 20);
        Task DeleteTask(int taskId);
        Task<int> GetTotalPages(TaskState taskState = TaskState.IN_PROGRESS, int limit = 20);
    }
}
