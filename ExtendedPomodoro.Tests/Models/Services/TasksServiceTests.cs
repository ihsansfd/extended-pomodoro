using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Services;
using Moq.AutoMock;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Domains;
using FluentAssertions;

namespace ExtendedPomodoro.Tests.Models.Services
{
    public class TasksServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<ITasksRepository> _repositoryMock;
        private readonly TasksService _tasksService;

        public TasksServiceTests()
        {
            _mocker = new AutoMocker();
            _repositoryMock = _mocker.GetMock<ITasksRepository>();
            _tasksService = _mocker.CreateInstance<TasksService>();
        }

        [Fact]
        public async Task CreateTask_InsertedSuccesfully()
        {
            // Arrange
            var dbRows = new List<TaskDTO>();

            var createdAt = DateTime.Now;
            var updatedAt = DateTime.Now;

            _repositoryMock.Setup((x) => x.CreateTask(It.IsAny<CreateTaskDTO>()))
                   .Callback((CreateTaskDTO dto) =>
                   dbRows.Add(new()
                   {
                       Id = 1,
                       Name = dto.Name,
                       Description = dto.Description,
                       EstPomodoro = dto.EstPomodoro,
                       ActPomodoro = 0,
                       IsTaskCompleted = 0,
                       TimeSpentInSeconds = 0,
                       CreatedAt = createdAt = dto.CreatedAt,
                       UpdatedAt = updatedAt = dto.UpdatedAt
                   })
                   );

            // Act
            var param = new CreateTaskDomain("Doing Something", "Dizzy", 5);

            await _tasksService.CreateTask(param);

            // Assert
            Assert.Equal(param.Name, dbRows.First().Name);
            Assert.Equal(param.Description, dbRows.First().Description);
            Assert.Equal(param.EstPomodoro, dbRows.First().EstPomodoro);
            Assert.Equal(0, dbRows.First().ActPomodoro);
            Assert.Equal(0, dbRows.First().IsTaskCompleted);
            Assert.Equal(0, dbRows.First().TimeSpentInSeconds);
            Assert.Equal(createdAt, dbRows.First().CreatedAt);
            Assert.Equal(updatedAt, dbRows.First().UpdatedAt);
        }

        [Fact]
        public async Task UpdateTaskState_UpdatedSuccesfully()
        {
            // Arrange
            var dbRows = new List<TaskDTO>()
            {
                new()
                {
                    Id = 1,
                    IsTaskCompleted = 0
                },

                new()
                {
                    Id = 2,
                    IsTaskCompleted = 1
                },
            };

            _repositoryMock.Setup((x) => x.UpdateTaskState(It.IsAny<UpdateTaskStateDTO>()))
                .Callback((UpdateTaskStateDTO dto) =>
                dbRows.Where((row) => row.Id == dto.Id).First().IsTaskCompleted = dto.IsTaskCompleted);

            // Act
            int id1 = 1, id2 = 2;
            TaskState taskState1 = TaskState.COMPLETED, taskState2 = TaskState.IN_PROGRESS;

            await _tasksService.UpdateTaskState(id1, taskState1);
            await _tasksService.UpdateTaskState(id2, taskState2);

            // Assert
            Assert.Equal((int)taskState1, dbRows.Where((row) => row.Id == id1).First().IsTaskCompleted);
            Assert.Equal((int)taskState2, dbRows.Where((row) => row.Id == id2).First().IsTaskCompleted);

        }

        [Fact]
        public async Task UpdateTimeSpent_UpdatedSuccessfully()
        {
            // Arrange
            var dbRows = new List<TaskDTO>()
            {
                new()
                {
                    Id = 1,
                    TimeSpentInSeconds = 5
                },

                new()
                {
                    Id = 2,
                    TimeSpentInSeconds = 10
                },
            };

            var id1TimeSpentBeforeUpdate = dbRows.First().TimeSpentInSeconds;

            _repositoryMock.Setup((x) => x.UpdateTimeSpent(It.IsAny<UpdateTimeSpentDTO>()))
                .Callback((UpdateTimeSpentDTO dto) =>
                dbRows.Where((row) => row.Id == dto.Id).First().TimeSpentInSeconds 
                += dto.TimeSpentInSecondsIncrementBy);

            // Act
            var idParam = 1;
            var timeSpentParam = TimeSpan.FromMinutes(5);
            await _tasksService.UpdateTimeSpent(idParam, timeSpentParam);

            // Assert
            Assert.Equal((int)timeSpentParam.TotalSeconds + id1TimeSpentBeforeUpdate,
                dbRows.Where((row) => row.Id == idParam).First().TimeSpentInSeconds);
        }

        [Fact]
        public async Task UpdateActPomodoro_UpdatedSuccessfully()
        {
            // Arrange
            var dbRows = new List<TaskDTO>()
            {
                new()
                {
                    Id = 1,
                    ActPomodoro = 2
                },

                new()
                {
                    Id = 2,
                    ActPomodoro = 3
                },
            };

            var id1ActPomodoroBeforeUpdate = dbRows.First().ActPomodoro;

            _repositoryMock.Setup((x) => x.UpdateActPomodoro(It.IsAny<UpdateActPomodoroDTO>()))
                .Callback((UpdateActPomodoroDTO dto) =>
                dbRows.Where((row) => row.Id == dto.Id).First().ActPomodoro
                += dto.ActPomodoroIncrementBy);

            // Act
            var idParam = 1;
            var actPomodoroParam = 3;
            await _tasksService.UpdateActPomodoro(idParam, actPomodoroParam);

            // Assert
            Assert.Equal((int)actPomodoroParam + id1ActPomodoroBeforeUpdate,
                dbRows.Where((row) => row.Id == idParam).First().ActPomodoro);
        }

        [Fact]
        public async Task UpdateTask_UpdatedSuccessfully()
        {
            // Arrange
            var dbRows = new List<TaskDTO>()
            {
                new()
                {
                    Id = 1,
                    Name = "Task 1",
                    Description = "Mesmet",
                    IsTaskCompleted = 0,
                    EstPomodoro = 10,
                },
                 new()
                {
                    Id = 2,
                }
            };

            _repositoryMock.Setup((x) => x.UpdateTask(It.IsAny<UpdateTaskDTO>()))
                .Callback(
                (UpdateTaskDTO dto) => {
                    var matchedRow = dbRows.Where((row) => row.Id == dto.Id).First();
                    matchedRow.Name = dto.Name;
                    matchedRow.Description = dto.Description;
                    matchedRow.IsTaskCompleted = dto.IsTaskCompleted;
                    matchedRow.EstPomodoro = dto.EstPomodoro;
                    }
                    );

            // Act
            var param = new UpdateTaskDomain()
            {
                Id = 1,
                Name = "Task 1 Updated",
                Description = "Mesmet Updated",
                Taskstate = TaskState.COMPLETED,
                EstPomodoro = 5
            };

            await _tasksService.UpdateTask(param);

            // Assert
            var rowId1 = dbRows.Where((row) => row.Id == param.Id).First();

            Assert.Equal(param.Name, rowId1.Name);
            Assert.Equal(param.Description, rowId1.Description);
            Assert.Equal((int)param.Taskstate, rowId1.IsTaskCompleted);
            Assert.Equal(param.EstPomodoro, rowId1.EstPomodoro);
        }

        [Theory]
        [InlineData(TaskState.IN_PROGRESS, 1, 20, 2)]
        [InlineData(TaskState.IN_PROGRESS, 1, 1, 1)]
        [InlineData(TaskState.IN_PROGRESS, 2, 20, 0)]
        [InlineData(TaskState.IN_PROGRESS, 2, 1, 1)]
        [InlineData(TaskState.COMPLETED, 1, 10, 2)]
        [InlineData(TaskState.COMPLETED, 1, 1, 1)]
        [InlineData(TaskState.COMPLETED, 2, 10, 0)]
        [InlineData(TaskState.COMPLETED, 2, 1, 1)]
        public void GetTasks_ReturnResultPaginatedCorrectly(
            TaskState taskState, int page, int limit, int expectedCount)
        {
            // Arrange
            List<TaskDTO> dbRows = new List<TaskDTO>()
                {
                    new()
                    {
                        Id = 1,
                        Name = "Task 1",
                        IsTaskCompleted = 1,
                        CreatedAt = DateTime.Now.AddDays(-7)
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Task 2",
                        IsTaskCompleted = 1,
                        CreatedAt = DateTime.Now.AddDays(-6)
                    },
                    new()
                    {
                        Id = 3,
                        Name = "Task 3",
                        IsTaskCompleted = 0,
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                     new()
                    {
                        Id = 4,
                        Name = "Task 4",
                        IsTaskCompleted = 0,
                        CreatedAt = DateTime.Now.AddDays(-4)
                    },
                };

            _repositoryMock.Setup((x) => x.GetTasks(It.IsAny<GetTaskDTO>()))
                .ReturnsAsync(
                (GetTaskDTO dto) =>
                {
                    return dbRows
                    .Where(x => x.IsTaskCompleted == dto.IsTaskCompleted)
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(dto.Offset)
                    .Take(dto.Limit);
                });

            // Act
            var records = _tasksService.GetTasks(taskState, page, limit).ToBlockingEnumerable();

            // Assert
            var shouldHaveExpectedCount = records.Should().HaveCount(expectedCount);

            if (records.Any())
            {
                shouldHaveExpectedCount.And
                 .AllSatisfy((rec) => rec.TaskState.Should().Be(taskState)).And
                .BeInDescendingOrder((rec) => rec.CreatedAt);
            }
        }

        [Theory]
        [InlineData(TaskState.IN_PROGRESS, 20, 1)]
        [InlineData(TaskState.IN_PROGRESS, 2, 2)]
        [InlineData(TaskState.IN_PROGRESS, 1, 3)]
        [InlineData(TaskState.IN_PROGRESS, 0, 0)]
        [InlineData(TaskState.COMPLETED, 20, 1)]
        [InlineData(TaskState.COMPLETED, 2, 2)]
        [InlineData(TaskState.COMPLETED, 1, 3)]
        [InlineData(TaskState.COMPLETED, 0, 0)]
        public async Task GetTotalPages_ReturnCorrectTotalPages(
            TaskState taskState, int limit, int expectedTotalPages)
        {
            // Arrange
            List<TaskDTO> dbRows = new List<TaskDTO>()
                {
                    new() { IsTaskCompleted = 1 },
                    new() { IsTaskCompleted = 1 },
                    new() { IsTaskCompleted = 1 },
                    new() { IsTaskCompleted = 0 },
                    new() { IsTaskCompleted = 0 },
                    new() { IsTaskCompleted = 0 },
                };

            _repositoryMock.Setup((x) => x.GetTotalRows(It.IsAny<int>()))
                .ReturnsAsync((int isTaskCompleted) =>
                dbRows.Where((row) => row.IsTaskCompleted == isTaskCompleted).Count());

            // Act
            var res = await _tasksService.GetTotalPages(taskState, limit);

            // Assert
            Assert.Equal(expectedTotalPages, res);
        }
    }

}
