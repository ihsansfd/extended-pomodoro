using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Services.Interfaces;

namespace ExtendedPomodoro.Models.Services
{
    public class TasksService : ITasksService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private const string CREATE_TASK_QUERY =
            @"INSERT INTO tblTasks (Name, Description, EstPomodoro, CreatedAt, UpdatedAt)
              VALUES (@Name, @Description, @EstPomodoro, @CreatedAt, @UpdatedAt)";

        // TODO: UpdatedAt not updated
        private const string UPDATE_TASK_QUERY =
            @"UPDATE tblTasks SET Name = @Name, Description = @Description, EstPomodoro = @EstPomodoro, 
            IsTaskCompleted = @IsTaskCompleted WHERE Id = @Id";

        // TODO: UpdatedAt not updated
        private const string UPDATE_TASK_STATE_QUERY =
            @"UPDATE tblTasks SET IsTaskCompleted = @IsTaskCompleted WHERE Id = @Id";

        // TODO: UpdatedAt not updated
        private const string UPDATE_TASK_TIME_SPENT_QUERY =
            @"UPDATE tblTasks SET TimeSpentInSeconds = TimeSpentInSeconds + @TimeSpentInSecondsIncrementBy WHERE Id = @Id";

        // TODO: UpdatedAt not updated
        private const string UPDATE_TASK_ACT_POMODORO_QUERY =
            @"UPDATE tblTasks
                SET 
                    ActPomodoro = ActPomodoro + @ActPomodoroIncrementBy,
                    UpdatedAt = @UpdatedAt
                WHERE 
                    Id = @Id";

        private const string GET_TASKS_QUERY =
            @"SELECT * FROM tblTasks WHERE IsTaskCompleted = @IsTaskCompleted
             ORDER BY datetime(CreatedAt) DESC LIMIT @Limit OFFSET @Offset";

        private const string GET_TOTAL_PAGES_QUERY =
            @"SELECT COUNT(*) FROM tblTasks WHERE IsTaskCompleted = @IsTaskCompleted";

        private const string DELETE_TASK_QUERY =
            @"DELETE FROM tblTasks WHERE Id = @Id";

        public TasksService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async IAsyncEnumerable<TaskDomain> GetTasks(TaskState taskState = TaskState.IN_PROGRESS,
            int page = 1, int limit = 20)
        {
            using (var db = _connectionFactory.Connect())
            {
                GetTaskDTO data = new()
                {
                    IsTaskCompleted = ConvertTaskStateToInt(taskState),
                    Limit = limit,
                    Offset = limit * (page - 1)
                };

                IEnumerable<TaskDTO> records = await db.QueryAsync<TaskDTO>(GET_TASKS_QUERY, data);

                foreach (var record in records) yield return ConvertToTaskDomain(record);
            }
        }

        public async Task<int> GetTotalPages(TaskState taskState = TaskState.IN_PROGRESS, int limit = 20)
        {
            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    IsTaskCompleted = ConvertTaskStateToInt(taskState),
                    Limit = limit,
                };

                int totalRows = await db.ExecuteScalarAsync<int>(GET_TOTAL_PAGES_QUERY, data);

                return (totalRows + (limit - 1)) / limit;

            }
        }

        public async Task CreateTask(CreateTaskDomain domain)
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = ConvertToSqliteCreateTaskDTO(domain);

                await db.ExecuteAsync(CREATE_TASK_QUERY, data);
            }
        }

        public async Task UpdateTaskState(int taskId, TaskState taskState)
        {
            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    Id = taskId,
                    IsTaskCompleted = ConvertTaskStateToInt(taskState)
                };

                await db.ExecuteAsync(UPDATE_TASK_STATE_QUERY, data);
            }
        }

        public async Task UpdateTimeSpent(int taskId, TimeSpan timeSpent)
        {
            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    Id = taskId,
                    TimeSpentInSecondsIncrementBy = timeSpent.TotalSeconds
                };

                await db.ExecuteAsync(UPDATE_TASK_TIME_SPENT_QUERY, data);
            }
        }

        public async Task UpdateTask(UpdateTaskDomain domain)
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = ConvertToSqliteUpdateTaskDTO(domain);

                await db.ExecuteAsync(UPDATE_TASK_QUERY, data);
            }
        }

        public async Task UpdateActPomodoro(int taskId, int ActPomodoroIncrementBy)
        {
            using (var db = _connectionFactory.Connect())
            {
                var data = new
                {
                    Id = taskId,
                    ActPomodoroIncrementBy,
                    UpdatedAt = DateTime.Now
                };

                await db.ExecuteAsync(UPDATE_TASK_ACT_POMODORO_QUERY, data);
            }
        }

        public async Task DeleteTask(int taskId)
        {
            using (var db = _connectionFactory.Connect())
            {
                await db.ExecuteAsync(DELETE_TASK_QUERY, new { Id = taskId });
            }
        }

        //public async Task 

        private static CreateTaskDTO ConvertToSqliteCreateTaskDTO(CreateTaskDomain domain)
        {
            var now = DateTime.Now;

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
