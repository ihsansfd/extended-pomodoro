using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Proxies;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;
using ExtendedPomodoro.ViewServices;
using NHotkey;
using System;
using System.Threading;
using System.Threading.Tasks;
using ExtendedPomodoro.ViewModels.Interfaces;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TimerViewModel : 
        ObservableObject, 
        INavigableViewModel,
        IRecipient<SettingsUpdateInfoMessage>,
        IRecipient<StartSessionInfoMessage>,
        IRecipient<StartHotkeyTriggeredMessage>,
        IRecipient<PauseHotkeyTriggeredMessage>,
        IRecipient<MainWindowIsClosingMessage>
    {
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IDailySessionsService _sessionsRepository;
        private readonly ITasksService _tasksRepository;
        private readonly IMessenger _messenger;
        private readonly TimerViewService _timerViewService;
        private readonly ITimerSession _timerSession;

        private int _timeElapsed = 0;

        public TimerViewModel(
            ReadTasksViewModel readTasksViewModel,
            CreateTaskViewModel createTaskViewModel,
            ITimerSession timerSession,
            TimerViewService timerViewService,
            IAppSettingsProvider appSettingsProvider,
            IDailySessionsService sessionsRepository,
            ITasksService tasksRepository,
            IMessenger messenger
            )
        {

            ReadTasksViewModel = readTasksViewModel;
            CreateTaskViewModel = createTaskViewModel;
            _timerSession = timerSession;
            _appSettingsProvider = appSettingsProvider;
            _timerViewService = timerViewService;
            _sessionsRepository = sessionsRepository;
            _tasksRepository = tasksRepository;
            _messenger = messenger;
            _messenger.RegisterAll(this);

            _timerSession.SessionFinish += TimerSession_OnSessionFinish;
            _timerSession.CurrentSessionStateChanged += TimerSession_OnCurrentSessionStateChanged;
            _timerSession.TimerSetForInitialized += TimerSession_OnTimerSetForInitialized;
            _timerSession.CanPausedChanged += TimerSession_OnCanPausedChanged;
            _timerSession.RemainingTimeChanged += TimerSession_OnRemainingTimeChanged;
            CurrentTimerSession = _timerSession.CurrentSessionState;
            _timerSession.Initialize();
        }

        #region Tasks

        public ReadTasksViewModel ReadTasksViewModel { get; }
        public CreateTaskViewModel CreateTaskViewModel { get; }

        [ObservableProperty]
        private TaskDomainViewModel? _selectedTask;

        [ObservableProperty]
        private int _pomodoroCompletedToday;

        [ObservableProperty]
        private bool _isTasksDropdownOpen;

        [RelayCommand]
        private void AddNewTaskModal()
        {
            CreateTaskViewModel.IsModalShown = true;
            IsTasksDropdownOpen = false;
        }

        [RelayCommand]
        private async Task CompleteTask()
        {
            if (SelectedTask == null) return;
            var today = DateOnly.FromDateTime(DateTime.Now);

            await UpdateTaskStateToComplete(SelectedTask.Id);
            await StoreAndResetTimeElapsed(today, SelectedTask);
            await StoreDailySessionTotalTasksCompleted(today, 1);
            await ReadTasksViewModel.LoadTasks();

            SelectedTask = null;
        }

        [RelayCommand]
        private async Task CancelTask()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            await StoreAndResetTimeElapsed(today, SelectedTask);
            SelectedTask = null;
        }

        #endregion

        #region Timer variables, props, and commands

        private TimeSpan _remainingTime;
        
        [ObservableProperty] // from 0 to 100, use converter to set it backwards
        private double _sessionProgress = 0.0;

        [ObservableProperty]
        private TimerSessionState _currentTimerSession = null!;

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

        public void StartSessionFromHotkey()
        {
            _timerSession.Start();
            _timerViewService.PlayMouseClickEffectSound();
            if(_appSettingsProvider.AppSettings.PushNotificationEnabled)
            {
                _timerViewService.ShowTimerStartedBalloonTips(CurrentTimerSession);
            }
        }

        public void PauseSessionFromHotkey()
        {
            _timerSession.Pause();
            _timerViewService.PlayMouseClickEffectSound();

            if(_appSettingsProvider.AppSettings.PushNotificationEnabled)
            {
                _timerViewService.ShowTimerPausedBalloonTips(CurrentTimerSession);
            }
        }

        #endregion

        #region bootstrap

        public async Task Load()
        {
            await ReadTasksViewModel.DisplayInProgressTasks();
            PomodoroCompletedToday = await GetPomodoroCompletedToday();
            AssignFromSettings();
        }

        #endregion

        #region messages and events


        private void TimerSession_OnCanPausedChanged(object? sender, bool canPause)
        {
            CanPause = canPause;
        }

        private void TimerSession_OnTimerSetForInitialized(object? sender, TimeSpan timerSetFor)
        {
            InitializeTimerProgress(timerSetFor);
        }

        private void TimerSession_OnCurrentSessionStateChanged(object? sender, TimerSessionState currSessionState)
        {
            CurrentTimerSession = currSessionState;
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

        /// <summary>
        /// Do Finish logic.
        /// Give information to the listeners that session has finished. 
        /// View will handle its logic (display notification, alarm, etc).
        /// </summary>
        public async void TimerSession_OnSessionFinish(object? sender, PrevNextSessionsEventArgs args)
        {
            if (!_appSettingsProvider.AppSettings.IsAutostart)
            {
                _timerViewService.OpenSessionFinishDialog(args.FinishedSession, args.NextSession);

                if (_appSettingsProvider.AppSettings.PushNotificationEnabled)
                {
                    _timerViewService.ShowSessionFinishBalloonTips(args.FinishedSession, args.NextSession, 15000);
                }

                _timerViewService.PlayAlarmSound();
                _timerViewService.AutoCloseDialogAndSound = 
                    !_appSettingsProvider.AppSettings.IsRepeatForever;
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

        public void Receive(StartSessionInfoMessage message)
        {
            if (message.IsStartSession) StartSession();
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
            SessionProgress = CalculateProgress(timerSetFor, timerSetFor); // should always return 100
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

            await _sessionsRepository.UpsertDailySession(domain);
        }

        private async Task StoreDailySessionTimeSpentInfo(DateOnly sessionDate, int timeSpent)
        {
            await _sessionsRepository.UpsertTimeSpent(
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
            await _sessionsRepository.UpsertTotalTasksCompleted(sessionDate, totaltasksCompleted);
        }

        private async Task<int> GetPomodoroCompletedToday()
        {
            return await _sessionsRepository.GetTotalPomodoroCompleted(DateOnly.FromDateTime(DateTime.Now));
        }

        private async Task UpdateTaskTimeSpent(int taskId, int timeEllapsed)
        {
            await _tasksRepository.UpdateTimeSpent(taskId, TimeSpan.FromSeconds(timeEllapsed));
        }

        private async Task UpdateTaskStateToComplete(int taskId)
        {
            await _tasksRepository.UpdateTaskState(taskId, TaskState.COMPLETED);
        }

        private async Task UpdateTaskActPomodoro(int taskId, int totalPomodoroCompleted)
        {
            await _tasksRepository.UpdateActPomodoro(taskId, totalPomodoroCompleted);
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
