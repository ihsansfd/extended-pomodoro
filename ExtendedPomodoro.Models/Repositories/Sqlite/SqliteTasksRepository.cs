using Dapper;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExtendedPomodoro.Models.Repositories.Sqlite
{
    public class SqliteTasksRepository : ITasksRepository
    {

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

        private readonly IDbConnectionFactory _connectionFactory;

        public SqliteTasksRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TaskDTO>> GetTasks(GetTaskDTO dto)
        {
            using (var db = _connectionFactory.Connect())
            {
                return await db.QueryAsync<TaskDTO>(GET_TASKS_QUERY, dto);
            }
        }

        public async Task<int> GetTotalRows(int IsTaskCompleted)
        {
            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    IsTaskCompleted = IsTaskCompleted,
                };

                return await db.ExecuteScalarAsync<int>(GET_TOTAL_PAGES_QUERY, data);

            }
        }

        public async Task CreateTask(CreateTaskDTO dto)
        {
            using (var db = _connectionFactory.Connect())
            {
                await db.ExecuteAsync(CREATE_TASK_QUERY, dto);
            }
        }

        public async Task UpdateTaskState(int taskId, int IsTaskCompleted)
        {
            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    Id = taskId,
                    IsTaskCompleted = IsTaskCompleted
                };

                await db.ExecuteAsync(UPDATE_TASK_STATE_QUERY, data);
            }
        }

        public async Task UpdateTimeSpent(int taskId, int timeSpentInSeconds)
        {
            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    Id = taskId,
                    TimeSpentInSecondsIncrementBy = timeSpentInSeconds
                };

                await db.ExecuteAsync(UPDATE_TASK_TIME_SPENT_QUERY, data);
            }
        }

        public async Task UpdateTask(UpdateTaskDTO dto)
        {
            using (var db = _connectionFactory.Connect())
            {
                await db.ExecuteAsync(UPDATE_TASK_QUERY, dto);
            }
        }

        public async Task UpdateActPomodoro(UpdateActPomodoroDTO dto)
        {
            using (var db = _connectionFactory.Connect())
            {
                await db.ExecuteAsync(UPDATE_TASK_ACT_POMODORO_QUERY, dto);
            }
        }

        public async Task DeleteTask(int taskId)
        {
            using (var db = _connectionFactory.Connect())
            {
                await db.ExecuteAsync(DELETE_TASK_QUERY, new { Id = taskId });
            }
        }
    }
}
