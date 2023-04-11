using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TimerViewModel : ObservableObject, 
        IRecipient<TimerTimeChangeInfoMessage>
    {
        private SettingsViewModel _settingsViewModel;

        #region Tasks

        public ReadTasksViewModel ReadTasksViewModel { get; }
        public CreateTaskViewModel CreateTaskViewModel { get; }

        [RelayCommand]
        public void NotifTasksComboBoxAddNewButtonPressed() =>
            StrongReferenceMessenger.Default.Send(new TasksComboBoxAddNewButtonPressedMessage());

        #endregion

        #region Timer

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

        [ObservableProperty]
        private string _sessionMessage;

        [RelayCommand]
        public void StartSession() => CurrentTimerSession.Start();

        [RelayCommand]
        public void PauseSession() => CurrentTimerSession.Pause();

        [RelayCommand]
        public void SkipSession() => CurrentTimerSession.Skip();

        [RelayCommand]
        public void ResetSession() => CurrentTimerSession.Reset();

        #endregion

        public TimerViewModel(ReadTasksViewModel readTasksViewModel, 
            CreateTaskViewModel createTaskViewModel, 
            TimerSessionState timerSessionState,
            SettingsViewModel settingsViewModel)
        {

            ReadTasksViewModel = readTasksViewModel;
            CreateTaskViewModel = createTaskViewModel;
            CurrentTimerSession = timerSessionState;
            _settingsViewModel = settingsViewModel;
            StrongReferenceMessenger.Default.RegisterAll(this);
        }

        public async Task Initialize()
        {
            await ReadTasksViewModel.DisplayInProgressTasks();
            if(NeedSessionSetup)
            {
                await CurrentTimerSession.InitialSetup(this, _settingsViewModel);
                NeedSessionSetup = false;
            }
        }

        public void Receive(TimerTimeChangeInfoMessage message)
        {
            UpdateRemainingTime(message.RemainingTime);
            SessionProgress = CalculateProgress(message.TimerSetFor, message.RemainingTime);

        }

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
    }


    public class TimerSessionState
    {
        protected bool _isRunning { get; set; }
        protected bool _isPaused { get; set; }
        protected static TimerViewModel _context;
        protected static SettingsViewModel _configuration;

        protected static ExtendedTimer _timer { get; } = new();
        protected static PomodoroSessionState _pomodoroSessionState = new();
        protected static ShortBreakSessionState _shortBreakSessionState = new();
        protected static LongBreakSessionState _longBreakSessionState = new();

        public async Task InitialSetup(TimerViewModel context, SettingsViewModel configuration)
        {
            _context = context;
            _configuration = configuration;
            await _configuration.Initialize();
            _context.CurrentTimerSession = _pomodoroSessionState;
            _context.CurrentTimerSession.Initialize();
        }

        public virtual void Initialize() { }

        public virtual void Start()
        {
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

        public virtual void Pause()
        {
            if (!_isRunning) return;
            _isPaused = true;
            _timer.Pause();

            OnCanPauseChange();
        }

        public virtual void Reset()
        {
            StopFromRunning();
            Initialize();
            OnCanPauseChange();
        }

        public virtual void Skip()
        {
            StopFromRunning();
            OnCanPauseChange();
        }

        public virtual void Finish()
        {
            StopFromRunning();
        }

        protected virtual void StopFromRunning()
        {
            _timer.Stop();
            _isRunning = false;
        }
        protected void SwitchToPomodoroSession()  {
             _context.CurrentTimerSession = _pomodoroSessionState;
             _context.CurrentTimerSession.Initialize();
        }
        protected void SwitchToShortBreakSession()
        {
            _context.CurrentTimerSession = _shortBreakSessionState;
            _context.CurrentTimerSession.Initialize();
        }
        protected void SwitchToLongBreakSession()
        {
            _context.CurrentTimerSession = _longBreakSessionState;
            _context.CurrentTimerSession.Initialize();
        }

        public void OnCanPauseChange()
        {
            _context.CanPause = !_isPaused && _isRunning;
        }
    }

    public class PomodoroSessionState : TimerSessionState
    {
        private static int _totalPomodoroCompleted { get; set; } = 0;
        private static int _totalPomodoroCompletedSkipIncluded { get; set; } = 0;

        public override void Initialize()
        {
            var timerSetFor = TimeSpan.FromMinutes(_configuration.PomodoroDurationInMinutes);
            _timer.Initialize(timerSetFor);
            _context.UpdateRemainingTime(timerSetFor);
            _context.SessionMessage = "Stay Focus";
        }

        public override void Finish()
        {
            base.Finish();
        }

        public override void Skip()
        {
            base.Skip();
            _totalPomodoroCompletedSkipIncluded++;
            if (_totalPomodoroCompletedSkipIncluded % 4 == 0) SwitchToLongBreakSession();
            else SwitchToShortBreakSession();
        }
    }

    public class ShortBreakSessionState : TimerSessionState
    {

        public override void Initialize()
        {
            var timerSetFor = TimeSpan.FromMinutes(_configuration.ShortBreakDurationInMinutes);
            _timer.Initialize(timerSetFor);
            _context.UpdateRemainingTime(timerSetFor);
            _context.SessionMessage = "Short Break";
        }

        public override void Finish()
        {
            base.Finish();
            SwitchToPomodoroSession();
        }

        public override void Skip()
        {
            base.Skip();
            SwitchToPomodoroSession();
        }
    }

    public class LongBreakSessionState : TimerSessionState
    {
        public override void Initialize()
        {
            var timerSetFor = TimeSpan.FromMinutes(_configuration.LongBreakDurationInMinutes);
            _timer.Initialize(timerSetFor);
            _context.UpdateRemainingTime(timerSetFor);
            _context.SessionMessage = "Long Break";
        }

        public override void Finish()
        {
            base.Finish();
            SwitchToPomodoroSession();
        }

        public override void Skip()
        {
            base.Skip();
            SwitchToPomodoroSession();
        }
    }
}
