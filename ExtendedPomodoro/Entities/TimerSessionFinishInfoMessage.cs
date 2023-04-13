using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Entities
{
    public record class TimerSessionFinishInfoMessage(
        TimerSessionState FinishedSession,
        TimerSessionState NextSession, 
        AlarmSound AlarmSound, 
        bool PushNotificationEnabled,
        bool IsAlarmSoundRepeatForever,
        int AlarmVolume
        );
}
