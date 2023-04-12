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
        IRecipient<TimerTimeChangeInfoMessage>,
        IRecipient<SettingsUpdateInfoMessage>,
        IRecipient<StartSessionInfoMessage>
    {
        private SettingsViewModel _settingsViewModel;

        private bool _hasBeenSetup;

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
            if(!_hasBeenSetup) await CurrentTimerSession.InitialSetup(this, _settingsViewModel);
            _hasBeenSetup = true;
        }

        public void Receive(TimerTimeChangeInfoMessage message)
        {
            UpdateRemainingTime(message.RemainingTime);
            SessionProgress = CalculateProgress(message.TimerSetFor, message.RemainingTime);

            if(message.RemainingTime.TotalSeconds <= 0)
            {
                CurrentTimerSession.Finish();
            }

        }

        public void UpdateRemainingTime(TimeSpan remainingTime)
        {
            RemainingTime = remainingTime;
            FormatRemainingTime();
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
        /// Give information to the listeners that session has finished. View will handle its logic (display notification, etc).
        /// </summary>
        /// <param name="message"></param>
        public void OnFinishSession(TimerSessionFinishInfoMessage message)
        {
            StrongReferenceMessenger.Default.Send(message);
        }

        private void FormatRemainingTime()
        {
            RemainingTimeFormatted = RemainingTime.ToString(@"mm\:ss");
        }

        private double CalculateProgress(TimeSpan timerSetFor, TimeSpan remainingTime)
        {
            return (timerSetFor - remainingTime) / timerSetFor * 100;
        }

        public void Receive(StartSessionInfoMessage message)
        {
            if(message.IsStartSession) StartSession();
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

        protected void StopFromRunning()
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
            _totalPomodoroCompleted++;
            _totalPomodoroCompletedSkipIncluded++;

            if(IsSwitchToLongBreak())
            {
                SwitchToLongBreakSession();
                _context.OnFinishSession(new(
                "Pomodoro session has completed.", "Pomodoro", "Long Break", _configuration.PushNotificationEnabled));
            }
          
            else
            {
                SwitchToShortBreakSession();
                _context.OnFinishSession(new(
                "Pomodoro session has completed.", "Pomodoro", "Short Break", _configuration.PushNotificationEnabled));
            }
        }

        public override void Skip()
        {
            base.Skip();
            _totalPomodoroCompletedSkipIncluded++;
            if (IsSwitchToLongBreak()) SwitchToLongBreakSession();
            else SwitchToShortBreakSession();
        }

        private bool IsSwitchToLongBreak()
        {
            return _totalPomodoroCompletedSkipIncluded % _configuration.LongBreakInterval == 0;
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
            _context.OnFinishSession(new(
                "Short Break session has completed.", "Short Break", "Pomodoro", _configuration.PushNotificationEnabled));
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
            _context.OnFinishSession(new(
                "Long Break session has completed.", "Long Break", "Pomodoro", _configuration.PushNotificationEnabled));
        }

        public override void Skip()
        {
            base.Skip();
            SwitchToPomodoroSession();
        }
    }
}
