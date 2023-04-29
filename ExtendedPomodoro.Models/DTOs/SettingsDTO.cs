﻿namespace ExtendedPomodoro.Models.DTOs
{
    public class SettingsDTO
    {
        public string SettingsType { get; set; } = "MAIN";
        public int PomodoroDuration { get; set; }
        public int ShortBreakDuration { get; set; }
        public int LongBreakDuration { get; set; }
        public int LongBreakInterval { get; set; }
        public int DailyPomodoroTarget { get; set; }
        public bool IsAutostart { get; set; }
        public int AlarmSound { get; set; }
        public int Volume { get; set; }
        public bool IsRepeatForever { get; set; }
        public bool PushNotificationEnabled { get; set; }
        public bool DarkModeEnabled { get; set; }
        public string? StartHotkey { get; set; }
        public string? PauseHotkey { get; set; }
    }
}
