using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.DTOs;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Services;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.Models.Services
{
    public class DailySessionsServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IDailySessionsRepository> _repositoryMock;
        private readonly DailySessionsService _dailySessionsService;

        public DailySessionsServiceTests()
        {
            _mocker = new();
            _repositoryMock = _mocker.GetMock<IDailySessionsRepository>();
            _dailySessionsService = _mocker.CreateInstance<DailySessionsService>();
        }

        [Fact]
        public void GetDailySessions_ReturnTheCorrectRangedDailySessions()
        {
            // Arrange
            var dbRows = new List<DailySessionDTO>()
            {
                new()
                {
                    SessionDate = new DateTime(2020, 10, 11),
                    TotalPomodoroCompleted = 5,
                    TotalShortBreaksCompleted = 2,
                    TotalLongBreaksCompleted = 3,
                    TimeSpentInSeconds = 4,
                    TotalTasksCompleted = 2,
                    CreatedAt = new DateTime(2020, 10, 11, 5, 5, 5)
                },
                new()
                {
                    SessionDate = new DateTime(2020, 10, 10),
                    CreatedAt = new DateTime(2020, 10, 10, 4, 4, 4)
                },
                new()
                {
                    SessionDate = new DateTime(2020, 10, 9),
                    CreatedAt = new DateTime(2020, 10, 9, 6, 6, 6)
                },
                 new()
                {
                    SessionDate = new DateTime(2020, 10, 8),
                    CreatedAt = new DateTime(2020, 10, 8, 7, 7, 7)
                }
            };

            _repositoryMock
                .Setup((x) => x.GetDailySessions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync((DateTime from, DateTime to) =>
                dbRows.Where((row) => row.CreatedAt >= from && row.CreatedAt <= to)
                      .OrderBy((row) => row.CreatedAt));

            // Act
            var res = _dailySessionsService.GetDailySessions(
                new DateTime(2020, 10, 9, 0, 0, 0), new DateTime(2020, 10, 11, 23, 59, 59))
                .ToBlockingEnumerable().ToList();

            // Assert
            Assert.Equal(3, res.Count);
            Assert.Equal(DateOnly.FromDateTime(dbRows[2].SessionDate), res[0].SessionDate);
            Assert.Equal(DateOnly.FromDateTime(dbRows[1].SessionDate), res[1].SessionDate);
            Assert.Equal(DateOnly.FromDateTime(dbRows[0].SessionDate), res[2].SessionDate);

            Assert.Equal(dbRows.First().TotalPomodoroCompleted, res[2].TotalPomodoroCompleted);
            Assert.Equal(dbRows.First().TotalShortBreaksCompleted, res[2].TotalShortBreaksCompleted);
            Assert.Equal(dbRows.First().TotalLongBreaksCompleted, res[2].TotalLongBreaksCompleted);
            Assert.Equal(dbRows.First().TotalTasksCompleted, res[2].TotalTasksCompleted);
            Assert.Equal(dbRows.First().TimeSpentInSeconds, res[2].TimeSpent.TotalSeconds);
        }

        [Fact]
        public async Task GetSumDailySessions_ReturnTheCorrectRangedSumDailySessions()
        {
            // Arrange
            var dbRows = new List<DailySessionDTO>()
                {
                new()
                {
                    TotalPomodoroCompleted = 1,
                    TotalShortBreaksCompleted = 1,
                    TotalLongBreaksCompleted = 1,
                    TimeSpentInSeconds = 1,
                    TotalTasksCompleted = 1,
                    CreatedAt = new DateTime(2020, 10, 11, 5, 5, 5)
                },
                new()
                {
                    TotalPomodoroCompleted = 2,
                    TotalShortBreaksCompleted = 2,
                    TotalLongBreaksCompleted = 2,
                    TimeSpentInSeconds = 2,
                    TotalTasksCompleted = 2,
                    CreatedAt = new DateTime(2020, 10, 10, 4, 4, 4)
                },
                new()
                {
                    TotalPomodoroCompleted = 3,
                    TotalShortBreaksCompleted = 3,
                    TotalLongBreaksCompleted = 3,
                    TimeSpentInSeconds = 3,
                    TotalTasksCompleted = 3,
                    CreatedAt = new DateTime(2020, 10, 9, 6, 6, 6)
                },
                 new()
                {
                    TotalPomodoroCompleted = 4,
                    TotalShortBreaksCompleted = 4,
                    TotalLongBreaksCompleted = 4,
                    TimeSpentInSeconds = 4,
                    TotalTasksCompleted = 4,
                    CreatedAt = new DateTime(2020, 10, 8, 7, 7, 7)
                }
            };

            _repositoryMock
            .Setup((x) => x.GetSumDailySessions(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((DateTime from, DateTime to) => {
                var res = dbRows
                .Where((row) => row.CreatedAt >= from && row.CreatedAt <= to)
                .Aggregate(
                new SumDailySessionsDTO(),
                (total, row) => new SumDailySessionsDTO()
                {
                    TotalPomodoroCompleted = total.TotalPomodoroCompleted + row.TotalPomodoroCompleted,
                    TotalShortBreaksCompleted = total.TotalShortBreaksCompleted + row.TotalShortBreaksCompleted,
                    TotalLongBreaksCompleted = total.TotalLongBreaksCompleted + row.TotalLongBreaksCompleted,
                    TotalTasksCompleted = total.TotalTasksCompleted + row.TotalTasksCompleted,
                    TotalTimeSpentInSeconds = total.TotalTimeSpentInSeconds + row.TimeSpentInSeconds
                });

                return res;
            });

            // Act
            var res = await _dailySessionsService.GetSumDailySessions(
                new DateTime(2020, 10, 9, 0, 0, 0), new DateTime(2020, 10, 11, 23, 59, 59));

            // Assert
            Assert.Equal(6, res.TotalShortBreaksCompleted);
            Assert.Equal(6, res.TotalLongBreaksCompleted);
            Assert.Equal(6, res.TotalPomodoroCompleted);
            Assert.Equal(6, res.TotalTasksCompleted);
            Assert.Equal(6, res.TotalTimeSpent.TotalSeconds);
        }

        [Fact]
        public async Task GetDateRangeDailySessions_ReturnCorrectResult()
        {
            // Arrange
            var minDate = DateTime.Today.AddDays(-7);
            var maxDate = DateTime.Today;

            _repositoryMock.Setup((x) => x.GetDateRangeDailySessions())
                .ReturnsAsync(new DateRangeDailySessionsDTO() { MinDate = minDate, MaxDate = maxDate });

            // Act
            var res = await _dailySessionsService.GetDateRangeDailySessions();

            // Assert
            Assert.Equal(minDate, res.MinDate);
            Assert.Equal(maxDate, res.MaxDate);

        }

        [Fact]
        public async Task UpsertDailySession_WhenInsert_InsertedProperly()
        {
            // Arrange
            var dbRows = new List<DailySessionDTO>();

            DateTime createdAt = DateTime.Now;
            DateTime updatedAt = DateTime.Now;

            _repositoryMock
                .Setup((x) => x.UpsertDailySession(It.IsAny<UpsertDailySessionDTO>()))
                .Callback((UpsertDailySessionDTO dto) => 
                        dbRows.Add(new()
                        {
                            SessionDate = DateTime.Parse(dto.SessionDate),
                            TimeSpentInSeconds = dto.TimeSpentInSeconds,
                            TotalPomodoroCompleted = dto.TotalPomodoroCompleted,
                            TotalShortBreaksCompleted = dto.TotalShortBreaksCompleted,
                            TotalLongBreaksCompleted = dto.TotalLongBreaksCompleted,
                            CreatedAt = createdAt = dto.CreatedAt,
                            UpdatedAt = updatedAt = dto.UpdatedAt,
                        }));

            // Act
            var param = new UpsertDailySessionDomain()
            {
                SessionDate = DateOnly.FromDateTime(DateTime.Today),
                TotalPomodoroCompleted = 10,
                TotalLongBreaksCompleted = 15,
                TotalShortBreaksCompleted = 20,
            };

            await _dailySessionsService.UpsertDailySession(param);

            // Assert
            Assert.Single(dbRows);
            Assert.Equal(param.SessionDate.ToDateTime(TimeOnly.MinValue), dbRows.First().SessionDate);
            Assert.Equal(param.TotalPomodoroCompleted, dbRows.First().TotalPomodoroCompleted);
            Assert.Equal(param.TotalLongBreaksCompleted, dbRows.First().TotalLongBreaksCompleted);
            Assert.Equal(param.TotalShortBreaksCompleted, dbRows.First().TotalShortBreaksCompleted);
            Assert.Equal(0, dbRows.First().TotalTasksCompleted);
            Assert.Equal(createdAt, dbRows.First().CreatedAt);
            Assert.Equal(updatedAt, dbRows.First().UpdatedAt);
        }

        [Fact]
        public async Task UpsertTimeSpent_WhenInsert_InsertedProperly()
        {
            // Arrange
            var dbRows = new List<DailySessionDTO>();

            var createdAt = DateTime.Now;
            var updatedAt = DateTime.Now;

            _repositoryMock
                .Setup((x) => x.UpsertTimeSpent(It.IsAny<UpsertTimeSpentDTO>()))
                .Callback((UpsertTimeSpentDTO dto)
                        => dbRows.Add(new DailySessionDTO()
                        {
                            SessionDate = DateTime.Parse(dto.SessionDate),
                            TimeSpentInSeconds = dto.TimeSpentInSeconds,
                            CreatedAt = createdAt = dto.CreatedAt,
                            UpdatedAt = updatedAt = dto.UpdatedAt
                        }));

            // Act
            var sessionDateParam = DateOnly.FromDateTime(DateTime.Today);
            var timeSpentParam = TimeSpan.FromMinutes(15);

            await _dailySessionsService.UpsertTimeSpent(sessionDateParam, timeSpentParam);

            // Assert
            Assert.Single(dbRows);
            Assert.Equal(sessionDateParam.ToDateTime(TimeOnly.MinValue), dbRows.First().SessionDate);
            Assert.Equal(timeSpentParam.TotalSeconds, dbRows.First().TimeSpentInSeconds);
            Assert.Equal(0, dbRows.First().TotalPomodoroCompleted);
            Assert.Equal(0, dbRows.First().TotalShortBreaksCompleted);
            Assert.Equal(0, dbRows.First().TotalLongBreaksCompleted);
            Assert.Equal(0, dbRows.First().TotalTasksCompleted);
            Assert.Equal(createdAt, dbRows.First().CreatedAt);
            Assert.Equal(updatedAt, dbRows.First().UpdatedAt);
        }

        [Fact]
        public async Task UpsertTotalTasksCompleted_WhenInsert_InsertedProperly()
        {
            // Arrange
            var dbRows = new List<DailySessionDTO>();

            var createdAt = DateTime.Now;
            var updatedAt = DateTime.Now;

            _repositoryMock
               .Setup((x) => x.UpsertTotalTasksCompleted(It.IsAny<UpsertTotalTasksCompletedDTO>()))
               .Callback((UpsertTotalTasksCompletedDTO dto)
                       => dbRows.Add(new DailySessionDTO()
                       {
                           SessionDate = DateTime.Parse(dto.SessionDate),
                           TotalTasksCompleted = dto.TotalTasksCompleted,
                           CreatedAt = createdAt = dto.CreatedAt,
                           UpdatedAt = updatedAt = dto.UpdatedAt
                       }));

            // Act
            var sessionDateParam = DateOnly.FromDateTime(DateTime.Today);
            var totalTasksComplParam = 1;
            await _dailySessionsService.UpsertTotalTasksCompleted(
                sessionDateParam, totalTasksComplParam);

            // Assert
            Assert.Single(dbRows);
            Assert.Equal(sessionDateParam.ToDateTime(TimeOnly.MinValue), dbRows.First().SessionDate);
            Assert.Equal(totalTasksComplParam, dbRows.First().TotalTasksCompleted);
            Assert.Equal(0, dbRows.First().TotalPomodoroCompleted);
            Assert.Equal(0, dbRows.First().TotalShortBreaksCompleted);
            Assert.Equal(0, dbRows.First().TotalLongBreaksCompleted);
            Assert.Equal(0, dbRows.First().TimeSpentInSeconds);
            Assert.Equal(createdAt, dbRows.First().CreatedAt);
            Assert.Equal(updatedAt, dbRows.First().UpdatedAt);
        }

        [Fact]
        public async Task GetTotalPomodoroCompleted_ReturnCorrectResult()
        {
            // Arrange
            var dbRows = new List<DailySessionDTO>()
            {
                new()
                {
                    SessionDate = new DateTime(2020, 10, 11),
                    TotalPomodoroCompleted = 5,
                },

                new()
                {
                    SessionDate = new DateTime(2020, 10, 12),
                    TotalPomodoroCompleted = 3,
                },

                new()
                {
                    SessionDate = new DateTime(2020, 10, 13),
                    TotalPomodoroCompleted = 1,
                }
            };

            _repositoryMock
                .Setup((x) => x.GetTotalPomodoroCompleted(It.IsAny<string>()))
                .ReturnsAsync((string sessionDate) => 
                dbRows.Where((row) => DateOnly.FromDateTime(row.SessionDate).ToString() == sessionDate)
                .First().TotalPomodoroCompleted);

            // Act
            var res = await _dailySessionsService.GetTotalPomodoroCompleted(new DateOnly(2020, 10, 12));

            // Assert
            Assert.Equal(3, res);
        }
    }
}
