using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.Services
{
    public class TimerSessionTests
    {
        private readonly AutoMocker _mocker = new();
        private readonly Mock<IAppSettingsProvider> _configurationMock;
        private readonly Mock<IExtendedTimer> _timerMock;
        private readonly TimerSession _sut;

        public TimerSessionTests()
        {
            _configurationMock = _mocker.GetMock<IAppSettingsProvider>();
            _timerMock = _mocker.GetMock<IExtendedTimer>();
            _sut = _mocker.CreateInstance<TimerSession>();
        }

        [Fact]
        public void ConstructorInvoked_CurrentSessionStateIsPomodoro()
        {
            Assert.Equal(typeof(PomodoroSessionState), _sut.CurrentSessionState.GetType());
        }

        [Fact]
        public void Initialize_RunAndEventInvokedSuccessfully()
        {
            // Arrange
           SetupConfiguration();

           // Act

            TimeSpan timerSetForOutput = TimeSpan.Zero;

            _sut.TimerSetForInitialized += (sender, timerSetFor) => timerSetForOutput = timerSetFor;

            _sut.Initialize();

            // Assert
            Assert.Equal(typeof(PomodoroSessionState), _sut.CurrentSessionState.GetType());
            Assert.Equal(TimeSpan.FromMinutes(25), timerSetForOutput);
        }

        [Fact]
        public void Pause_CanPauseIsFalse()
        {
            // Arrange
            SetupConfiguration();
            SetupStartTimer();

            bool? canPause = null;
            _sut.CanPausedChanged += (_, val) => canPause = val;

            _sut.Initialize();
            _sut.Start();
            _sut.Pause();

            Assert.False(canPause);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(1)]
        public void Start_RunAndEventInvokedSuccessfully(int ticks)
        {
            // Arrange
            SetupConfiguration();
            SetupStartTimer(25, ticks);

            // Act
            TimeSpan? remainingTime = null;
            TimeSpan? timerSetFor = null;
            bool? canPause = null;

            _sut.CanPausedChanged += (_, val) => canPause = val;

            _sut.RemainingTimeChanged += (_, args) =>
            {
                remainingTime = args.RemainingTime;
                timerSetFor = args.TimerSetFor;
            };

            _sut.Initialize();
            _sut.Start();

            // Assert
            Assert.True(canPause);
            Assert.Equal(TimeSpan.FromMinutes(25) - TimeSpan.FromSeconds(ticks), remainingTime);
            Assert.Equal(TimeSpan.FromMinutes(25), timerSetFor);
        }

        [Fact]
        public void Skip_RunAndEventInvokedSuccessfully()
        { 
            // Arrange
            SetupConfiguration();

            // Act
            TimeSpan? timerSetForOutput = null;
            bool? canPauseOutput = null;
            TimerSessionState? currentStateOutput = null;

            _sut.CanPausedChanged += (_, canPause) => canPauseOutput = canPause;
            _sut.CurrentSessionStateChanged += (_, current) => currentStateOutput = current; 
            _sut.TimerSetForInitialized += (_, timerSetFor) => timerSetForOutput = timerSetFor;
            _sut.Initialize();

            _sut.Skip();

            // Assert
            Assert.Equal(typeof(ShortBreakSessionState), _sut.CurrentSessionState.GetType());
            Assert.Equal(typeof(ShortBreakSessionState), currentStateOutput?.GetType());
            Assert.Equal(TimeSpan.FromMinutes(5), timerSetForOutput);
            Assert.False(canPauseOutput);
        }

        [Theory]
        [InlineData(4)]
        //[InlineData(2)]
        //[InlineData(1)]
        public void Skip_AfterLongBreakInterval_GoToLongBreak(int longBreakIntervalInput)
        {
            // Arrange
            SetupConfiguration(longBreakInterval: longBreakIntervalInput);

            // Act
            TimerSessionState? currentStateOutput = null;

            _sut.CurrentSessionStateChanged += (_, current) => currentStateOutput = current;

            _sut.Initialize();

            int numberOfStatesBeforeLongBreak = 2; 
            int skipCount = (longBreakIntervalInput * numberOfStatesBeforeLongBreak)-1;

            for (int i = 0; i < skipCount ; i++)
            {
                _sut.Skip();
            }

            // Assert
            Assert.Equal(typeof(LongBreakSessionState), currentStateOutput?.GetType());
            Assert.Equal(typeof(LongBreakSessionState), _sut.CurrentSessionState.GetType());
        }

        [Fact]
        public void Reset_RunSuccessfully()
        {

            // Arrange
            SetupConfiguration();
            SetupStartTimer(25, 5);

            // Act
            TimeSpan? remainingTime = null;
            bool? canPause = null;
            _sut.RemainingTimeChanged += (_, args) => remainingTime = args.RemainingTime; 
            _sut.TimerSetForInitialized += (_, val) => remainingTime = val;
            _sut.CanPausedChanged += (_, val) => canPause = val;

            _sut.Initialize();
            _sut.Start();
            _sut.Reset();

            // Assert
            Assert.False(canPause);
            Assert.Equal(TimeSpan.FromMinutes(25), remainingTime);
        }

        [Fact]
        public void Finish_RunAndEventInvokedSuccessfully()
        {
            // Arrange
            SetupConfiguration();

            // Act
            TimeSpan? timerSetForOutput = null;
            bool? canPauseOutput = null;
            TimerSessionState? currentStateOutput = null;
            
            _sut.CanPausedChanged += (_, canPause) => canPauseOutput = canPause;
            _sut.CurrentSessionStateChanged += (_, current) => currentStateOutput = current;
            _sut.TimerSetForInitialized += (_, timerSetFor) => timerSetForOutput = timerSetFor;
            _sut.Initialize();

            _sut.Finish();
            
            // Assert
            Assert.Equal(typeof(ShortBreakSessionState), _sut.CurrentSessionState.GetType());
            Assert.Equal(typeof(ShortBreakSessionState), currentStateOutput?.GetType());
            Assert.Equal(TimeSpan.FromMinutes(5), timerSetForOutput);
            Assert.False(canPauseOutput);
        }

        [Theory]
        [InlineData(4)]
        //[InlineData(2)]
        //[InlineData(1)]
        public void Finish_AfterLongBreakInterval_GoToLongBreak(int longBreakIntervalInput)
        {
            // Arrange
            SetupConfiguration(longBreakInterval: longBreakIntervalInput);

            // Act
            TimerSessionState? currentStateOutput = null;

            _sut.CurrentSessionStateChanged += (_, current) => currentStateOutput = current;

            _sut.Initialize();

            int numberOfStatesBeforeLongBreak = 2;
            int skipCount = (longBreakIntervalInput * numberOfStatesBeforeLongBreak) - 1;

            for (int i = 0; i < skipCount; i++)
            {
                _sut.Finish();
            }

            // Assert
            Assert.Equal(typeof(LongBreakSessionState), currentStateOutput?.GetType());
            Assert.Equal(typeof(LongBreakSessionState), _sut.CurrentSessionState.GetType());
        }

        #region Helpers

        private void SetupStartTimer(int timerSetForInMinutes = 25, int ticks = 1)
        {
            _timerMock.SetupGet((x) => x.Interval).Returns(TimeSpan.FromSeconds(1));

            _timerMock.Setup((x) => x.Start())
                .Callback(() =>
                {
                    TimeSpan remainingTime = TimeSpan.FromMinutes(timerSetForInMinutes);

                    for (int i = 0; i < ticks; i++)
                    {
                        remainingTime -= _timerMock.Object.Interval;

                        _timerMock.Raise((x) => x.RemainingTimeChanged += null,
                            new RemainingTimeChangedEventArgs
                                { TimerSetFor = TimeSpan.FromMinutes(timerSetForInMinutes),
                                    RemainingTime = remainingTime });
                    }
                });
        }

        private void SetupConfiguration(
            int pomodoroDurationInMinutes = 25,
            int shortBreakDurationInMinutes = 5, 
            int longBreakDurationInMinutes = 15,
            int longBreakInterval = 4
            )
        {
            _configurationMock.SetupGet((x) => x.AppSettings)
                .Returns(
                    new AppSettings()
                    {
                        PomodoroDuration = TimeSpan.FromMinutes(pomodoroDurationInMinutes),
                        ShortBreakDuration = TimeSpan.FromMinutes(shortBreakDurationInMinutes),
                        LongBreakDuration = TimeSpan.FromMinutes(longBreakDurationInMinutes),
                        LongBreakInterval = longBreakInterval
                    });
        }

        #endregion

    }
}
