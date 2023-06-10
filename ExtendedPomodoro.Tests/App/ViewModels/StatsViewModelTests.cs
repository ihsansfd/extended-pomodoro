using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExtendedPomodoro.Core.Extensions;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.ViewServices.Interfaces;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.ViewModels
{

    // TODO: UNTESTED ASSESSMENT, TASKDOMAINS

    public class StatsViewModelTests
    {
        private readonly AutoMocker _mocker = new AutoMocker();
        private readonly Mock<IDailySessionsService> _dailySessionsServiceMock;
        private readonly Mock<IStatsViewService> _statsViewServiceMock;
        private readonly Mock<ITasksService> _tasksServiceMock;
        private readonly StatsViewModel _sut;

        public StatsViewModelTests()
        {
            _dailySessionsServiceMock = _mocker.GetMock<IDailySessionsService>();
            _statsViewServiceMock = _mocker.GetMock<IStatsViewService>();
            _tasksServiceMock = _mocker.GetMock<ITasksService>();
            _sut = _mocker.CreateInstance<StatsViewModel>();
        }

        [Theory]
        [ClassData(typeof(StatsData))]
        void GenerateStatsCommand_LoadChartAndProps(
            StatsValue statsValueToDisplay,
            int expectedXAxisCount,
            int expectedYAxisCount,
            bool expectedEventCount,
            bool expectedDisplayChart,
            DailySessionDomain[] dailySessionDomains,
            TaskDomain[] taskDomains
        )
        {
            // Arrange
            GenerateSetupFrom(dailySessionDomains, taskDomains);

            // Act
            double[] itsXAxis = null!;
            double[] itsYAxis = null!;
            bool eventIsInvoked = false;

            void SutOnNewStatsAxes(object? sender, ChartDataDomainViewModel e)
            {
                eventIsInvoked = true;
                itsXAxis = e.XAxis;
                itsYAxis = e.YAxis;
            }

            _sut.NewChartData += SutOnNewStatsAxes;
            _sut.GenerateStatsCommand.Execute(null);
            _sut.StatsValueToDisplay = (int)statsValueToDisplay;

            // Assert
            Assert.Equal(expectedEventCount, eventIsInvoked);
            Assert.Equal(expectedDisplayChart, _sut.DisplayChart);
            Assert.Equal(expectedXAxisCount, itsXAxis?.Length ?? 0);
            Assert.Equal(expectedYAxisCount, itsYAxis?.Length ?? 0);
            Assert.Equal(dailySessionDomains.Sum((x) => x.TotalPomodoroCompleted), 
                _sut.TotalPomodoroCompleted);
            Assert.Equal(dailySessionDomains.Sum((x) => x.TotalShortBreaksCompleted),
                _sut.TotalShortBreaksCompleted);
            Assert.Equal(dailySessionDomains.Sum((x) => x.TotalLongBreaksCompleted),
                _sut.TotalLongBreaksCompleted);
            Assert.Equal(dailySessionDomains.Sum((x) => x.TotalTasksCompleted),
                _sut.TotalTasksCompleted);
            Assert.Equal(dailySessionDomains.Sum((x) => x.TimeSpent.TotalMinutes),
                _sut.TotalTimeSpentInMinutes);
        }

        [Fact]
        async Task Load_Successfully()
        {
            // Assert
            GenerateSetupFrom(new[]
            {
                new DailySessionDomain()
                {
                    SessionDate = new DateOnly(2020, 8, 15),
                    TimeSpent = TimeSpan.FromMinutes(10),
                    TotalPomodoroCompleted = 5,
                    TotalShortBreaksCompleted = 10,
                    TotalLongBreaksCompleted = 15,
                    TotalTasksCompleted = 5,
                    CreatedAt = new DateOnly(2020, 8, 15).ToDateTime(new TimeOnly(5, 5, 5)),
                    UpdatedAt = new DateOnly(2020, 8, 15).ToDateTime(new TimeOnly(5, 5, 5)),
                },
                new DailySessionDomain()
                {
                    SessionDate = new DateOnly(2020, 8, 18),
                    TimeSpent = TimeSpan.FromMinutes(15),
                    TotalPomodoroCompleted = 1,
                    TotalShortBreaksCompleted = 2,
                    TotalLongBreaksCompleted = 3,
                    TotalTasksCompleted = 4,
                    CreatedAt = new DateOnly(2020, 8, 18).ToDateTime(new TimeOnly(5, 5, 5)),
                    UpdatedAt = new DateOnly(2020, 8, 18).ToDateTime(new TimeOnly(5, 5, 5)),
                },
                new DailySessionDomain()
                {
                    SessionDate = new DateOnly(2020, 8, 20),
                    TimeSpent = TimeSpan.FromMinutes(3),
                    TotalPomodoroCompleted = 1,
                    TotalShortBreaksCompleted = 2,
                    TotalLongBreaksCompleted = 5,
                    TotalTasksCompleted = 0,
                    CreatedAt = new DateOnly(2020, 8, 20).ToDateTime(new TimeOnly(5, 5, 5)),
                    UpdatedAt = new DateOnly(2020, 8, 20).ToDateTime(new TimeOnly(5, 5, 5)),
                }
            },
                new[]
                {
                    new TaskDomain()
                    {
                        Name = "Task 1",
                        ActPomodoro = 2,
                        EstPomodoro = 3,
                        TaskState = TaskState.COMPLETED,
                        CompletedAt = new DateTime(2019, 1, 1),
                        CreatedAt = new DateTime(2019, 1, 1),
                        UpdatedAt =  new DateTime(2019, 1, 1),
                    },

                    new TaskDomain()
                    {
                        Name = "Task 2",
                        ActPomodoro = 2,
                        EstPomodoro = 3,
                        TaskState = TaskState.IN_PROGRESS,
                        CreatedAt = new DateTime(2019, 1, 1),
                        UpdatedAt =  new DateTime(2019, 1, 1),
                    }
                }
            
            );

            await _sut.LoadCommand.ExecuteAsync(null);

            Assert.Equal(DateTime.Today.AddDays(-7).ToMinTime(), _sut.FromDate);
            Assert.Equal(DateTime.Today.ToMaxTime(), _sut.ToDate);
            Assert.Equal(DateTime.Today.AddDays(-5), _sut.MinDate);
            Assert.Equal(DateTime.Today, _sut.MaxDate);

            Assert.NotEmpty(_sut.ChartData.XAxis);
            Assert.NotEmpty(_sut.ChartData.YAxis);
        }

        #region Helpers
        private void GenerateSetupFrom(
            IEnumerable<DailySessionDomain> dailySessionDomains, IEnumerable<TaskDomain> taskDomains)
        {
            _dailySessionsServiceMock.Setup((x) =>
                    x.GetDailySessions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(dailySessionDomains.ToAsyncEnumerable());

            _tasksServiceMock.Setup((x) =>
                    x.GetTasks(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(taskDomains.ToAsyncEnumerable());

            _dailySessionsServiceMock.Setup((x) =>
                    x.GetDateRangeDailySessions())
                .ReturnsAsync(new DateRangeDailySessionsDomain(
                    DateTime.Today.AddDays(-5), DateTime.Today));
        }
        #endregion

        #region Test Data

        public class StatsData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    StatsValue.POMODORO_COMPLETED,
                    3,
                    3,
                    true,
                    true,
                    new []
                    {
                        new DailySessionDomain()
                        {
                            SessionDate = new DateOnly(2020, 8, 15),
                            TimeSpent = TimeSpan.FromMinutes(10),
                            TotalPomodoroCompleted = 5,
                            TotalShortBreaksCompleted = 10,
                            TotalLongBreaksCompleted = 15,
                            TotalTasksCompleted = 5,
                            CreatedAt = new DateOnly(2020, 8, 15).ToDateTime(new TimeOnly(5, 5, 5)),
                            UpdatedAt = new DateOnly(2020, 8, 15).ToDateTime(new TimeOnly(5, 5, 5)),
                        },
                        new DailySessionDomain()
                        {
                            SessionDate = new DateOnly(2020, 8, 18),
                            TimeSpent = TimeSpan.FromMinutes(15),
                            TotalPomodoroCompleted = 1,
                            TotalShortBreaksCompleted = 2,
                            TotalLongBreaksCompleted = 3,
                            TotalTasksCompleted = 4,
                            CreatedAt = new DateOnly(2020, 8, 18).ToDateTime(new TimeOnly(5, 5, 5)),
                            UpdatedAt = new DateOnly(2020, 8, 18).ToDateTime(new TimeOnly(5, 5, 5)),
                        },
                        new DailySessionDomain()
                        {
                            SessionDate = new DateOnly(2020, 8, 20),
                            TimeSpent = TimeSpan.FromMinutes(3),
                            TotalPomodoroCompleted = 1,
                            TotalShortBreaksCompleted = 2,
                            TotalLongBreaksCompleted = 5,
                            TotalTasksCompleted = 0,
                            CreatedAt = new DateOnly(2020, 8, 20).ToDateTime(new TimeOnly(5, 5, 5)),
                            UpdatedAt = new DateOnly(2020, 8, 20).ToDateTime(new TimeOnly(5, 5, 5)),
                        }
                    },

                    new[]
                    {
                        new TaskDomain()
                        {
                            Name = "Task 1",
                            ActPomodoro = 2,
                            EstPomodoro = 3,
                            TaskState = TaskState.COMPLETED,
                            CompletedAt = new DateTime(2019, 1, 1),
                            CreatedAt = new DateTime(2019, 1, 1),
                            UpdatedAt =  new DateTime(2019, 1, 1),
                        },

                        new TaskDomain()
                        {
                            Name = "Task 2",
                            ActPomodoro = 2,
                            EstPomodoro = 3,
                            TaskState = TaskState.IN_PROGRESS,
                            CreatedAt = new DateTime(2019, 1, 1),
                            UpdatedAt =  new DateTime(2019, 1, 1),
                        }
                    }
                };

                yield return new object[]
                {
                    StatsValue.SHORT_BREAKS_COMPLETED,
                    3,
                    3,
                    true,
                    true,
                     new []
                    {
                        new DailySessionDomain()
                        {
                            SessionDate = new DateOnly(2020, 8, 15),
                            TimeSpent = TimeSpan.FromMinutes(10),
                            TotalPomodoroCompleted = 5,
                            TotalShortBreaksCompleted = 0,
                            TotalLongBreaksCompleted = 15,
                            TotalTasksCompleted = 5,
                            CreatedAt = new DateOnly(2020, 8, 15).ToDateTime(new TimeOnly(5, 5, 5)),
                            UpdatedAt = new DateOnly(2020, 8, 15).ToDateTime(new TimeOnly(5, 5, 5)),
                        },
                        new DailySessionDomain()
                        {
                            SessionDate = new DateOnly(2020, 8, 18),
                            TimeSpent = TimeSpan.FromMinutes(15),
                            TotalPomodoroCompleted = 1,
                            TotalShortBreaksCompleted = 0,
                            TotalLongBreaksCompleted = 3,
                            TotalTasksCompleted = 4,
                            CreatedAt = new DateOnly(2020, 8, 18).ToDateTime(new TimeOnly(5, 5, 5)),
                            UpdatedAt = new DateOnly(2020, 8, 18).ToDateTime(new TimeOnly(5, 5, 5)),
                        },
                        new DailySessionDomain()
                        {
                            SessionDate = new DateOnly(2020, 8, 20),
                            TimeSpent = TimeSpan.FromMinutes(3),
                            TotalPomodoroCompleted = 1,
                            TotalShortBreaksCompleted = 0,
                            TotalLongBreaksCompleted = 5,
                            TotalTasksCompleted = 0,
                            CreatedAt = new DateOnly(2020, 8, 20).ToDateTime(new TimeOnly(5, 5, 5)),
                            UpdatedAt = new DateOnly(2020, 8, 20).ToDateTime(new TimeOnly(5, 5, 5)),
                        }
                    },
                     new[]
                     {
                         new TaskDomain()
                         {
                             Name = "Task 1",
                             ActPomodoro = 2,
                             EstPomodoro = 3,
                             TaskState = TaskState.COMPLETED,
                             CompletedAt = new DateTime(2019, 1, 1),
                             CreatedAt = new DateTime(2019, 1, 1),
                             UpdatedAt =  new DateTime(2019, 1, 1),
                         },

                         new TaskDomain()
                         {
                             Name = "Task 2",
                             ActPomodoro = 2,
                             EstPomodoro = 3,
                             TaskState = TaskState.IN_PROGRESS,
                             CreatedAt = new DateTime(2019, 1, 1),
                             UpdatedAt =  new DateTime(2019, 1, 1),
                         }
                     }
                };

                yield return new object[]
                {
                    StatsValue.POMODORO_COMPLETED,
                    0,
                    0,
                    false,
                    false,
                    Array.Empty<DailySessionDomain>(),
                    Array.Empty<TaskDomain>()
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion
    }
}
