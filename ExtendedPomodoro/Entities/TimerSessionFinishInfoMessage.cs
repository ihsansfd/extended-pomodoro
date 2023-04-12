using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Entities
{
    public record class TimerSessionFinishInfoMessage(TimerSessionState FinishedSession,
        TimerSessionState NextSession, bool PushNotificationEnabled);
}
