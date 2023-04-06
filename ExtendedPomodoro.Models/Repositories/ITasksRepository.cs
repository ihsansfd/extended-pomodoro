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
        IAsyncEnumerable<TaskDomain> GetTasks(TaskState taskState = TaskState.IN_PROGRESS, int limit = 20);
        Task DeleteTask(int taskId);
    }
}
