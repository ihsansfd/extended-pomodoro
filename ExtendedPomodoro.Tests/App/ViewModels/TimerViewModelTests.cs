using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.ViewServices.Interfaces;
using Moq;
using Moq.AutoMock;

namespace ExtendedPomodoro.Tests.App.ViewModels
{
    public class TimerViewModelTests
    {
        private readonly AutoMocker _mocker = new();
        private readonly Mock<ITimerSession> _timerSessionMock;
        private readonly Mock<ITimerViewService> _timerViewServiceMock;
        private readonly Mock<IAppSettingsProvider> _appSettingsProviderMock;
        private readonly Mock<IDailySessionsService> _dailySessionsServiceMock;
        private readonly Mock<ITasksService> _tasksServiceMock;
        private readonly Mock<IMessenger> _messengerMock;
        private readonly Mock<IExtendedTimer> _extendedTimerMock;

        private readonly ReadTasksViewModel _readTasksViewModel;
        private readonly CreateTaskViewModel _createTasksViewModel;
        private readonly TimerSession _timerSession;
        private readonly TimerViewModel _sut;

        public TimerViewModelTests()
        {
            _timerSessionMock = _mocker.GetMock<ITimerSession>();
            _timerViewServiceMock = _mocker.GetMock<ITimerViewService>();
            _appSettingsProviderMock = _mocker.GetMock<IAppSettingsProvider>();
            _dailySessionsServiceMock = _mocker.GetMock<IDailySessionsService>();
            _tasksServiceMock = _mocker.GetMock<ITasksService>();
            _messengerMock = _mocker.GetMock<IMessenger>();
            _extendedTimerMock = _mocker.GetMock<IExtendedTimer>();
            _timerSession = _mocker.CreateInstance<TimerSession>();
            _readTasksViewModel = _mocker.CreateInstance<ReadTasksViewModel>();
            _createTasksViewModel = _mocker.CreateInstance<CreateTaskViewModel>();
            _sut = new TimerViewModel(
                _readTasksViewModel,
                _createTasksViewModel,
                _timerSessionMock.Object,
                _timerViewServiceMock.Object,
                _appSettingsProviderMock.Object,
                _dailySessionsServiceMock.Object,
                _tasksServiceMock.Object,
                _messengerMock.Object
            );
        }

        [Fact]
        public async Task LoadCommand_Successfully()
        {
            // Arrange
            _dailySessionsServiceMock.Setup((x) =>
                    x.GetTotalPomodoroCompleted(It.IsAny<DateOnly>()))
                .ReturnsAsync(5);

            AlarmSound? alarmSoundSet = null;
            int? soundVolumeSet = null;
            bool? isAlarmRepeatForeverSet = null;

            _timerViewServiceMock.SetupSet((x) => x.AlarmSound = It.IsAny<AlarmSound>())
                .Callback((AlarmSound val) => alarmSoundSet = val);
            _timerViewServiceMock.SetupSet((x) => x.SoundVolume = It.IsAny<int>())
                .Callback((int val) => soundVolumeSet = val);
            _timerViewServiceMock.SetupSet((x) => x.IsAlarmRepeatForever = It.IsAny<bool>())
                .Callback((bool val) => isAlarmRepeatForeverSet = val);

            _tasksServiceMock.Setup((x) => x.GetTasks(
                It.IsAny<TaskState>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<TaskDomain> {
                        new()
                        {
                            Id = 1,
                            Name = "task"
                        }
                    }.ToAsyncEnumerable()
                );


            var dailyPomodoroTarget = 5;
            var alarmSound = AlarmSound.Echo;
            var soundVolume = 70;
            var isAlarmRepeatForever = false;

            SetupSettingsProvider(
                dailyPomodoroTarget: dailyPomodoroTarget,
                alarmSound: alarmSound,
                soundVolume: soundVolume,
                isRepeatForever: isAlarmRepeatForever);

            // Act
            await _sut.LoadCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(dailyPomodoroTarget, _sut.DailyPomodoroTarget);
            Assert.Equal(alarmSound, alarmSoundSet);
            Assert.Equal(soundVolume, soundVolumeSet);
            Assert.Equal(isAlarmRepeatForever, isAlarmRepeatForeverSet);
            Assert.Single(_sut.ReadTasksViewModel.Tasks);
        }

        [Fact]
        public void OpenAddNewTaskModalCommand_Successfully()
        {
            _sut.OpenAddNewTaskModalCommand.Execute(null);

            Assert.True(_sut.CreateTaskViewModel.IsModalShown);
        }

        [Fact]
        public async Task CompleteTaskCommand_Successfully()
        {
            // Arrange
            _tasksServiceMock.Setup((x) => 
                x.UpdateTaskState(It.IsAny<int>(), It.IsAny<TaskState>()));
            _tasksServiceMock.Setup((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()));
            _tasksServiceMock.Setup((x) => x.GetTasks(
                    It.IsAny<TaskState>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new List<TaskDomain> {
                        new()
                        {
                            Id = 1,
                            Name = "task"
                        }
                    }.ToAsyncEnumerable()
                );
            _dailySessionsServiceMock.Setup((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()));
            _dailySessionsServiceMock.Setup((x) =>
                x.UpsertTotalTasksCompleted(It.IsAny<DateOnly>(), It.IsAny<int>()));
            

            // Act
            _sut.SelectedTask = new TaskDomainViewModel();
            await _sut.CompleteTaskCommand.ExecuteAsync(null);

            // Assert
            Assert.Null(_sut.SelectedTask);
            _tasksServiceMock.Verify((x) =>
                x.UpdateTaskState(It.IsAny<int>(), It.IsAny<TaskState>()), Times.Once);
            _tasksServiceMock.Verify((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once);
            _dailySessionsServiceMock.Verify((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()), Times.Once);
            _dailySessionsServiceMock.Verify((x) =>
                x.UpsertTotalTasksCompleted(It.IsAny<DateOnly>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void CancelTaskCommand_Successfully()
        {
            // Arrange
            _tasksServiceMock.Setup((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()));
            _dailySessionsServiceMock.Setup((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()));

            // Act
            _sut.SelectedTask = new TaskDomainViewModel();
            _sut.CancelTaskCommand.Execute(null);

            // Assert
            Assert.Null(_sut.SelectedTask);
            _tasksServiceMock.Verify((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once);
            _dailySessionsServiceMock.Verify((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public void StartSessionCommand_Successfully()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Start());
            _timerViewServiceMock.Setup((x) => x.PlayMouseClickEffectSound());

            // Act
            _sut.StartSessionCommand.Execute(null);

            // Assert
            _timerSessionMock.Verify((x) => x.Start(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.PlayMouseClickEffectSound(), Times.Once);
        }

        [Fact]
        public void PauseSessionCommand_Successfully()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Pause());
            _timerViewServiceMock.Setup((x) => x.PlayMouseClickEffectSound());

            // Act
            _sut.PauseSessionCommand.Execute(null);

            // Assert
            _timerSessionMock.Verify((x) => x.Pause(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.PlayMouseClickEffectSound(), Times.Once);
        }

        [Fact]
        public void SkipSessionCommand_Successfully()
        {
            // Arrange
            SetupSettingsProvider(longBreakInterval: 4);
            _timerSessionMock.Setup((x) => x.Skip());

            // Act
            _sut.SkipSessionCommand.Execute(null);

            // Assert
            _timerSessionMock.Verify((x) => x.Skip(), Times.Once);
        }

        [Fact]
        public void ResetSessionCommand_Successfully()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Reset());

            // Act
            _sut.ResetSessionCommand.Execute(null);

            // Assert
            _timerSessionMock.Verify((x) => x.Reset(), Times.Once);

        }

        [Fact]
        public void ReceiveStartHotkeyTriggeredMessage_Successfully()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Start());
            _timerViewServiceMock.Setup((x) => x.PlayMouseClickEffectSound());
            _timerViewServiceMock.Setup((x) => 
                x.ShowTimerStartedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()));

            SetupSettingsProvider(pushNotificationEnabled: true);

            // Act
            _sut.Receive(new StartHotkeyTriggeredMessage());

            // Assert
            _timerSessionMock.Verify((x) => x.Start(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.PlayMouseClickEffectSound(), Times.Once);
            _timerViewServiceMock.Verify((x) =>
                x.ShowTimerStartedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ReceivePauseHotkeyTriggeredMessage_Successfully()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Pause());
            _timerViewServiceMock.Setup((x) => x.PlayMouseClickEffectSound());
            _timerViewServiceMock.Setup((x) =>
                x.ShowTimerPausedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()));

            SetupSettingsProvider(pushNotificationEnabled: true);

            // Act
            _sut.Receive(new PauseHotkeyTriggeredMessage());

            // Assert
            _timerSessionMock.Verify((x) => x.Pause(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.PlayMouseClickEffectSound(), Times.Once);
            _timerViewServiceMock.Verify((x) =>
                x.ShowTimerPausedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ReceiveStartHotkeyTriggeredMessage_WhenPushNotificationNotEnabled_NotShowBalloon()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Start());
            _timerViewServiceMock.Setup((x) => x.PlayMouseClickEffectSound());
            _timerViewServiceMock.Setup((x) =>
                x.ShowTimerStartedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()));

            SetupSettingsProvider(pushNotificationEnabled: false);

            // Act
            _sut.Receive(new StartHotkeyTriggeredMessage());

            // Assert
            _timerSessionMock.Verify((x) => x.Start(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.PlayMouseClickEffectSound(), Times.Once);
            _timerViewServiceMock.Verify((x) =>
                x.ShowTimerStartedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void ReceivePauseHotkeyTriggeredMessage_WhenPushNotificationNotEnabled_NotShowBalloon()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Pause());
            _timerViewServiceMock.Setup((x) => x.PlayMouseClickEffectSound());
            _timerViewServiceMock.Setup((x) =>
                x.ShowTimerPausedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()));

            SetupSettingsProvider(pushNotificationEnabled: false);

            // Act
            _sut.Receive(new PauseHotkeyTriggeredMessage());

            // Assert
            _timerSessionMock.Verify((x) => x.Pause(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.PlayMouseClickEffectSound(), Times.Once);
            _timerViewServiceMock.Verify((x) =>
                x.ShowTimerPausedBalloonTips(It.IsAny<TimerSessionState>(), It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0)]
        public void ReceiveSettingsUpdateInfoMessage_AssignFromSettings(int sessionProgressInput)
        {
            AlarmSound? alarmSoundSet = null;
            int? soundVolumeSet = null;
            bool? isAlarmRepeatForeverSet = null;

            _timerViewServiceMock.SetupSet((x) => x.AlarmSound = It.IsAny<AlarmSound>())
                .Callback((AlarmSound val) => alarmSoundSet = val);
            _timerViewServiceMock.SetupSet((x) => x.SoundVolume = It.IsAny<int>())
                .Callback((int val) => soundVolumeSet = val);
            _timerViewServiceMock.SetupSet((x) => x.IsAlarmRepeatForever = It.IsAny<bool>())
                .Callback((bool val) => isAlarmRepeatForeverSet = val);

            var dailyPomodoroTarget = 5;
            var alarmSound = AlarmSound.Echo;
            var soundVolume = 70;
            var isAlarmRepeatForever = false;

            SetupSettingsProvider(
                dailyPomodoroTarget: dailyPomodoroTarget,
                alarmSound: alarmSound,
                soundVolume: soundVolume,
                isRepeatForever: isAlarmRepeatForever);

            // Act
            _sut.SessionProgress = sessionProgressInput;
            _sut.Receive(new SettingsUpdateInfoMessage(new AppSettings()));

            // Assert
            Assert.Equal(dailyPomodoroTarget, _sut.DailyPomodoroTarget);
            Assert.Equal(alarmSound, alarmSoundSet);
            Assert.Equal(soundVolume, soundVolumeSet);
            Assert.Equal(isAlarmRepeatForever, isAlarmRepeatForeverSet);
        }

        [Fact]
        public void ReceiveSettingsUpdateInfoMessage_WhenSessionProgressNotZero_NotInitializeTimer()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Initialize());
            SetupSettingsProvider();

            // Act
            _sut.SessionProgress = 10;
            _sut.Receive(new SettingsUpdateInfoMessage(new AppSettings()));

            // Assert
            _timerSessionMock.Verify((x) => x.Initialize(), Times.Once); // first one is when the timersession get called
        }

        [Fact]
        public void ReceiveSettingsUpdateInfoMessage_WhenSessionProgressZero_InitializeTimer()
        {
            // Arrange
            _timerSessionMock.Setup((x) => x.Initialize());
            SetupSettingsProvider();

            // Act
            _sut.SessionProgress = 0;
            _sut.Receive(new SettingsUpdateInfoMessage(new AppSettings()));

            // Assert
            _timerSessionMock.Verify((x) => x.Initialize(), Times.Exactly(2)); // first one is when the timersession get called
        }

        [Fact]
        public void ReceiveMainWindowIsClosingMessage_WhenTheresTask_SuccessfullyStored()
        {
            // Arrange
            _tasksServiceMock.Setup((x) => 
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()));
            _dailySessionsServiceMock.Setup((x) => 
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()));

            // Act
            _sut.SelectedTask = new TaskDomainViewModel() { Id = 5 };
            _sut.Receive(new MainWindowIsClosingMessage());

            // Assert
            _tasksServiceMock.Verify((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once);
            _dailySessionsServiceMock.Verify((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public void ReceiveMainWindowIsClosingMessage_WhenTheresNoTask_OnlyStoreSessions()
        {
            // Arrange
            _tasksServiceMock.Setup((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()));
            _dailySessionsServiceMock.Setup((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()));

            // Act
            _sut.SelectedTask = null;
            _sut.Receive(new MainWindowIsClosingMessage());

            // Assert
            _tasksServiceMock.Verify((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Never);
            _dailySessionsServiceMock.Verify((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public void ReceiveMainWindowIsClosingMessage_TimeElapsedStoredAndReset()
        {
            // Arrange
            TimeSpan? timeElapsedTask = null;
            TimeSpan? timeElapsedSession = null;

            _tasksServiceMock.Setup((x) =>
                x.UpdateTimeSpent(It.IsAny<int>(), It.IsAny<TimeSpan>()))
                .Callback((int _, TimeSpan timeSpent) => timeElapsedTask = timeSpent);
            _dailySessionsServiceMock.Setup((x) =>
                x.UpsertTimeSpent(It.IsAny<DateOnly>(), It.IsAny<TimeSpan>()))
                .Callback((DateOnly _, TimeSpan timeSpent) => timeElapsedSession = timeSpent);

            _timerSessionMock.Raise((x) => x.RemainingTimeChanged += null, 
                new RemainingTimeChangedEventArgs());

            // Act
            _sut.SelectedTask = new TaskDomainViewModel() { Id = 5 };
            _sut.Receive(new MainWindowIsClosingMessage());

            // Assert
            Assert.Equal(TimeSpan.FromSeconds(1), timeElapsedTask);
            Assert.Equal(TimeSpan.FromSeconds(1), timeElapsedSession);

            // Act
            _sut.SelectedTask = new TaskDomainViewModel() { Id = 5 };
            _sut.Receive(new MainWindowIsClosingMessage());

            // Assert
            Assert.Equal(TimeSpan.FromSeconds(0), timeElapsedTask);
            Assert.Equal(TimeSpan.FromSeconds(0), timeElapsedSession);
        }

        [Fact]
        public void ReceiveFinishBalloonStartNextSessionClickedMessage_Successfully()
        {
            // Arrange
            _timerViewServiceMock.Setup((x) => x.StopAlarmSound());
            _timerViewServiceMock.Setup((x) => x.CloseCurrentSessionFinishDialog());

            _timerSessionMock.Setup((x) => x.Start());
            _timerViewServiceMock.Setup((x) => x.PlayMouseClickEffectSound());

            // Act
            _sut.Receive(new FinishBalloonStartNextSessionClickedMessage());

            // Assert
            _timerViewServiceMock.Verify((x) => x.StopAlarmSound(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.CloseCurrentSessionFinishDialog(), Times.Once);
            _timerSessionMock.Verify((x) => x.Start(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.PlayMouseClickEffectSound(), Times.Once);
        }

        [Fact]
        public void ReceiveFinishBalloonCloseClickedMessage_WhenIsRepeatForever_DoNothing()
        {
            // Arrange
            _timerViewServiceMock.Setup((x) => x.StopAlarmSound());
            _timerViewServiceMock.Setup((x) => x.CloseCurrentSessionFinishDialog());

            SetupSettingsProvider(isRepeatForever: true);

            // Act
            _sut.Receive(new FinishBalloonCloseClickedMessage());

            // Assert
            _timerViewServiceMock.Verify((x) => x.StopAlarmSound(), Times.Never);
            _timerViewServiceMock.Verify((x) => x.CloseCurrentSessionFinishDialog(), Times.Never);
        }

        [Fact]
        public void ReceiveFinishBalloonCloseClickedMessage_WhenIsNotRepeatForever_StopAlarmAndCloseDialog()
        {
            // Arrange
            _timerViewServiceMock.Setup((x) => x.StopAlarmSound());
            _timerViewServiceMock.Setup((x) => x.CloseCurrentSessionFinishDialog());

            SetupSettingsProvider(isRepeatForever: false);

            // Act
            _sut.Receive(new FinishBalloonCloseClickedMessage());

            // Assert
            _timerViewServiceMock.Verify((x) => x.StopAlarmSound(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.CloseCurrentSessionFinishDialog(), Times.Once);
        }

        [Fact]
        public void FinishDialogCloseClickedMessage_StopAlarmAndCloseFinishBalloon()
        {
            // Arrange
            _timerViewServiceMock.Setup((x) => x.StopAlarmSound());
            _timerViewServiceMock.Setup((x) => x.CloseCurrentSessionFinishBalloon());

            // Act
            _sut.Receive(new FinishDialogCloseClickedMessage());

            // Assert
            _timerViewServiceMock.Verify((x) => x.StopAlarmSound(), Times.Once);
            _timerViewServiceMock.Verify((x) => x.CloseCurrentSessionFinishBalloon(), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanPauseEventRaised_CanPauseValueChanged(bool canPauseArg)
        {
            _timerSessionMock.Raise((x) => 
                x.CanPausedChanged += null, _timerSessionMock.Object, canPauseArg);

            Assert.Equal(canPauseArg, _sut.CanPause);
        }

        [Fact]
        public void CurrentSessionStateChangedRaised_CurrentSessionStateValueChanged()
        {
            SetupSettingsProvider(longBreakInterval: 4);

            TimerSessionState sessionStateArg = new ShortBreakSessionState(_timerSession);

            _timerSessionMock.Raise((x) =>
                x.CurrentSessionStateChanged += null, _timerSessionMock.Object, sessionStateArg);

            Assert.Equal(sessionStateArg, _sut.CurrentSessionState);
        }

        [Fact]
        public void TimerSetForInitializedRaised_ExecutedSuccessfully()
        {
            var timerSetForArg = TimeSpan.FromMinutes(30); 
            _timerSessionMock.Raise((x) => 
                x.TimerSetForInitialized += null, _timerSessionMock.Object, timerSetForArg);

            //Assert.Equal(_sut.RemainingTimeFormatted);
            Assert.Equal(0, _sut.SessionProgress);
            Assert.Equal("30:00", _sut.RemainingTimeFormatted);
        }

        [Theory]
        [InlineData(30, 30, 0, "30:00")]
        [InlineData(30, 0, 100, "00:00")]
        [InlineData(30, 15, 50, "15:00")]
        [InlineData(30, 24, 20, "24:00")]
        public void RemainingTimeChangedRaised_SessionProgressAndUpdatedRemainingTimeAreCorrect(
            int timerSetForInMinutesArg, 
            int remainingTimeInMinutesArg, 
            double expectedSessionProgress,
            string expectedRemainingTimeFormatted
            )
        {
            var timerSetForArg = TimeSpan.FromMinutes(timerSetForInMinutesArg);
            var remainingTimeArg = TimeSpan.FromMinutes(remainingTimeInMinutesArg);

            _timerSessionMock.Raise((x)
                => x.RemainingTimeChanged += null, this, 
                new RemainingTimeChangedEventArgs { TimerSetFor = timerSetForArg, RemainingTime = remainingTimeArg } );

            Assert.Equal(expectedSessionProgress, _sut.SessionProgress);
            Assert.Equal(expectedRemainingTimeFormatted, _sut.RemainingTimeFormatted);
        }

        [Fact]
        public void RemainingTimeChangedRaised_WhenRemainingTimeZero_Finish()
        {
            _timerSessionMock.Raise((x)
                    => x.RemainingTimeChanged += null, this,
                new RemainingTimeChangedEventArgs { RemainingTime = TimeSpan.Zero });

            _timerSessionMock.Setup((x) => x.Finish());

            _timerSessionMock.Verify((x) => x.Finish(), Times.Once);
        }

        [Fact]
        public void RemainingTimeChangedRaised_WhenRemainingTimeNotZero_NotFinish()
        {
            _timerSessionMock.Raise((x)
                    => x.RemainingTimeChanged += null, this,
                new RemainingTimeChangedEventArgs { RemainingTime = TimeSpan.FromMinutes(1) });

            _timerSessionMock.Setup((x) => x.Finish());

            _timerSessionMock.Verify((x) => x.Finish(), Times.Never);
        }

        // TODO
        [Fact]
        public void SessionFinishRaised_()
        {

        }

        #region Helpers

        //private void SetupSessionsService()
        //{

        //}

        private void SetupSettingsProvider(
            int pomodoroDurationInMinutes = 25,
            int shortBreakDurationInMinutes = 5,
            int longBreakDurationInMinutes = 15,
            int dailyPomodoroTarget = 10,
            int longBreakInterval = 4,
            AlarmSound alarmSound = AlarmSound.Chimes,
            int soundVolume = 50,
            bool isAutostart = false,
            bool isRepeatForever = false,
            bool pushNotificationEnabled = true
        )
        {

            _appSettingsProviderMock.SetupGet((x) => x.AppSettings).Returns(
                new AppSettings()
                {
                    PomodoroDuration = TimeSpan.FromMinutes(pomodoroDurationInMinutes),
                    ShortBreakDuration = TimeSpan.FromMinutes(shortBreakDurationInMinutes),
                    LongBreakDuration = TimeSpan.FromMinutes(longBreakDurationInMinutes),
                    DailyPomodoroTarget = dailyPomodoroTarget,
                    LongBreakInterval = longBreakInterval,
                    AlarmSound = alarmSound,
                    Volume = soundVolume,
                    IsAutostart = isAutostart,
                    IsRepeatForever = isRepeatForever,
                    PushNotificationEnabled = pushNotificationEnabled
                }
            );
        }

        #endregion

    }
}
