using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewServices;
using NHotkey;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TimerViewModel : ObservableObject,
        IRecipient<TimerTimeChangeInfoMessage>,
        IRecipient<SettingsUpdateInfoMessage>,
        IRecipient<StartSessionInfoMessage>
    {
        public SettingsViewModel SettingsViewModel { get; }
        private readonly IDailySessionsService _sessionsRepository;
        private readonly ITasksService _tasksRepository;
        private readonly IMessenger _messenger;
        private readonly TimerViewService _timerViewService;
        private bool _hasBeenSetup;

        private int _timeEllapsed = 0;

        public TimerViewModel(ReadTasksViewModel readTasksViewModel,
            CreateTaskViewModel createTaskViewModel,
            TimerSessionState timerSessionState,
            SettingsViewModel settingsViewModel,
            TimerViewService timerViewService,
            IDailySessionsService sessionsRepository,
            ITasksService tasksRepository,
            IMessenger messenger
            )
        {

            ReadTasksViewModel = readTasksViewModel;
            CreateTaskViewModel = createTaskViewModel;
            CurrentTimerSession = timerSessionState;
            SettingsViewModel = settingsViewModel;
            _timerViewService = timerViewService;
            _sessionsRepository = sessionsRepository;
            _tasksRepository = tasksRepository;
            _messenger = messenger;

            _messenger.RegisterAll(this);
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
        public void AddNewTaskModal()
        {
            CreateTaskViewModel.IsModalShown = true;
            IsTasksDropdownOpen = false;
        }

        [RelayCommand]
        public async Task CompleteTask()
        {
            if (SelectedTask == null) return;
            var today = DateOnly.FromDateTime(DateTime.Now);

            await UpdateTaskStateToComplete(SelectedTask.Id);
            await StoreAndResetTimeEllapsed(today, SelectedTask);
            await StoreDailySessionTotalTasksCompleted(today, 1);
            await ReadTasksViewModel.LoadTasks();

            SelectedTask = null;
        }

        [RelayCommand]
        public async Task CancelTask()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            await StoreAndResetTimeEllapsed(today, SelectedTask);
            SelectedTask = null;
        }

        #endregion

        #region Timer variables, props, and commands

        public TimeSpan RemainingTime;

        public bool NeedSessionSetup { get; set; } = true;

        [ObservableProperty] // from 0 to 100, use converter to set it backwards
        private double _sessionProgress = 0.0;

        [ObservableProperty]
        private TimerSessionState _currentTimerSession;

        [ObservableProperty]
        private bool _canPause;

        [ObservableProperty]
        private string _remainingTimeFormatted;

        [RelayCommand]
        public void StartSession() {
            CurrentTimerSession.Start();
            _timerViewService.PlayMouseClickEffectSound(SettingsViewModel.Volume);
        }

        [RelayCommand]
        public void PauseSession()
        {
            CurrentTimerSession.Pause();
            _timerViewService.PlayMouseClickEffectSound(SettingsViewModel.Volume);
        }

        [RelayCommand]
        public void SkipSession() => CurrentTimerSession.Skip();

        [RelayCommand]
        public void ResetSession() => CurrentTimerSession.Reset();

        public void StartSessionFromHotkey(object? sender, HotkeyEventArgs e)
        {
            CurrentTimerSession.Start();
            _timerViewService.PlayMouseClickEffectSound(SettingsViewModel.Volume);
            if(SettingsViewModel.PushNotificationEnabled)
            {
                _timerViewService.ShowTimerStartedBalloonTips(CurrentTimerSession);
            }
        }

        public void PauseSessionFromHotkey(object? sender, HotkeyEventArgs e)
        {
            CurrentTimerSession.Pause();
            _timerViewService.PlayMouseClickEffectSound(SettingsViewModel.Volume);

            if(SettingsViewModel.PushNotificationEnabled)
            {
                _timerViewService.ShowTimerPausedBalloonTips(CurrentTimerSession);
            }
        }

        #endregion

        #region bootstrap

        public async Task Initialize()
        {
            await ReadTasksViewModel.DisplayInProgressTasks();
            PomodoroCompletedToday = await GetPomodoroCompletedToday();
            if (!_hasBeenSetup) await CurrentTimerSession.InitialSetup(this, SettingsViewModel);
            _hasBeenSetup = true;
        }

        #endregion

        #region messages and events

        public void Receive(TimerTimeChangeInfoMessage message)
        {
            _timeEllapsed++;

            UpdateRemainingTime(message.RemainingTime);
            SessionProgress = CalculateProgress(message.TimerSetFor, message.RemainingTime);

            if (message.RemainingTime.TotalSeconds <= 0)
            {
                CurrentTimerSession.Finish();
            }

        }

        /// <summary>
        /// If there are changes in settings while the timer hasn't run, reinitialized.
        /// </summary>
        /// <param name="message"></param>
        public void Receive(SettingsUpdateInfoMessage message)
        {
            if (SessionProgress <= 0.0) CurrentTimerSession.Initialize();
        }

        /// <summary>
        /// Do Finish logic.
        /// Give information to the listeners that session has finished. 
        /// View will handle its logic (display notification, alarm, etc).
        /// </summary>
        /// <param name="message"></param>
        public async Task OnFinishSession(TimerSessionState finishedSession, TimerSessionState nextSession)
        {
            if (!SettingsViewModel.IsAutostart)
            {
                _timerViewService.OpenSessionFinishDialog(finishedSession, nextSession);

                if (SettingsViewModel.PushNotificationEnabled)
                {
                    _timerViewService.ShowSessionFinishBalloonTips(finishedSession, nextSession);
                }
                _timerViewService.PlaySound((AlarmSound)SettingsViewModel.AlarmSound, SettingsViewModel.Volume,
                    SettingsViewModel.IsRepeatForever ? Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(15));
                _timerViewService.AutoCloseDialogAndSound = SettingsViewModel.IsRepeatForever ? false : true;
            }
            else
            {
                StartSession();
            }
          
            var today = DateOnly.FromDateTime(DateTime.Now);
            await StoreSessionFinishInfo(today, finishedSession);

            PomodoroCompletedToday = await GetPomodoroCompletedToday();

            if (finishedSession is PomodoroSessionState && SelectedTask != null)
            {
                await UpdateTaskActPomodoro(SelectedTask.Id, 1);
            }

            await StoreAndResetTimeEllapsed(today, SelectedTask);
        }

        public void Receive(StartSessionInfoMessage message)
        {
            if (message.IsStartSession) StartSession();
        }

        [RelayCommand]
        public async Task HandleWindowClosed()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            await StoreAndResetTimeEllapsed(today, SelectedTask);
        }

        #endregion

        #region View Spesific

        public void UpdateRemainingTime(TimeSpan remainingTime)
        {
            RemainingTime = remainingTime;
            FormatRemainingTime();
        }

        private void FormatRemainingTime()
        {
            RemainingTimeFormatted = RemainingTime.ToString(@"mm\:ss");
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
                TotalLongBreaksCompleted = longBreakCompleted
            };

            await _sessionsRepository.UpsertDailySession(domain);
        }

        private async Task StoreDailySessionTimeSpentInfo(DateOnly sessionDate, int timeSpent)
        {
            await _sessionsRepository.UpsertTimeSpent(
                DateOnly.FromDateTime(DateTime.Now), TimeSpan.FromSeconds(timeSpent));
        }

        private async Task StoreAndResetTimeEllapsed(DateOnly sessionDate, TaskDomainViewModel? task)
        {
            if(task != null)
            {
                await UpdateTaskTimeSpent(task.Id, _timeEllapsed);

            }
            await StoreDailySessionTimeSpentInfo(sessionDate, _timeEllapsed);
            _timeEllapsed = 0; // we need to reset as the current timeellapsed has been stored to the db
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

        ~TimerViewModel()
        {
            _messenger.UnregisterAll(this);
        }
    }


    public class TimerSessionState
    {
        public virtual string Name { get; } = string.Empty;
        public virtual string SessionMessage { get; } = string.Empty;

        protected static bool _isRunning { get; set; }
        protected static bool _isPaused { get; set; }
        protected static TimerViewModel _context;
        protected static SettingsViewModel _configuration;

        protected static ExtendedTimer _timer { get; } = new();
        protected static PomodoroSessionState _pomodoroSessionState = new();
        protected static ShortBreakSessionState _shortBreakSessionState = new();
        protected static LongBreakSessionState _longBreakSessionState = new();

        // Need to be called before anything else
        public async Task InitialSetup(TimerViewModel context, SettingsViewModel configuration)
        {
            _context = context;
            _configuration = configuration;
            await _configuration.Initialize();
            _context.CurrentTimerSession = _pomodoroSessionState;
            _context.CurrentTimerSession.Initialize();
        }

        public void Start()
        {
            if (AlreadyStarting()) return;

            _isPaused = false;

            if (_isRunning)
            {
                _timer.Resume();
                OnCanPauseChange();
                return;
            }

            _isRunning = true;
            _timer.Start();
            OnCanPauseChange();
        }

        public void Pause()
        {
            if (_isPaused) return;

            if (!_isRunning) return;
            _isPaused = true;
            _timer.Pause();

            OnCanPauseChange();
        }

        public void Reset()
        {
            StopFromRunning();
            Initialize();
            OnCanPauseChange();
        }

        public virtual void Initialize() { }

        public virtual void Skip()
        {
            StopFromRunning();
            OnCanPauseChange();
        }

        public virtual void Finish()
        {
            StopFromRunning();
            OnCanPauseChange();
        }

        protected void AfterFinish()
        {
            if (_configuration.IsAutostart) _context.CurrentTimerSession.Start();
        }

        protected void StopFromRunning()
        {
            _timer.Stop();
            _isRunning = false;
        }
        
        protected void InitializeTo(TimeSpan timerSetFor)
        {
            _timer.Initialize(timerSetFor);
            _context.UpdateRemainingTime(timerSetFor);
        }

        protected async void FinishTo(TimerSessionState nextSession)
        {
            await _context.OnFinishSession(_context.CurrentTimerSession, nextSession);
            SwitchTo(nextSession);
        }

        protected void SkipTo(TimerSessionState nextSession)
        {
            SwitchTo(nextSession);
        }

        private void SwitchTo(TimerSessionState session)
        {
            _context.CurrentTimerSession = session;
            _context.CurrentTimerSession.Initialize();
        }
        
        private bool AlreadyStarting()
        {
            return !_isPaused && _isRunning;
        }

        public void OnCanPauseChange()
        {
            _context.CanPause = !_isPaused && _isRunning;
        }
    }

    public class PomodoroSessionState : TimerSessionState
    {
        public override string Name { get; } = "Pomodoro";
        public override string SessionMessage { get; } = "Stay Focus";

        private static int _totalPomodoroCompleted { get; set; } = 0;
        private static int _totalPomodoroCompletedSkipIncluded { get; set; } = 0;

        public override void Initialize()
        {
            base.Initialize();
            var timerSetFor = TimeSpan.FromMinutes(_configuration.PomodoroDurationInMinutes);
            InitializeTo(timerSetFor);
        }

        public override void Finish()
        {
            base.Finish();
            _totalPomodoroCompleted++;
            _totalPomodoroCompletedSkipIncluded++;

            if(IsSwitchToLongBreak())
            {
                FinishTo(_longBreakSessionState);
            }
          
            else
            {
                FinishTo(_shortBreakSessionState);
            }

            base.AfterFinish();
        }

        public override void Skip()
        {
            base.Skip();
            _totalPomodoroCompletedSkipIncluded++;
            if (IsSwitchToLongBreak()) SkipTo(_longBreakSessionState);
            else SkipTo(_shortBreakSessionState);
        }

        private bool IsSwitchToLongBreak()
        {
            return _totalPomodoroCompletedSkipIncluded % _configuration.LongBreakInterval == 0;
        }
    }

    public class ShortBreakSessionState : TimerSessionState
    {
        public override string Name { get; } = "Short Break";
        public override string SessionMessage { get; } = "Short Break";

        public override void Initialize()
        {
            base.Initialize();
            var timerSetFor = TimeSpan.FromMinutes(_configuration.ShortBreakDurationInMinutes);
            InitializeTo(timerSetFor);
        }

        public override void Finish()
        {
            base.Finish();
            FinishTo(_pomodoroSessionState);
            base.AfterFinish();
        }

        public override void Skip()
        {
            base.Skip();
            SkipTo(_pomodoroSessionState);
        }
    }

    public class LongBreakSessionState : TimerSessionState
    {
        public override string Name { get; } = "Long Break";
        public override string SessionMessage { get; } = "Long Break";

        public override void Initialize()
        {
            var timerSetFor = TimeSpan.FromMinutes(_configuration.LongBreakDurationInMinutes);
            InitializeTo(timerSetFor);
        }

        public override void Finish()
        {
            base.Finish();
            FinishTo(_pomodoroSessionState);
            base.AfterFinish();
        }

        public override void Skip()
        {
            base.Skip();
            SkipTo(_pomodoroSessionState);
        }
    }
}
