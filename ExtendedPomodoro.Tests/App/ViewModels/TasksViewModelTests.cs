using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Services.Interfaces;
using ExtendedPomodoro.ViewModels;
using FluentAssertions.Extensions;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.ViewModels
{
    public class ReadTasksViewModelTests
    {
        private readonly AutoMocker _mocker = new();

        private readonly Mock<ITasksService> _tasksServiceMock;
        private readonly Mock<IMessenger> _messengerMock;
        private readonly ReadTasksViewModel _sut;

        public ReadTasksViewModelTests()
        {
            _tasksServiceMock = _mocker.GetMock<ITasksService>();
            _messengerMock = _mocker.GetMock<IMessenger>();
            _sut = _mocker.CreateInstance<ReadTasksViewModel>();
        }

        [Theory]
        [InlineData(TaskState.IN_PROGRESS, 1, false)]
        [InlineData(TaskState.COMPLETED, 1, false)]
        [InlineData(TaskState.IN_PROGRESS, 2, true)]
        [InlineData(TaskState.COMPLETED, 2, true)]
        public async Task LoadTasks_Successfully(
            TaskState taskStateInput, int returnedTotalPages, bool expectedMoreTasks)
        {
            // Arrange
            List<TaskDomain> tasksRows = GenerateTasksRow();

            _tasksServiceMock.Setup((x) =>
                x.GetTotalPages(It.IsAny<TaskState>(), It.IsAny<int>())).ReturnsAsync(returnedTotalPages);

            _tasksServiceMock.Setup((x) =>
                    x.GetTasks(It.IsAny<TaskState>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() => tasksRows.Where((x) => x.TaskState == taskStateInput)
                    .ToAsyncEnumerable());

            // Act
            _sut.IsDisplayingCompletedTasks = taskStateInput == TaskState.COMPLETED;
            await _sut.LoadTasks();

            // Assert
            Assert.Equal(expectedMoreTasks, _sut.AreThereMoreTasks);
            Assert.Equal(tasksRows.Count(x => x.TaskState == taskStateInput), _sut.Tasks.Count);
            Assert.All(_sut.Tasks, (x) =>
                Assert.Equal((int)taskStateInput, x.TaskStatus));
        }

        [Fact]
        public async Task DisplayCompletedTasksCommand_Successfully()
        {
            // Arrange
            List<TaskDomain> tasksRows = GenerateTasksRow();

            _tasksServiceMock.Setup((x) =>
                x.GetTotalPages(It.IsAny<TaskState>(), It.IsAny<int>())).ReturnsAsync(1);

            _tasksServiceMock.Setup((x) =>
                    x.GetTasks(It.IsAny<TaskState>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() => tasksRows.Where((x) => x.TaskState == TaskState.COMPLETED)
                    .ToAsyncEnumerable());

            // Act
            await _sut.DisplayCompletedTasksCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(tasksRows.Count(x => x.TaskState == TaskState.COMPLETED), _sut.Tasks.Count);
            Assert.All(_sut.Tasks, (x) =>
                Assert.Equal((int)TaskState.COMPLETED, x.TaskStatus));
        }

        [Fact]
        public async Task DisplayInProgressTasksCommand_Successfully()
        {
            // Arrange
            List<TaskDomain> tasksRows = GenerateTasksRow();

            _tasksServiceMock.Setup((x) =>
                x.GetTotalPages(It.IsAny<TaskState>(), It.IsAny<int>())).ReturnsAsync(1);

            _tasksServiceMock.Setup((x) =>
                    x.GetTasks(It.IsAny<TaskState>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() => tasksRows.Where((x) => x.TaskState == TaskState.IN_PROGRESS)
                    .ToAsyncEnumerable());

            // Act
            await _sut.DisplayInProgressTasksCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(tasksRows.Count(x => x.TaskState == TaskState.IN_PROGRESS), _sut.Tasks.Count);
            Assert.All(_sut.Tasks, (x) =>
                Assert.Equal((int)TaskState.IN_PROGRESS, x.TaskStatus));
        }

        [Theory]
        [InlineData(false, 4, TaskState.IN_PROGRESS, 2, 1)]
        [InlineData(false, 6, TaskState.IN_PROGRESS, 3, 2)]
        [InlineData(true, 4, TaskState.IN_PROGRESS, 3, 1)]
        [InlineData(false, 4, TaskState.COMPLETED, 2, 1)]
        [InlineData(false, 6, TaskState.COMPLETED, 3, 2)]
        [InlineData(true, 4, TaskState.COMPLETED, 3, 1)]
        public void DisplayMoreTasksCommand_WhenThereAreMorePages_DisplayIt(
            bool expectedMoreTasks, 
            int expectedTotalTasks,
            TaskState taskStateInput,
            int returnedTotalPages,
            int executedFor
            )
        {
            // Arrange
            List<TaskDomain> tasksRows = GenerateTasksRow();

            _tasksServiceMock.Setup((x) =>
                x.GetTotalPages(It.IsAny<TaskState>(), It.IsAny<int>())).ReturnsAsync(returnedTotalPages);

            _tasksServiceMock.Setup((x) =>
                    x.GetTasks(It.IsAny<TaskState>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(() => tasksRows.Where((x) => x.TaskState == taskStateInput)
                    .ToAsyncEnumerable());

            // Act
            _sut.DisplayInProgressTasksCommand.ExecuteAsync(null);

            for (int i = 0; i < executedFor; i++)
            {
                _sut.DisplayMoreTasksCommand.ExecuteAsync(null);
            }
            // Assert
            Assert.Equal(expectedMoreTasks, _sut.AreThereMoreTasks);
            Assert.Equal(expectedTotalTasks, _sut.Tasks.Count);
            Assert.All(_sut.Tasks, (x) =>
                Assert.Equal((int)taskStateInput, x.TaskStatus));
        }

        #region Helpers

        private List<TaskDomain> GenerateTasksRow()
        {
            return new List<TaskDomain>
            {
                new TaskDomain()
                {
                    Id = 1,
                    Name = "Task 1",
                    TaskState = TaskState.IN_PROGRESS,
                },
                new TaskDomain()
                {
                    Id = 2,
                    Name = "Task 2",
                    TaskState = TaskState.IN_PROGRESS,
                },
                new TaskDomain()
                {
                    Id = 3,
                    Name = "Task 3",
                    TaskState = TaskState.COMPLETED,
                },
                new TaskDomain()
                {
                    Id = 4,
                    Name = "Task 4",
                    TaskState = TaskState.COMPLETED,
                }
            };
        }

        #endregion
    }

    public class CreateTaskViewModelTests
    {
        private readonly AutoMocker _mocker = new();

        private readonly Mock<ITasksService> _tasksServiceMock;
        private readonly Mock<IMessenger> _messengerMock;
        private readonly Mock<IMessageBoxService> _messageBoxServiceMock;
        private readonly CreateTaskViewModel _sut;

        public CreateTaskViewModelTests()
        {
            _tasksServiceMock = _mocker.GetMock<ITasksService>();
            _messageBoxServiceMock = _mocker.GetMock<IMessageBoxService>();
            _messengerMock = _mocker.GetMock<IMessenger>();
            _sut = _mocker.CreateInstance<CreateTaskViewModel>();
        }

        [Fact]
        public void OpenModal_Successfully()
        {
            _sut.OpenModalCommand.Execute(null);

            Assert.True(_sut.IsModalShown);
        }

        [Fact]
        public void CloseModal_Successfully()
        {
            _sut.CloseModalCommand.Execute(null);

            Assert.False(_sut.IsModalShown);
        }

        [Fact]
        public async Task CreateTask_WhenDataValid_CreatedSuccessfully()
        {
            // Arrange
            bool? isCreationSuccess = null;

            _messengerMock.Setup((x) => x.Send(It.IsAny<TaskCreationInfoMessage>(), It.IsAny<IsAnyToken>()))
                .Callback((object val, object _) => isCreationSuccess = ((TaskCreationInfoMessage)val).IsTaskCreationSuccess);
            _tasksServiceMock.Setup((x) => x.CreateTask(It.IsAny<CreateTaskDomain>()));

            // Act
            _sut.Name = "Name";
            _sut.Description = "Description";
            _sut.EstPomodoro = 2.ToString();

            _sut.OpenModalCommand.Execute(null);
            await _sut.CreateTaskCommand.ExecuteAsync(null);

            // Assert
            Assert.False(_sut.IsModalShown);
            Assert.True(isCreationSuccess);
            Assert.True(string.IsNullOrEmpty(_sut.Name));
            Assert.True(string.IsNullOrEmpty(_sut.Description));
            Assert.True(string.IsNullOrEmpty(_sut.EstPomodoro));
            _tasksServiceMock.Verify((x) => x.CreateTask(It.IsAny<CreateTaskDomain>()), Times.Once);
        }

        [Theory]
        [InlineData("", "Desc", "0")]
        [InlineData(null, null, null)]
        [InlineData(null, "Desc", "0")]
        [InlineData("Hai", "Desc", "wrong type")]
        public async Task CreateTask_WhenDataInvalid_Failed(string nameInput, string descInput, string estPomodoroInput)
        {
            // Arrange
            _tasksServiceMock.Setup((x) => x.CreateTask(It.IsAny<CreateTaskDomain>()));

            // Act
            _sut.Name = nameInput;
            _sut.Description = descInput;
            _sut.EstPomodoro = estPomodoroInput;

            _sut.OpenModalCommand.Execute(null);
            await _sut.CreateTaskCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_sut.IsModalShown);
            Assert.Equal(nameInput, _sut.Name);
            Assert.Equal(descInput, _sut.Description);
            Assert.Equal(estPomodoroInput, _sut.EstPomodoro);
            _tasksServiceMock.Verify((x) => x.CreateTask(It.IsAny<CreateTaskDomain>()), Times.Never);
        }
    }

    public class UpdateTaskViewModelTests
    {
        private readonly AutoMocker _mocker = new();
        private readonly Mock<ITasksService> _tasksServiceMock;
        private readonly Mock<IMessenger> _messengerMock;
        private readonly Mock<IMessageBoxService> _messageBoxServiceMock;
        private readonly UpdateTaskViewModel _sut;

        public UpdateTaskViewModelTests()
        {
            _tasksServiceMock = _mocker.GetMock<ITasksService>();
            _messageBoxServiceMock = _mocker.GetMock<IMessageBoxService>();
            _messengerMock = _mocker.GetMock<IMessenger>();
            _sut = _mocker.CreateInstance<UpdateTaskViewModel>();
        }

        [Fact]
        public async Task UpdateTask_WhenDataValid_UpdatedSuccessfully()
        {
            // Arrange
            bool? isUpdationSuccess = null;

            _tasksServiceMock.Setup((x) => x.UpdateTask(It.IsAny<UpdateTaskDomain>()));
            _messengerMock.Setup((x) => x.Send(It.IsAny<TaskUpdateInfoMessage>(), It.IsAny<IsAnyToken>()))
                .Callback((object val, object _) => isUpdationSuccess = ((TaskUpdateInfoMessage)val).IsTaskUpdateSuccess);
            _sut.IsModalShown = true;

            // Act
            _sut.Id = 5;
            _sut.Name = "Name";
            _sut.Description = "Description";
            _sut.ActPomodoro = 3;
            _sut.TaskStatus = (int)TaskState.COMPLETED;

            await _sut.UpdateTaskCommand.ExecuteAsync(null);

            // Assert
            Assert.True(isUpdationSuccess);
            Assert.False(_sut.IsModalShown);
            _tasksServiceMock.Verify((x) => x.UpdateTask(It.IsAny<UpdateTaskDomain>()), Times.Once());

        }

        [Theory]
        [InlineData(5, "", "", "1", TaskState.COMPLETED)]
        [InlineData(5, null, "", "1", TaskState.COMPLETED)]
        [InlineData(5, "Correct", "", "wrong type", TaskState.COMPLETED)]
        [InlineData(5, null, null, null, TaskState.COMPLETED)]
        public async Task UpdateTask_WhenDataInvalid_UpdatedSuccessfully(
            int idInput, string nameInput, string descInput, string estPomodoroInput, TaskState taskStateInput)
        {
            // Arrange

            _tasksServiceMock.Setup((x) => x.UpdateTask(It.IsAny<UpdateTaskDomain>()));
            _sut.IsModalShown = true;

            // Act
            _sut.Id = idInput;
            _sut.Name = nameInput;
            _sut.Description = descInput;
            _sut.EstPomodoro = estPomodoroInput;
            _sut.TaskStatus = (int)taskStateInput;

            await _sut.UpdateTaskCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_sut.IsModalShown);
            _tasksServiceMock.Verify((x) => x.UpdateTask(It.IsAny<UpdateTaskDomain>()), Times.Never());
        }

        [Fact]
        public async Task UpdateTaskState_Successfully()
        {
            // Arrange
            _messageBoxServiceMock.Setup((x) =>
                x.Show(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MessageBoxButton>(),
                    It.IsAny<MessageBoxImage>())).Returns(MessageBoxResult.Yes);
            _tasksServiceMock.Setup((x) => x.UpdateTask(It.IsAny<UpdateTaskDomain>()));

            bool? isUpdateSuccess = null;

            _messengerMock.Setup((x) => x.Send(It.IsAny<TaskUpdateStateInfoMessage>(), It.IsAny<IsAnyToken>()))
                .Callback((object val, object _) => isUpdateSuccess = ((TaskUpdateStateInfoMessage)val).IsTaskUpdateSuccess);

            // Act
            var updateTaskStateDomainViewModel = _mocker.GetMock<IUpdateTaskStateDomainViewModel>().Object;
            updateTaskStateDomainViewModel.TaskId = 5;
            updateTaskStateDomainViewModel.IntendedTaskState = (int)TaskState.COMPLETED;
            await _sut.UpdateTaskStateCommand.ExecuteAsync(updateTaskStateDomainViewModel);

            // Assert
            Assert.True(isUpdateSuccess);
            _tasksServiceMock.Verify((x) => 
                x.UpdateTaskState(It.IsAny<int>(), It.IsAny<TaskState>()), Times.Once());
            
        }

        [Fact]
        public void LoadTaskDetail_Successfully()
        {
            // Act
            var now = DateTime.Now.ToString();
            _sut.LoadTaskDetailCommand.Execute(new TaskDomainViewModel()
            {
                Id = 5,
                Name = "Task",
                ActPomodoro = 5,
                CreatedAt = now,
                Description = "Desc",
                EstPomodoro = 3,
                TaskStatus = 0,
                TimeSpentInMinutes = 30
            });

            // Assert
            Assert.Equal(5, _sut.Id);
            Assert.Equal("Task", _sut.Name);
            Assert.Equal(5, _sut.ActPomodoro);
            Assert.Equal("Desc", _sut.Description);
            Assert.Equal(3.ToString(), _sut.EstPomodoro);
            Assert.Equal(0, _sut.TaskStatus);
            Assert.Equal(30, _sut.TimeSpentInMinutes);
            Assert.True(_sut.IsModalShown);

        }

    }
    public class DeleteTaskViewModelTests
    {
        private readonly AutoMocker _mocker = new();

        private readonly Mock<ITasksService> _tasksServiceMock;
        private readonly Mock<IMessenger> _messengerMock;
        private readonly Mock<IMessageBoxService> _messageBoxServiceMock;
        private readonly DeleteTaskViewModel _sut;

        public DeleteTaskViewModelTests()
        {
            _tasksServiceMock = _mocker.GetMock<ITasksService>();
            _messageBoxServiceMock = _mocker.GetMock<IMessageBoxService>();
            _messengerMock = _mocker.GetMock<IMessenger>();
            _sut = _mocker.CreateInstance<DeleteTaskViewModel>();
        }

        [Fact]
        public async Task DeleteTaskCommand_Successfully()
        {
            // Assert
            bool? isDeletionSuccess = null;

            _messengerMock.Setup((x) => x.Send(It.IsAny<TaskDeletionInfoMessage>(), It.IsAny<IsAnyToken>()))
                .Callback((object val, object _) => isDeletionSuccess = ((TaskDeletionInfoMessage)val).IsTaskDeletionSuccess);

            _messageBoxServiceMock.Setup((x) =>
                x.Show(It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<MessageBoxButton>(),
                    It.IsAny<MessageBoxImage>())).Returns(MessageBoxResult.Yes);

            _tasksServiceMock.Setup((x) => x.DeleteTask(It.IsAny<int>()));


            // Act
            await _sut.DeleteTaskCommand.ExecuteAsync(2);

            // Verify
            Assert.True(isDeletionSuccess);
            _tasksServiceMock.Verify((x) => x.DeleteTask(It.IsAny<int>()), Times.Once);

        }
    }


}
