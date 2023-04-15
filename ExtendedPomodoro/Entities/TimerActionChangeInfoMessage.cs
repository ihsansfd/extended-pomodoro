using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Entities
{
    public enum TimerAction
    {
        Start = 0,
        Pause = 1,
        Reset = 2,
        Skip = 3,
    }

    public record class TimerActionChangeInfoMessage(TimerSessionState CurrentSession,
        TimerAction TimerAction, bool TriggeredByHotkey = false, bool PushNotificationEnabled = true);
}
