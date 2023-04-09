﻿using Dapper;
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

        private const string UPDATE_TASK_QUERY =
            @"UPDATE tblTasks SET Name = @Name, Description = @Description, EstPomodoro = @EstPomodoro, 
            IsTaskCompleted = @IsTaskCompleted WHERE Id = @Id";

        private const string UPDATE_TASK_STATE_QUERY =
            @"UPDATE tblTasks SET IsTaskCompleted = @IsTaskCompleted WHERE Id = @Id";

        private const string GET_TASKS_QUERY =
            @"SELECT * FROM tblTasks WHERE IsTaskCompleted = @IsTaskCompleted
             ORDER BY datetime(CreatedAt) DESC LIMIT @Limit OFFSET @Offset";

        private const string GET_TOTAL_PAGES_QUERY =
            @"SELECT COUNT(*) FROM tblTasks WHERE IsTaskCompleted = @IsTaskCompleted";

        private const string DELETE_TASK_QUERY =
            @"DELETE FROM tblTasks WHERE Id = @Id";

        public SqliteTasksRepository(SqliteDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async IAsyncEnumerable<TaskDomain> GetTasks(TaskState taskState = TaskState.IN_PROGRESS, int page = 1, int limit = 20)
        {

            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    IsTaskCompleted = ConvertTaskStateToInt(taskState),
                    Limit = limit,
                    Offset = limit * (page - 1)
                };

                IEnumerable<SqlTaskDTO> records = await db.QueryAsync<SqlTaskDTO>(GET_TASKS_QUERY, data);

                foreach(var record in records) yield return ConvertToTaskDomain(record);
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

        public async Task UpdateTask(UpdateTaskDomain task)
        {
            using (var db = _connectionFactory.Connect())
            {
                object data = new
                {
                    Id = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    EstPomodoro = task.EstPomodoro,
                    IsTaskCompleted = ConvertTaskStateToInt(task.Taskstate),
                    UpdatedAt = DateTime.Now,
                };

                await db.ExecuteAsync(UPDATE_TASK_QUERY, data);
            }
        }

        public async Task DeleteTask(int taskId)
        {
            using(var db = _connectionFactory.Connect())
            {

                await db.ExecuteAsync(DELETE_TASK_QUERY, new { Id = taskId });
            }
        }

        private static TaskDomain ConvertToTaskDomain(SqlTaskDTO taskDTO)
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

        private static int ConvertTaskStateToInt(TaskState taskState)
        {
            return taskState == TaskState.COMPLETED ? 1 : 0;
        }
    }
}
