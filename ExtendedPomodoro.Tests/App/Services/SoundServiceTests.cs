using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedPomodoro.Core.Timeout;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.Services
{
    public class SoundServiceTests
    {
        private readonly AutoMocker _mocker = new AutoMocker();
        private readonly Mock<IMediaPlayer> _mediaPlayerMock;
        private readonly Mock<RegisterWaitTimeoutCallback> _registerWaitTimeoutCallbackMock;
        private readonly SoundService _sut;

        public SoundServiceTests()
        {
            _mediaPlayerMock = _mocker.GetMock<IMediaPlayer>();
            _registerWaitTimeoutCallbackMock = _mocker.GetMock<RegisterWaitTimeoutCallback>();
            _sut = _mocker.CreateInstance<SoundService>();
        }

        [Theory]
        [InlineData(5, 1, 5, 5)]
        [InlineData(15, 7, 3, 21)]
        [InlineData(10, 5.5, 2, 11)]
        [InlineData(0, 10, 1, 10)]
        [InlineData(-10, 10, 1, 10)]
        public void PlayWithDefiniteRepeatFor_WhilePlayingTimeIsSmallerThanRepeatFor_KeepReplaying(
            double repeatForInSeconds, 
            double soundLengthInSeconds,
            int expectedPlayCount,
            int expectedTotalDurationPlayedInSeconds
            )
        {
            // Arrange
            double[] totalDurationPlayedInSeconds = {0}; // need to use this as callback doesn't support ref parameter

            Action? callbackToStopRepeat = null; 
            
            GenerateSetup(repeatForInSeconds, soundLengthInSeconds, callbackToStopRepeat, totalDurationPlayedInSeconds);

            // Act
            _sut.Open(new Uri("/", UriKind.Relative));
            _sut.RepeatFor = TimeSpan.FromSeconds(repeatForInSeconds);
            _sut.Play();

            // Assert
            if (repeatForInSeconds <= 0)
            {
                _registerWaitTimeoutCallbackMock.Verify((x) => x.Invoke(It.IsAny<Action>(),
                    It.IsAny<TimeSpan>()), Times.Never);
            }

            _mediaPlayerMock.Verify((x) => x.Play(), 
                Times.Exactly(expectedPlayCount));
            _mediaPlayerMock.VerifySet((x) => x.Position = It.IsAny<TimeSpan>(),
                Times.Exactly(expectedPlayCount));
            _mediaPlayerMock.Verify((x) => x.Pause(),
                Times.Once);
            _mediaPlayerMock.VerifyRemove((x) => x.MediaEnded -= It.IsAny<EventHandler>(),
                Times.Once);
            Assert.Equal(expectedTotalDurationPlayedInSeconds, totalDurationPlayedInSeconds[0]);
        }

        [Theory]
        [InlineData(1, 10, 10)]
        [InlineData(7, 20, 3)]
        [InlineData(5.5, 15, 3)]
        [InlineData(8.5, 10, 2)]
        [InlineData(10, 32, 4)]
        public void PlayWithInfiniteRepeatFor_KeepPlayingUntilStopped(
            int soundLengthInSeconds,
            int stopAfterInSeconds,
            int expectedPlayCount
        )
        {
            // Arrange
            double[] totalDurationPlayedInSeconds = { 0 };

            Action? callbackToStopRepeat = null;

            GenerateSetup(
                Timeout.InfiniteTimeSpan.TotalSeconds, 
                soundLengthInSeconds, 
                callbackToStopRepeat, 
                totalDurationPlayedInSeconds,
                true,
                stopAfterInSeconds
                );

            // Act
            _sut.Open(new Uri("/", UriKind.Relative));
            _sut.RepeatFor = Timeout.InfiniteTimeSpan;
            _sut.Play();

            // Assert
            _mediaPlayerMock.Verify((x) => x.Play(),
                Times.Exactly(expectedPlayCount));
            _mediaPlayerMock.VerifySet((x) => x.Position = It.IsAny<TimeSpan>(),
                Times.Exactly(expectedPlayCount));
            _mediaPlayerMock.Verify((x) => x.Pause(),
                Times.Once);
            _mediaPlayerMock.Verify((x) => x.Stop(),
                Times.Once);
            _mediaPlayerMock.VerifyRemove((x) => x.MediaEnded -= It.IsAny<EventHandler>(),
                Times.Once);
        }

        #region Helpers

        private void GenerateSetup(
            double repeatForInSeconds, 
            double soundLengthInSeconds, 
            Action? callbackToStopRepeat,
            double[] totalDurationPlayedInSeconds,
            bool stopAfterEnabled = false,
        int stopAfterInSeconds = 0
            )
        {
            _registerWaitTimeoutCallbackMock
                .Setup((x) => x.Invoke(It.IsAny<Action>(),
                    It.IsAny<TimeSpan>()))
                .Callback((Action callback, TimeSpan _) => callbackToStopRepeat = callback);

            _mediaPlayerMock.Setup((x) => x.Play())
                .Callback(() =>
                {
                    totalDurationPlayedInSeconds[0] += soundLengthInSeconds;

                    if (totalDurationPlayedInSeconds[0] >= repeatForInSeconds)
                        callbackToStopRepeat?.Invoke();

                    if (stopAfterEnabled && totalDurationPlayedInSeconds[0] >= stopAfterInSeconds)
                    {
                        _sut.Stop();
                    }

                    _mediaPlayerMock.Raise((x) => x.MediaEnded += null, EventArgs.Empty);
                });

            _mediaPlayerMock.Setup((x) => x.Pause());
            _mediaPlayerMock.Setup((x) => x.Stop());
            _mediaPlayerMock.SetupGet((x) => x.Position);
            _mediaPlayerMock.SetupRemove((x) => x.MediaEnded -= It.IsAny<EventHandler>());
        }

        #endregion

    }
}
