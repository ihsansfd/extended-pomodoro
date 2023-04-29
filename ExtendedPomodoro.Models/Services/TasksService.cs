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

        public TasksService(IDbConnectionFactory connectionFactory, ITasksRepository repository)
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

        public async Task<int> GetTotalPages(TaskState taskState = TaskState.IN_PROGRESS, int limit = 20)
        {
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
            await _repository.UpdateTaskState(taskId, ConvertTaskStateToInt(taskState));
        }

        public async Task UpdateTimeSpent(int taskId, TimeSpan timeSpent)
        {
            await _repository.UpdateTimeSpent(taskId, (int)timeSpent.TotalSeconds);
        }

        public async Task UpdateTask(UpdateTaskDomain domain)
        {
            var data = ConvertToSqliteUpdateTaskDTO(domain);

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

        private static UpdateTaskDTO ConvertToSqliteUpdateTaskDTO(UpdateTaskDomain domain)
        {
            return new UpdateTaskDTO()
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                EstPomodoro = domain.EstPomodoro,
                IsTaskCompleted = ConvertTaskStateToInt(domain.Taskstate),
                UpdatedAt = DateTime.Now,
            };
        }

        private static TaskDomain ConvertToTaskDomain(TaskDTO dto)
        {
            return new(
                    dto.Id,
                    dto.Name,
                    dto.Description,
                    dto.EstPomodoro,
                    dto.ActPomodoro,
                    dto.CreatedAt,
                    dto.UpdatedAt,
                    dto.IsTaskCompleted == 1 ? TaskState.COMPLETED : TaskState.IN_PROGRESS,
                    TimeSpan.FromSeconds(dto.TimeSpentInSeconds)
                );
        }

        private static int ConvertTaskStateToInt(TaskState taskState)
        {
            return taskState == TaskState.COMPLETED ? 1 : 0;
        }
    }
}
