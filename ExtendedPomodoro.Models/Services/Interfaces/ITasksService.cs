using ExtendedPomodoro.Models.Domains;

namespace ExtendedPomodoro.Models.Services.Interfaces
{
    public interface ITasksService
    {
        Task CreateTask(CreateTaskDomain task);
        Task UpdateTask(UpdateTaskDomain task);
        Task UpdateTaskState(int taskId, TaskState taskIntendedState);
        Task UpdateTimeSpent(int taskId, TimeSpan timeSpent);
        Task UpdateActPomodoro(int taskId, int ActPomodoroIncrementBy);
        IAsyncEnumerable<TaskDomain> GetTasks(TaskState taskState = TaskState.IN_PROGRESS, int page = 1, int limit = 20);
        Task DeleteTask(int taskId);
        Task<int> GetTotalPages(TaskState taskState = TaskState.IN_PROGRESS, int limit = 20);
    }
}
