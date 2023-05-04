using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Entities
{
    public record class AppSettings
    {
        public TimeSpan PomodoroDuration { get; set; }
        public TimeSpan ShortBreakDuration { get; set; }
        public TimeSpan LongBreakDuration { get; set; }
        public int LongBreakInterval { get; set; }
        public int DailyPomodoroTarget { get; set; }
        public bool IsAutostart { get; set; }
        public AlarmSound AlarmSound { get; set; }
        public int Volume { get; set; }
        public bool IsRepeatForever { get; set; }
        public bool PushNotificationEnabled { get; set; }
        public bool DarkModeEnabled { get; set; }
        public Hotkey? StartHotkey { get; set; }
        public Hotkey? PauseHotkey { get; set; }
    }
}
