using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;

namespace ExtendedPomodoro.ViewServices.Interfaces
{
    public interface ITimerViewService
    {
        public AlarmSound AlarmSound { set; }
        public bool IsAlarmRepeatForever { set; }
        public int SoundVolume { set; }
        public void PlayMouseClickEffectSound();
        public void PlayAlarmSound();
        public void StopAlarmSound();
        public void ShowSessionFinishDialog(TimerSessionState finishedSession,
            TimerSessionState nextSession);
        public void ShowSessionFinishBalloonTips(TimerSessionState finishedSession,
            TimerSessionState nextSession, int autoCloseAfterInMillSeconds);
        public void ShowTimerStartedBalloonTips(TimerSessionState currentSession,
            int autoCloseAfterInMillSeconds = 5000);
        public void ShowTimerPausedBalloonTips(TimerSessionState currentSession, int autoCloseAfterInMillSeconds = 5000);
        public void CloseCurrentSessionFinishDialog();
        public void CloseCurrentSessionFinishBalloon();
    }
}
