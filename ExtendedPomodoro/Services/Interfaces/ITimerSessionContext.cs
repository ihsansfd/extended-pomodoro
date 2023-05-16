using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface ITimerSessionContext
    {
        TimerSessionState CurrentTimerSession { get; set; }
        void InitializeTimerProgress(TimeSpan timerSetFor);
        Task OnFinishSession(TimerSessionState finishedSession, TimerSessionState nextSession);
        bool CanPause { get; set; }
    }
}
