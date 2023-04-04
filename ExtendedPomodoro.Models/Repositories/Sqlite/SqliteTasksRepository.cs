using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories.Sqlite
{
    public class SqliteTasksRepository : ITasksRepository
    {
        private readonly SqliteDbConnectionFactory _connectionFactory;

        private const string CREATE_TASK_QUERY =
            @"INSERT INTO tblTasks (Name, Description, EstPomodoro, CreatedAt, UpdatedAt)
              VALUES (@Name, @Description, @EstPomodoro, @CreatedAt, @UpdatedAt)";

        private const string GET_TASKS_QUERY = @"SELECT * FROM tblTasks ORDER BY datetime(CreatedAt) DESC LIMIT {0}";

        public SqliteTasksRepository(SqliteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async IAsyncEnumerable<TaskDomain> GetTasks(int limit = 20)
        {
            var query = string.Format(GET_TASKS_QUERY, limit);

            using (var db = _connectionFactory.Connect())
            {
                IEnumerable<TaskDTO> records = await db.QueryAsync<TaskDTO>(query);

                foreach(var record in records) yield return ConvertToTaskDomain(record);
            }
        }

        public async Task CreateTask(CreateTaskDomain task)
        {
            using(var db = _connectionFactory.Connect())
            {
                var now = DateTime.Now;

                object data = new
                {
                    Name = task.Name,
                    Description = task.Description,
                    EstPomodoro = task.EstPomodoro,
                    CreatedAt = now,
                    UpdatedAt = now,
                };

                await db.ExecuteAsync(CREATE_TASK_QUERY, data);
            }
        }

        private static TaskDomain ConvertToTaskDomain(TaskDTO taskDTO)
        {
            return new(
                    taskDTO.Id,
                    taskDTO.Name,
                    taskDTO.Description,
                    taskDTO.EstPomodoro,
                    taskDTO.ActPomodoro,
                    taskDTO.CreatedAt,
                    taskDTO.UpdatedAt,
                    taskDTO.IsTaskCompleted == 1 ? TaskState.COMPLETED : TaskState.IN_PROGRESS,
                    TimeSpan.FromSeconds(taskDTO.TimeSpent)
                );
        }

    }
}
