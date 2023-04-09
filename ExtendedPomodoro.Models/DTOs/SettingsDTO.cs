using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.DTOs
{
    public class SqlSettingsDTO {
        public string SettingsType { get; } = "MAIN";
        public int PomodoroDuration {get;}
        public int ShortBreakDuration {get;}
        public int LongBreakDuration {get;}
        public int LongBreakInterval {get;}
        public int DailyPomodoroTarget {get;}
        public bool IsAutostart {get;}
        public int AlarmSound {get;}
        public int Volume {get;}
        public bool IsRepeatForever {get;}
        public bool PushNotificationEnabled {get;}
        public bool DarkModeEnabled {get;}
        public string? StartHotkey {get;}
        public string? PauseHotkey { get; }
    }
}
