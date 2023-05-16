using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Services.Interfaces;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services;

public class PrevNextSessionsEventArgs : EventArgs
{
    public PrevNextSessionsEventArgs(TimerSessionState finishedSession, TimerSessionState nextSession)
    {
        FinishedSession = finishedSession;
        NextSession = nextSession;
    }

    public TimerSessionState FinishedSession { get; }
    public TimerSessionState NextSession { get; }
}

public class TimerSession : ITimerSession
{
    public TimerSessionState CurrentSessionState
    {
        get => _currentSessionState;

        private set 
        {
            _currentSessionState = value;
            CurrentSessionStateChanged?.Invoke(this, _currentSessionState);
        }
    }
    public PomodoroSessionState PomodoroSessionState { get; }
    public ShortBreakSessionState ShortBreakSessionState { get; }
    public LongBreakSessionState LongBreakSessionState { get; }
    public IAppSettingsProvider Configuration { get; }
    public IExtendedTimer Timer { get; }

    public event EventHandler<RemainingTimeChangedEventArgs> RemainingTimeChanged
    {
        add => Timer.RemainingTimeChanged += value;
        remove => Timer.RemainingTimeChanged -= value;
    }
    public event EventHandler<TimerSessionState>? CurrentSessionStateChanged; 
    public event EventHandler<TimeSpan>? TimerSetForInitialized;
    public event EventHandler<PrevNextSessionsEventArgs>? SessionFinish;
    public event EventHandler<bool>? CanPausedChanged;

    private TimerSessionState _currentSessionState;
    private bool _isRunning;
    private bool _isPaused;

    public TimerSession(IAppSettingsProvider configuration, IExtendedTimer timer)
    {
        Configuration = configuration;
        Timer = timer;
        PomodoroSessionState = new PomodoroSessionState(this);
        ShortBreakSessionState = new ShortBreakSessionState(this);
        LongBreakSessionState = new LongBreakSessionState(this);

        _currentSessionState = PomodoroSessionState;
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

    public void Initialize()
    {
        CurrentSessionState.Initialize();
    }

    public virtual void Skip()
    {
        StopFromRunning();
        OnCanPauseChange();
        CurrentSessionState.Skip();
    }

    public virtual void Finish()
    {
        StopFromRunning();
        OnCanPauseChange();
        CurrentSessionState.Finish();
        if (Configuration.AppSettings.IsAutostart) Start();

    }

    public void InitializeTo(TimeSpan timerSetFor)
    {
        Timer.Initialize(timerSetFor);
        TimerSetForInitialized?.Invoke(this, timerSetFor);
    }

    public void SkipTo(TimerSessionState nextSession) => SwitchTo(nextSession);

    public void FinishTo(TimerSessionState nextSession)
    {
        SessionFinish?.Invoke(this, new PrevNextSessionsEventArgs(CurrentSessionState, nextSession));
        SwitchTo(nextSession);
    }

    private void StopFromRunning()
    {
        Timer.Stop();
        _isRunning = false;
    }


    private void SwitchTo(TimerSessionState session)
    {
        CurrentSessionState = session;
        CurrentSessionState.Initialize();
    }

    private bool AlreadyStarting()
    {
        return !_isPaused && _isRunning;
    }

    private void OnCanPauseChange()
    {
        CanPausedChanged?.Invoke(this, !_isPaused && _isRunning);
    }

}

public abstract class TimerSessionState
{
    protected TimerSession Context;

    protected TimerSessionState(TimerSession context) => Context = context;

    public abstract string Name { get; }
    public abstract string SessionMessage { get; }

    public abstract void Initialize();
    public abstract void Finish();
    public abstract void Skip();

}

public class PomodoroSessionState : TimerSessionState
{
    public override string Name => "Pomodoro";
    public override string SessionMessage => "Stay Focus";

    private static int _totalPomodoroCompletedSkipIncluded = 0;

    public PomodoroSessionState(TimerSession context) : base(context) { }

    public override void Initialize()
    {
        var timerSetFor = Context.Configuration.AppSettings.PomodoroDuration;
        Context.InitializeTo(timerSetFor);
    }

    public override void Finish()
    {
        _totalPomodoroCompletedSkipIncluded++;

        if (IsSwitchToLongBreak())
        {
            Context.FinishTo(Context.LongBreakSessionState);
        }

        else
        {
            Context.FinishTo(Context.ShortBreakSessionState);
        }

    }

    public override void Skip()
    {
        _totalPomodoroCompletedSkipIncluded++;
        if (IsSwitchToLongBreak()) Context.SkipTo(Context.LongBreakSessionState);
        else Context.SkipTo(Context.ShortBreakSessionState);
    }

    private bool IsSwitchToLongBreak()
    {
        return _totalPomodoroCompletedSkipIncluded %
            Context.Configuration.AppSettings.LongBreakInterval == 0;
    }
}

public class ShortBreakSessionState : TimerSessionState
{
    public override string Name => "Short Break";
    public override string SessionMessage => "Short Break";

    public ShortBreakSessionState(TimerSession context) : base(context) { }

    public override void Initialize()
    {
        var timerSetFor = Context.Configuration.AppSettings.ShortBreakDuration;
        Context.InitializeTo(timerSetFor);
    }

    public override void Finish()
    {
        Context.FinishTo(Context.PomodoroSessionState);
    }

    public override void Skip()
    {
        Context.SkipTo(Context.PomodoroSessionState);
    }

}

public class LongBreakSessionState : TimerSessionState
{
    public override string Name => "Long Break";
    public override string SessionMessage => "Long Break";

    public LongBreakSessionState(TimerSession context) : base(context) { }

    public override void Initialize()
    {
        var timerSetFor = Context.Configuration.AppSettings.LongBreakDuration;
        Context.InitializeTo(timerSetFor);
    }

    public override void Finish()
    {
        Context.FinishTo(Context.PomodoroSessionState);
    }

    public override void Skip()
    {
        Context.SkipTo(Context.PomodoroSessionState);
    }
}