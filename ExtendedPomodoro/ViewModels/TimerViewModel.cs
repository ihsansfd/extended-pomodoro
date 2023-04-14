using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Services;
using NHotkey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TimerViewModel : ObservableObject,
        IRecipient<TimerTimeChangeInfoMessage>,
        IRecipient<SettingsUpdateInfoMessage>,
        IRecipient<StartSessionInfoMessage>
    {
        private readonly SettingsViewModel _settingsViewModel;
        private readonly ISessionsRepository _sessionsRepository;
        private readonly ITasksRepository _tasksRepository;
        private bool _hasBeenSetup;

        private int _timeEllapsed = 0;

        #region Tasks

        public ReadTasksViewModel ReadTasksViewModel { get; }
        public CreateTaskViewModel CreateTaskViewModel { get; }

        [ObservableProperty]
        private TaskDomainViewModel? _selectedTask;

        [RelayCommand]
        public void NotifTasksComboBoxAddNewButtonPressed() =>
            StrongReferenceMessenger.Default.Send(new TasksComboBoxAddNewButtonPressedMessage());

        [RelayCommand]
        public async Task CompleteTask()
        {
            if (SelectedTask == null) return;

            var _selectedTaskTemp = SelectedTask;
            SelectedTask = null;

            await StoreSessionFinishInfo(CurrentTimerSession, _timeEllapsed);
            await UpdateTaskStateToComplete(_selectedTaskTemp.Id);
            await StoreTaskTimeSpent(_selectedTaskTemp.Id, _timeEllapsed);
            await ReadTasksViewModel.LoadTasks();
            _timeEllapsed = 0; // we need to reset as the current timeellapsed has been stored to the db
        }

        [RelayCommand]
        public async Task CancelTask()
        {
            if(SelectedTask == null) return;

            var _selectedTaskTemp = SelectedTask;
            SelectedTask = null;
            await StoreSessionFinishInfo(CurrentTimerSession, _timeEllapsed);
            await StoreTaskTimeSpent(_selectedTaskTemp.Id, _timeEllapsed);
            _timeEllapsed = 0; // we need to reset as the current timeellapsed has been stored to the db
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
            StrongReferenceMessenger.Default.Send(new TimerActionChangeInfoMessage(CurrentTimerSession, TimerAction.Start, false));
        }

        [RelayCommand]
        public void PauseSession()
        {
            CurrentTimerSession.Pause();
            StrongReferenceMessenger.Default.Send(new TimerActionChangeInfoMessage(CurrentTimerSession, TimerAction.Pause, false));
        }

        [RelayCommand]
        public void SkipSession() => CurrentTimerSession.Skip();

        [RelayCommand]
        public void ResetSession() => CurrentTimerSession.Reset();

        [RelayCommand]
        public void StartSessionFromHotkey() => CurrentTimerSession.Start();

        [RelayCommand]
        public void PauseSessionFromHotkey() => CurrentTimerSession.Pause();

        #endregion

        #region bootstrap

        public TimerViewModel(ReadTasksViewModel readTasksViewModel, 
            CreateTaskViewModel createTaskViewModel, 
            TimerSessionState timerSessionState,
            SettingsViewModel settingsViewModel,
            ISessionsRepository sessionsRepository,
            ITasksRepository tasksRepository
            )
        {

            ReadTasksViewModel = readTasksViewModel;
            CreateTaskViewModel = createTaskViewModel;
            CurrentTimerSession = timerSessionState;
            _settingsViewModel = settingsViewModel;
            _sessionsRepository = sessionsRepository;
            _tasksRepository = tasksRepository;

            StrongReferenceMessenger.Default.RegisterAll(this);
        }

        public async Task Initialize()
        {
            await ReadTasksViewModel.DisplayInProgressTasks();
            if(!_hasBeenSetup) await CurrentTimerSession.InitialSetup(this, _settingsViewModel);
            _hasBeenSetup = true;
        }

        #endregion

        #region messages and events

        public void OnStartTimerByHotkey(object? sender, HotkeyEventArgs e)
        {
            CurrentTimerSession.Start();
            StrongReferenceMessenger.Default.Send(new TimerActionChangeInfoMessage(CurrentTimerSession, TimerAction.Start, true));
        }

        public void OnPauseTimerByHotkey(object? sender, HotkeyEventArgs e)
        {
            CurrentTimerSession.Pause();
            StrongReferenceMessenger.Default.Send(new TimerActionChangeInfoMessage(CurrentTimerSession, TimerAction.Pause, true));
        }

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
        public async Task OnFinishSession(TimerSessionFinishInfoMessage message)
        {
            StrongReferenceMessenger.Default.Send(message);

            await StoreSessionFinishInfo(message.FinishedSession, _timeEllapsed);
            if(SelectedTask != null) await StoreTaskTimeSpent(SelectedTask.Id, _timeEllapsed);

            _timeEllapsed = 0; // we need to reset as the current timeellapsed has been stored to the db
        }

        public void Receive(StartSessionInfoMessage message)
        {
            if (message.IsStartSession) StartSession();
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

        private async Task StoreSessionFinishInfo(
            TimerSessionState currentSessionState, int timeSpent)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            int pomodoroCompleted = (currentSessionState is PomodoroSessionState) ? 1 : 0;
            int shortBreakCompleted = (currentSessionState is ShortBreakSessionState) ? 1 : 0;
            int longBreakCompleted = (currentSessionState is LongBreakSessionState) ? 1 : 0;

            var domain = new UpsertDailySessionDomain(
                today,
                TimeSpan.FromSeconds(timeSpent),
                pomodoroCompleted,
                shortBreakCompleted,
                longBreakCompleted
                );

            await _sessionsRepository.UpsertDailySession(domain);
        }

        private async Task StoreTaskTimeSpent(int taskId, int timeEllapsed)
        {
            await _tasksRepository.UpdateTaskTimeSpent(taskId, TimeSpan.FromSeconds(timeEllapsed));
        }

        private async Task UpdateTaskStateToComplete(int taskId)
        {
            await _tasksRepository.UpdateTaskState(taskId, TaskState.COMPLETED);
        }

        #endregion
    }


    public class TimerSessionState
    {
        public virtual string Name { get; } = string.Empty;
        public virtual string SessionMessage { get; } = string.Empty;

        protected bool _isRunning { get; set; }
        protected bool _isPaused { get; set; }
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
            await _context.OnFinishSession(new
                (_context.CurrentTimerSession, 
                nextSession, 
                (AlarmSound)_configuration.AlarmSound,
                _configuration.PushNotificationEnabled, 
                _configuration.IsRepeatForever,
                _configuration.Volume
                ));

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
