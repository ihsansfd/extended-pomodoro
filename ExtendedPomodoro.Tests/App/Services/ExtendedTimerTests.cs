using ExtendedPomodoro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using ExtendedPomodoro.Services.Interfaces;
using FluentAssertions.Extensions;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.Services
{
    public class ExtendedTimerTests
    {
        private readonly AutoMocker _mocker = new AutoMocker();
        private readonly Mock<ITimer> _timerMock;
        private readonly ExtendedTimer _sut;

        public ExtendedTimerTests()
        {
            _timerMock = _mocker.GetMock<ITimer>();
            _sut = _mocker.CreateInstance<ExtendedTimer>();
        }

        [Theory]
        [InlineData(1, 10, 10)]
        [InlineData(1, 5, 5)]
        [InlineData(2, 30, 15)]
        public void StartUntilTimerFinishItself_InvokedCorrectlyAndStopAutomatically(
            int intervalInSeconds, int timerSetForInSeconds, int expectedInvokedCount)
        {
            // Arrange
            GenerateTimerSetup(intervalInSeconds, timerSetForInSeconds);

            // Act
            int invokedCount = 0;
            int remainingTimeInSeconds = timerSetForInSeconds;

            _sut.RemainingTimeChanged += (sender, args) =>
            {
                invokedCount++;
                remainingTimeInSeconds = (int)args.RemainingTime.TotalSeconds;
            };

            _sut.Initialize(TimeSpan.FromSeconds(timerSetForInSeconds));
            _sut.Interval = TimeSpan.FromSeconds(intervalInSeconds);
            _sut.Start();

            // Assert
            _timerMock.Verify((x) => x.Stop(), Times.Once);
            Assert.Equal(0, remainingTimeInSeconds);
            Assert.Equal(expectedInvokedCount, invokedCount);
        }

        [Theory]
        [InlineData(1, 10, 5, 5, 5)]
        [InlineData(1, 5, 4, 4, 1)]
        [InlineData(2, 30, 10, 5, 20)]
        public void StartAndCaptureResultHalfway_InvokedCorrectly(
            int intervalInSeconds, 
            int timerSetForInSeconds, 
            int stopAt, 
            int expectedInvokedCount,
            int expectedRemainingTimeInSeconds
            )
        {
            // Arrange
            GenerateTimerSetup(intervalInSeconds, stopAt);

            // Act
            int invokedCount = 0;
            int remainingTimeInSeconds = timerSetForInSeconds;

            _sut.RemainingTimeChanged += (sender, args) =>
            {
                invokedCount++;
                remainingTimeInSeconds = (int)args.RemainingTime.TotalSeconds;
            };

            _sut.Initialize(TimeSpan.FromSeconds(timerSetForInSeconds));
            _sut.Interval = TimeSpan.FromSeconds(intervalInSeconds);
            _sut.Start();

            // Assert
            _timerMock.Verify((x) => x.Stop(), Times.Never);
            Assert.Equal(expectedRemainingTimeInSeconds, remainingTimeInSeconds);
            Assert.Equal(expectedInvokedCount, invokedCount);
        }

        #region Helpers

        private void GenerateTimerSetup(int intervalInSeconds, int stopAt)
        {
            _timerMock.Setup((x) => x.Start())
                .Callback(() =>
                {
                    for (int i = 0; i < stopAt; i += intervalInSeconds)
                    {
                        _timerMock.Raise((x) =>
                            x.Tick += null, _timerMock.Object, EventArgs.Empty);
                    }
                });

            _timerMock.SetupGet((x) => x.Interval)
                .Returns(TimeSpan.FromSeconds(intervalInSeconds));

            _timerMock.Setup((x) => x.Stop());
        }

        #endregion

    }
}
