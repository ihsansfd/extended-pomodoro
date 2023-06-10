using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ExtendedPomodoro.ViewServices.Interfaces;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TimerViewModel : 
        ObservableObject, 
        IRecipient<SettingsUpdateInfoMessage>,
        IRecipient<FinishBalloonStartNextSessionClickedMessage>,
        IRecipient<FinishBalloonCloseClickedMessage>,
        IRecipient<FinishDialogCloseClickedMessage>,
        IRecipient<StartHotkeyTriggeredMessage>,
        IRecipient<PauseHotkeyTriggeredMessage>,
        IRecipient<MainWindowIsClosingMessage>,
        IRecipient<TaskCreationInfoMessage>
    {
        public IFlashMessageServiceViewModel FlashMessageServiceViewModel { get; init; }

        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IDailySessionsService _sessionsService;
        private readonly ITasksService _tasksService;
        private readonly IMessenger _messenger;
        private readonly ITimerViewService _timerViewService;
        private readonly ITimerSession _timerSession;

        private int _timeElapsed = 0;

        public TimerViewModel(
            ReadTasksViewModel readTasksViewModel,
            CreateTaskViewModel createTaskViewModel,
            ITimerSession timerSession,
            ITimerViewService timerViewService,
            IAppSettingsProvider appSettingsProvider,
            IDailySessionsService sessionsRepository,
            ITasksService tasksRepository,
            IMessenger messenger,
            IFlashMessageServiceViewModel flashMessageServiceViewModel
            )
        {

            ReadTasksViewModel = readTasksViewModel;
            CreateTaskViewModel = createTaskViewModel;
            _timerSession = timerSession;
            _appSettingsProvider = appSettingsProvider;
            _timerViewService = timerViewService;
            _sessionsService = sessionsRepository;
            _tasksService = tasksRepository;
            _messenger = messenger;
            FlashMessageServiceViewModel = flashMessageServiceViewModel;
            _messenger.RegisterAll(this);

            _timerSession.SessionFinish += TimerSession_OnSessionFinish;
            _timerSession.CurrentSessionStateChanged += TimerSession_OnCurrentSessionStateChanged;
            _timerSession.TimerSetForInitialized += TimerSession_OnTimerSetForInitialized;
            _timerSession.CanPausedChanged += TimerSession_OnCanPausedChanged;
            _timerSession.RemainingTimeChanged += TimerSession_OnRemainingTimeChanged;
            CurrentSessionState = _timerSession.CurrentSessionState;
            _timerSession.Initialize();
        }

        #region Tasks

        public ReadTasksViewModel ReadTasksViewModel { get; }
        public CreateTaskViewModel CreateTaskViewModel { get; }

        private TaskDomainViewModel? _lastSelectedTask = null;

        [ObservableProperty]
        private TaskDomainViewModel? _selectedTask;

        [ObservableProperty]
        private int _pomodoroCompletedToday;

        [ObservableProperty]
        private bool _isTasksDropdownOpen;

        [RelayCommand]
        private async Task Load()
        {
            PomodoroCompletedToday = await GetPomodoroCompletedToday();
            AssignFromSettings();
            await ReadTasksViewModel.DisplayInProgressTasksCommand.ExecuteAsync(null);
            SelectedTask = _lastSelectedTask;
        }

        [RelayCommand]
        private void OpenAddNewTaskModal()
        {
            CreateTaskViewModel.IsModalShown = true;
            IsTasksDropdownOpen = false;
        }

        [RelayCommand]
        private async Task CompleteTask()
        {
            if (SelectedTask == null)
            {
                Debug.Fail("Selected task cannot be null");
                return;
            }

            var today = DateOnly.FromDateTime(DateTime.Now);

            try
            {
                await UpdateTaskStateToComplete(SelectedTask.Id);
                FlashMessageServiceViewModel.NewFlashMessage(FlashMessageType.SUCCESS, "Task marked as completed.");
            }

            catch (Exception ex)
            {
                FlashMessageServiceViewModel.NewFlashMessage(FlashMessageType.ERROR, $"Failed to mark task as completed: {ex.Message}");

            }

            await StoreAndResetTimeElapsed(today, SelectedTask);
            await StoreDailySessionTotalTasksCompleted(today, 1);
            await ReadTasksViewModel.LoadTasks();

            _lastSelectedTask = SelectedTask = null;
        }

        [RelayCommand]
        private async Task CancelTask()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            await StoreAndResetTimeElapsed(today, SelectedTask);
            _lastSelectedTask = SelectedTask = null;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(SelectedTask) && SelectedTask != null)
            {
                _lastSelectedTask = SelectedTask;
            }
        }

        #endregion

        #region Timer variables, props, and commands

        private TimeSpan _remainingTime;
        
        [ObservableProperty] // from 0 to 100, use converter to set it backwards
        private double _sessionProgress = 0.0;

        [ObservableProperty]
        private TimerSessionState _currentSessionState;

        [ObservableProperty]
        private bool _canPause;

        [ObservableProperty]
        private string _remainingTimeFormatted = null!;

        [ObservableProperty]
        private int _dailyPomodoroTarget;

        [RelayCommand]
        private void StartSession() {
            _timerSession.Start();
            _timerViewService.PlayMouseClickEffectSound();
        }

        [RelayCommand]
        private void PauseSession()
        {
            _timerSession.Pause();
            _timerViewService.PlayMouseClickEffectSound();
        }

        [RelayCommand]
        private void SkipSession() => _timerSession.Skip();

        [RelayCommand]
        private void ResetSession() => _timerSession.Reset();

        private void StartSessionFromHotkey()
        {
           StartSession();
           if(_appSettingsProvider.AppSettings.PushNotificationEnabled)
            {
                _timerViewService.ShowTimerStartedBalloonTips(CurrentSessionState);
            }
        }

        private void PauseSessionFromHotkey()
        {
           PauseSession();
           if(_appSettingsProvider.AppSettings.PushNotificationEnabled)
            {
                _timerViewService.ShowTimerPausedBalloonTips(CurrentSessionState);
            }
        }

        #endregion

        #region messages and events
        public void Receive(StartHotkeyTriggeredMessage message)
        {
            StartSessionFromHotkey();
        }

        public void Receive(PauseHotkeyTriggeredMessage message)
        {
            PauseSessionFromHotkey();
        }

        /// <summary>
        /// If there are changes in settings while the timer hasn't run, reinitialized.
        /// </summary>
        /// <param name="message"></param>
        public void Receive(SettingsUpdateInfoMessage message)
        {
            if (SessionProgress <= 0.0) _timerSession.Initialize();
            AssignFromSettings();
        }

        public async void Receive(MainWindowIsClosingMessage message)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            await StoreAndResetTimeElapsed(today, SelectedTask);
        }


        public void Receive(FinishBalloonStartNextSessionClickedMessage message)
        {
            _timerViewService.StopAlarmSound();
            _timerViewService.CloseCurrentSessionFinishDialog();
            StartSession();
        }

        public void Receive(FinishBalloonCloseClickedMessage message)
        {
            if (_appSettingsProvider.AppSettings.IsRepeatForever) return;
            _timerViewService.StopAlarmSound();
            _timerViewService.CloseCurrentSessionFinishDialog();
        }

        public void Receive(FinishDialogCloseClickedMessage message)
        {
            _timerViewService.StopAlarmSound();
            _timerViewService.CloseCurrentSessionFinishBalloon();
        }

        public void Receive(TaskCreationInfoMessage message)
        {
            FlashMessageServiceViewModel.NewFlashMessage(
                message.IsSuccess ? FlashMessageType.SUCCESS : FlashMessageType.ERROR, message.Message);
        }


        private void TimerSession_OnCanPausedChanged(object? sender, bool canPause)
        {
            CanPause = canPause;
        }

        private void TimerSession_OnTimerSetForInitialized(object? sender, TimeSpan timerSetFor)
        {
            InitializeTimerProgress(timerSetFor);
        }

        private void TimerSession_OnCurrentSessionStateChanged(object? sender, TimerSessionState currentSessionState)
        {
            CurrentSessionState = currentSessionState;
        }

        private void TimerSession_OnRemainingTimeChanged(object? sender, RemainingTimeChangedEventArgs args)
        {
            _timeElapsed++;

            UpdateRemainingTime(args.RemainingTime);
            SessionProgress = CalculateProgress(args.TimerSetFor, args.RemainingTime);

            if (args.RemainingTime.TotalSeconds <= 0)
            {
                _timerSession.Finish();
            }
        }

        /// <summary>
        /// Do Finish logic.
        /// Give information to the listeners that session has finished. 
        /// View will handle its logic (display notification, alarm, etc).
        /// </summary>
        private async void TimerSession_OnSessionFinish(object? sender, PrevNextSessionsEventArgs args)
        {
            if (!_appSettingsProvider.AppSettings.IsAutostart)
            {
                _timerViewService.ShowSessionFinishDialog(args.FinishedSession, args.NextSession);

                if (_appSettingsProvider.AppSettings.PushNotificationEnabled)
                {
                    _timerViewService.ShowSessionFinishBalloonTips(args.FinishedSession, args.NextSession, 15000);
                }

                _timerViewService.PlayAlarmSound();
            }
            else
            {
                StartSession();
            }
          
            var today = DateOnly.FromDateTime(DateTime.Now);
            await StoreSessionFinishInfo(today, args.FinishedSession);

            PomodoroCompletedToday = await GetPomodoroCompletedToday();

            if (args.FinishedSession is PomodoroSessionState && SelectedTask != null)
            {
                await UpdateTaskActPomodoro(SelectedTask.Id, 1);
            }

            await StoreAndResetTimeElapsed(today, SelectedTask);
        }

        #endregion

        #region View Spesific

        private void UpdateRemainingTime(TimeSpan remainingTime)
        {
            _remainingTime = remainingTime;
            FormatRemainingTime();
        }

        private void InitializeTimerProgress(TimeSpan timerSetFor)
        {
            UpdateRemainingTime(timerSetFor);
            SessionProgress = CalculateProgress(timerSetFor, timerSetFor);
            Debug.Assert(SessionProgress == 0);
        }

        private void FormatRemainingTime()
        {
            RemainingTimeFormatted = _remainingTime.ToString(@"mm\:ss");
        }

        private double CalculateProgress(TimeSpan timerSetFor, TimeSpan remainingTime)
        {
            return (timerSetFor - remainingTime) / timerSetFor * 100;
        }

        #endregion

        #region Storing to Repository

        private async Task StoreSessionFinishInfo(DateOnly sessionDate,TimerSessionState currentSessionState) {
            int pomodoroCompleted = (currentSessionState is PomodoroSessionState) ? 1 : 0;
            int shortBreakCompleted = (currentSessionState is ShortBreakSessionState) ? 1 : 0;
            int longBreakCompleted = (currentSessionState is LongBreakSessionState) ? 1 : 0;

            var domain = new UpsertDailySessionDomain()
            {
                SessionDate = sessionDate,
                TotalPomodoroCompleted = pomodoroCompleted,
                TotalShortBreaksCompleted = shortBreakCompleted,
                TotalLongBreaksCompleted = longBreakCompleted,
                DailyPomodoroTarget = _appSettingsProvider.AppSettings.DailyPomodoroTarget
            };

            await _sessionsService.UpsertDailySession(domain);
        }

        private async Task StoreDailySessionTimeSpentInfo(DateOnly sessionDate, int timeSpent)
        {
            await _sessionsService.UpsertTimeSpent(
                DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromSeconds(timeSpent));
        }

        private async Task StoreAndResetTimeElapsed(DateOnly sessionDate, TaskDomainViewModel? task)
        {
            if(task != null)
            {
                await UpdateTaskTimeSpent(task.Id, _timeElapsed);

            }
            await StoreDailySessionTimeSpentInfo(sessionDate, _timeElapsed);
            _timeElapsed = 0; // we need to reset as the current time elapsed has been stored to the db
        }

        private async Task StoreDailySessionTotalTasksCompleted(DateOnly sessionDate, int totaltasksCompleted)
        {
            await _sessionsService.UpsertTotalTasksCompleted(sessionDate, totaltasksCompleted);
        }

        private async Task<int> GetPomodoroCompletedToday()
        {
            return await _sessionsService.GetTotalPomodoroCompleted(DateOnly.FromDateTime(DateTime.Now));
        }

        private async Task UpdateTaskTimeSpent(int taskId, int timeEllapsed)
        {
            await _tasksService.UpdateTimeSpent(taskId, TimeSpan.FromSeconds(timeEllapsed));
        }

        private async Task UpdateTaskStateToComplete(int taskId)
        {
            await _tasksService.UpdateTaskState(taskId, TaskState.COMPLETED);
        }

        private async Task UpdateTaskActPomodoro(int taskId, int totalPomodoroCompleted)
        {
            await _tasksService.UpdateActPomodoro(taskId, totalPomodoroCompleted);
        }

        #endregion

        private void AssignFromSettings()
        {
            DailyPomodoroTarget = _appSettingsProvider.AppSettings.DailyPomodoroTarget;
            _timerViewService.AlarmSound = _appSettingsProvider.AppSettings.AlarmSound;
            _timerViewService.SoundVolume = _appSettingsProvider.AppSettings.Volume;
            _timerViewService.IsAlarmRepeatForever = _appSettingsProvider.AppSettings.IsRepeatForever;
        }

        ~TimerViewModel()
        {
            _messenger.UnregisterAll(this);
            _timerSession.SessionFinish -= TimerSession_OnSessionFinish;
            _timerSession.CanPausedChanged -= TimerSession_OnCanPausedChanged;
            _timerSession.CurrentSessionStateChanged -= TimerSession_OnCurrentSessionStateChanged;
            _timerSession.RemainingTimeChanged -= TimerSession_OnRemainingTimeChanged;
            _timerSession.TimerSetForInitialized -= TimerSession_OnTimerSetForInitialized;
        }
    }
}
