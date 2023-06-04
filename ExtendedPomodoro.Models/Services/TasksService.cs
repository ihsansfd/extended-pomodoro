using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Services.Interfaces;

namespace ExtendedPomodoro.Models.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _repository;

        public TasksService(ITasksRepository repository)
        {
            _repository = repository;
        }

        public async IAsyncEnumerable<TaskDomain> GetTasks(TaskState taskState = TaskState.IN_PROGRESS,
            int page = 1, int limit = 20)
        {
            var data = ConvertToGetTaskDTO(taskState, page, limit);
            var records = await _repository.GetTasks(data);
            foreach (var record in records) yield return ConvertToTaskDomain(record);

        }

        public async IAsyncEnumerable<TaskDomain> GetTasks(DateTime from, DateTime to)
        {
            var records = await _repository.GetTasks(from, to);
            foreach(var record in records) yield return ConvertToTaskDomain(record);
        }

        public async Task<int> GetTotalPages(TaskState taskState = TaskState.IN_PROGRESS, int limit = 20)
        {
            if (limit == 0) return 0;

            int totalRows = await _repository.GetTotalRows(ConvertTaskStateToInt(taskState));
            return (totalRows + (limit - 1)) / limit;
        }

        public async Task CreateTask(CreateTaskDomain domain)
        {
            var now = DateTime.Now;

            var data = ConvertToSqliteCreateTaskDTO(domain, now);

            await _repository.CreateTask(data);
        }

        public async Task UpdateTaskState(int taskId, TaskState taskState)
        {
            var now = DateTime.Now;

            DateTime? completedAt = taskState == TaskState.COMPLETED ? now : null;

            var data = ConvertToUpdateTaskStateDTO(taskId, taskState, now, completedAt);

            await _repository.UpdateTaskState(data);
        }

        public async Task UpdateTimeSpent(int taskId, TimeSpan timeSpent)
        {
            var now = DateTime.Now;

            var data = ConvertToUpdateTimeSpentDTO(taskId, timeSpent, now);

            await _repository.UpdateTimeSpent(data);
        }

        public async Task UpdateTask(UpdateTaskDomain domain)
        {
            var now = DateTime.Now;
            DateTime? completedAt = domain.Taskstate == TaskState.COMPLETED ? now : null;

            var data = ConvertToSqliteUpdateTaskDTO(domain, now, completedAt);

            await _repository.UpdateTask(data);
        }

        public async Task UpdateActPomodoro(int taskId, int actPomodoroIncrementBy)
        {
            UpdateActPomodoroDTO data = new()
            {
                Id = taskId,
                ActPomodoroIncrementBy = actPomodoroIncrementBy,
                UpdatedAt = DateTime.Now
            };

            await _repository.UpdateActPomodoro(data);
        }

        public async Task DeleteTask(int taskId)
        {
           await _repository.DeleteTask(taskId);
        }

        private UpdateTimeSpentDTO ConvertToUpdateTimeSpentDTO(int taskId, TimeSpan timeSpent, DateTime now)
        {
            return new()
            {
                Id = taskId,
                TimeSpentInSecondsIncrementBy = (int)timeSpent.TotalSeconds,
                UpdatedAt = now
            };
        }

        private UpdateTaskStateDTO ConvertToUpdateTaskStateDTO(int taskId, TaskState taskState, DateTime now, DateTime? completedAt)
        {
            return new()
            {
                Id = taskId,
                IsTaskCompleted = ConvertTaskStateToInt(taskState),
                CompletedAt = completedAt,
                UpdatedAt = now
            };
        }

        private static GetTaskDTO ConvertToGetTaskDTO(TaskState taskState, int page, int limit)
        {
            return new()
            {
                IsTaskCompleted = ConvertTaskStateToInt(taskState),
                Limit = limit,
                Offset = limit * (page - 1)
            };
        }

        private static CreateTaskDTO ConvertToSqliteCreateTaskDTO(CreateTaskDomain domain, DateTime now)
        {
            return new CreateTaskDTO()
            {
                Name = domain.Name,
                Description = domain.Description,
                EstPomodoro = domain.EstPomodoro,
                CreatedAt = now,
                UpdatedAt = now,
            };
        }

        private static UpdateTaskDTO ConvertToSqliteUpdateTaskDTO(UpdateTaskDomain domain, DateTime updatedAt, DateTime? completedAt)
        {
            return new UpdateTaskDTO()
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                EstPomodoro = domain.EstPomodoro,
                IsTaskCompleted = ConvertTaskStateToInt(domain.Taskstate),
                UpdatedAt = updatedAt,
                CompletedAt = completedAt
            };
        }

        private static TaskDomain ConvertToTaskDomain(TaskDTO dto)
        {
            return new TaskDomain()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                EstPomodoro = dto.EstPomodoro,
                ActPomodoro = dto.ActPomodoro,
                TaskState = dto.IsTaskCompleted == 1 ? TaskState.COMPLETED : TaskState.IN_PROGRESS,
                TimeSpent = TimeSpan.FromSeconds(dto.TimeSpentInSeconds),
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                CompletedAt = dto.CompletedAt
            };
        }

        private static int ConvertTaskStateToInt(TaskState taskState)
        {
            return taskState == TaskState.COMPLETED ? 1 : 0;
        }
    }
}
