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

namespace ExtendedPomodoro.ViewModels
{
    public partial class TimerViewModel : ObservableObject,
        IRecipient<TimerTimeChangeInfoMessage>,
        IRecipient<SettingsUpdateInfoMessage>,
        IRecipient<StartSessionInfoMessage>
    {
        private readonly IAppSettingsProvider _appSettingsProvider;
        private readonly IDailySessionsService _sessionsRepository;
        private readonly ITasksService _tasksRepository;
        private readonly IMessenger _messenger;
        private readonly TimerViewService _timerViewService;
        private bool _hasBeenSetup;

        private int _timeElapsed = 0;

        public TimerViewModel(
            ReadTasksViewModel readTasksViewModel,
            CreateTaskViewModel createTaskViewModel,
            TimerSessionState timerSessionState,
            TimerViewService timerViewService,
            IAppSettingsProvider appSettingsProvider,
            IDailySessionsService sessionsRepository,
            ITasksService tasksRepository,
            IMessenger messenger
            )
        {

            ReadTasksViewModel = readTasksViewModel;
            CreateTaskViewModel = createTaskViewModel;
            CurrentTimerSession = timerSessionState;
            _appSettingsProvider = appSettingsProvider;
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
        private TimerSessionState _currentTimerSession;

        [ObservableProperty]
        private bool _canPause;

        [ObservableProperty]
        private string _remainingTimeFormatted;

        [ObservableProperty]
        private int _dailyPomodoroTarget;

        [RelayCommand]
        private void StartSession() {
            CurrentTimerSession.Start();
            _timerViewService.PlayMouseClickEffectSound(_appSettingsProvider.AppSettings.Volume);
        }

        [RelayCommand]
        private void PauseSession()
        {
            CurrentTimerSession.Pause();
            _timerViewService.PlayMouseClickEffectSound(_appSettingsProvider.AppSettings.Volume);
        }

        [RelayCommand]
        private void SkipSession() => CurrentTimerSession.Skip();

        [RelayCommand]
        private void ResetSession() => CurrentTimerSession.Reset();

        public void StartSessionFromHotkey(object? sender, HotkeyEventArgs e)
        {
            CurrentTimerSession.Start();
            _timerViewService.PlayMouseClickEffectSound(_appSettingsProvider.AppSettings.Volume);
            if(_appSettingsProvider.AppSettings.PushNotificationEnabled)
            {
                _timerViewService.ShowTimerStartedBalloonTips(CurrentTimerSession);
            }
        }

        public void PauseSessionFromHotkey(object? sender, HotkeyEventArgs e)
        {
            CurrentTimerSession.Pause();
            _timerViewService.PlayMouseClickEffectSound(_appSettingsProvider.AppSettings.Volume);

            if(_appSettingsProvider.AppSettings.PushNotificationEnabled)
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
            DailyPomodoroTarget = _appSettingsProvider.AppSettings.DailyPomodoroTarget;
            if (!_hasBeenSetup) CurrentTimerSession.InitialSetup(this, _appSettingsProvider, _messenger);
            _hasBeenSetup = true;
        }

        #endregion

        #region messages and events

        public void Receive(TimerTimeChangeInfoMessage message)
        {
            _timeElapsed++;

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
            DailyPomodoroTarget = message.AppSettings.DailyPomodoroTarget;
        }

        /// <summary>
        /// Do Finish logic.
        /// Give information to the listeners that session has finished. 
        /// View will handle its logic (display notification, alarm, etc).
        /// </summary>
        public async Task OnFinishSession(TimerSessionState finishedSession, TimerSessionState nextSession)
        {
            if (!_appSettingsProvider.AppSettings.IsAutostart)
            {
                _timerViewService.OpenSessionFinishDialog(finishedSession, nextSession);

                if (_appSettingsProvider.AppSettings.PushNotificationEnabled)
                {
                    _timerViewService.ShowSessionFinishBalloonTips(finishedSession, nextSession);
                }

                _timerViewService.PlaySound((AlarmSound)
                    _appSettingsProvider.AppSettings.AlarmSound, 
                    _appSettingsProvider.AppSettings.Volume,
                    _appSettingsProvider.AppSettings.IsRepeatForever ? 
                    Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(15));
                _timerViewService.AutoCloseDialogAndSound = 
                    !_appSettingsProvider.AppSettings.IsRepeatForever;
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

            await StoreAndResetTimeElapsed(today, SelectedTask);
        }

        public void Receive(StartSessionInfoMessage message)
        {
            if (message.IsStartSession) StartSession();
        }

        [RelayCommand]
        private async Task HandleWindowClosed()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            await StoreAndResetTimeElapsed(today, SelectedTask);
        }

        #endregion

        #region View Spesific

        public void UpdateRemainingTime(TimeSpan remainingTime)
        {
            _remainingTime = remainingTime;
            FormatRemainingTime();
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
                TotalLongBreaksCompleted = longBreakCompleted
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

        ~TimerViewModel()
        {
            _messenger.UnregisterAll(this);
        }
    }


    public class TimerSessionState
    {
        public virtual string Name { get; } = string.Empty;
        public virtual string SessionMessage { get; } = string.Empty;

        private static bool _isRunning;
        private static bool _isPaused;
        private static TimerViewModel _context = null!;
        private static IMessenger _messenger = null!;
        protected static IAppSettingsProvider Configuration = null!;
        
        private static readonly ExtendedTimer Timer = new();
        protected static readonly PomodoroSessionState PomodoroSessionState = new();
        protected static readonly ShortBreakSessionState ShortBreakSessionState = new();
        protected static readonly LongBreakSessionState LongBreakSessionState = new();

        
        // Need to be called before anything else
        public void InitialSetup(
            TimerViewModel context, 
            IAppSettingsProvider configuration,
            IMessenger messenger
            )
        {
            _context = context;
            Configuration = configuration;
            _messenger = messenger;
            _context.CurrentTimerSession = PomodoroSessionState;
            
            _context.CurrentTimerSession.Initialize();
            
            Timer.RemainingTimeChanged += TimerOnRemainingTimeChanged;
        }

        private void TimerOnRemainingTimeChanged(object? sender, RemainingTimeChangedEventArgs e)
        {
            _messenger.Send(new TimerTimeChangeInfoMessage(e.TimerSetFor, e.RemainingTime));
        }

        public void Start()
        {
            if (AlreadyStarting()) return;

            _isPaused = false;

            if (_isRunning)
            {
                Timer.Resume();
                OnCanPauseChange();
                return;
            }

            _isRunning = true;
            Timer.Start();
            OnCanPauseChange();
        }

        public void Pause()
        {
            if (_isPaused) return;

            if (!_isRunning) return;
            _isPaused = true;
            Timer.Pause();

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
            if (Configuration.AppSettings.IsAutostart) _context.CurrentTimerSession.Start();
        }

        private void StopFromRunning()
        {
            Timer.Stop();
            _isRunning = false;
        }
        
        protected void InitializeTo(TimeSpan timerSetFor)
        {
            Timer.Initialize(timerSetFor);
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

        private void OnCanPauseChange()
        {
            _context.CanPause = !_isPaused && _isRunning;
        }
    }

    public class PomodoroSessionState : TimerSessionState
    {
        public override string Name { get; } = "Pomodoro";
        public override string SessionMessage { get; } = "Stay Focus";

        private static int _totalPomodoroCompletedSkipIncluded  = 0;

        public override void Initialize()
        {
            base.Initialize();
            var timerSetFor = Configuration.AppSettings.PomodoroDuration;
            InitializeTo(timerSetFor);
        }

        public override void Finish()
        {
            base.Finish();
            _totalPomodoroCompletedSkipIncluded++;

            if(IsSwitchToLongBreak())
            {
                FinishTo(LongBreakSessionState);
            }
          
            else
            {
                FinishTo(ShortBreakSessionState);
            }

            base.AfterFinish();
        }

        public override void Skip()
        {
            base.Skip();
            _totalPomodoroCompletedSkipIncluded++;
            if (IsSwitchToLongBreak()) SkipTo(LongBreakSessionState);
            else SkipTo(ShortBreakSessionState);
        }

        private bool IsSwitchToLongBreak()
        {
            return _totalPomodoroCompletedSkipIncluded %
                Configuration.AppSettings.LongBreakInterval == 0;
        }
    }

    public class ShortBreakSessionState : TimerSessionState
    {
        public override string Name { get; } = "Short Break";
        public override string SessionMessage { get; } = "Short Break";

        public override void Initialize()
        {
            base.Initialize();
            var timerSetFor = Configuration.AppSettings.ShortBreakDuration;
            InitializeTo(timerSetFor);
        }

        public override void Finish()
        {
            base.Finish();
            FinishTo(PomodoroSessionState);
            base.AfterFinish();
        }

        public override void Skip()
        {
            base.Skip();
            SkipTo(PomodoroSessionState);
        }
    }

    public class LongBreakSessionState : TimerSessionState
    {
        public override string Name { get; } = "Long Break";
        public override string SessionMessage { get; } = "Long Break";

        public override void Initialize()
        {
            var timerSetFor = Configuration.AppSettings.LongBreakDuration;
            InitializeTo(timerSetFor);
        }

        public override void Finish()
        {
            base.Finish();
            FinishTo(PomodoroSessionState);
            base.AfterFinish();
        }

        public override void Skip()
        {
            base.Skip();
            SkipTo(PomodoroSessionState);
        }
    }
}
