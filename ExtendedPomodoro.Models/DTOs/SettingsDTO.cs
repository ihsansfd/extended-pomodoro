namespace ExtendedPomodoro.Models.DTOs
{
    public class SettingsDTO
    {
        public string SettingsType { get; set; } = "MAIN";
        public int PomodoroDurationInSeconds { get; set; }
        public int ShortBreakDurationInSeconds { get; set; }
        public int LongBreakDurationInSeconds { get; set; }
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
