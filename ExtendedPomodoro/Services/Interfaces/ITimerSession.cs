using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface ITimerSession
    {
        TimerSessionState CurrentSessionState { get; }
        PomodoroSessionState PomodoroSessionState { get; }
        ShortBreakSessionState ShortBreakSessionState { get; }
        LongBreakSessionState LongBreakSessionState { get; }

        event EventHandler<RemainingTimeChangedEventArgs> RemainingTimeChanged;
        event EventHandler<TimeSpan>? TimerSetForInitialized;
        event EventHandler<PrevNextSessionsEventArgs>? SessionFinish;
        event EventHandler<bool>? CanPausedChanged;
        event EventHandler<TimerSessionState>? CurrentSessionStateChanged;

        void Start();
        void Pause();
        void Reset();
        void Initialize();
        void Skip();
        void Finish();

    }
}
