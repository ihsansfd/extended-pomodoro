namespace ExtendedPomodoro.Models.Domains
{
    public enum SettingsType
    {
        MAIN = 0, 
        DEFAULT =1,
    }

    public enum AlarmSound
    {
        Chimes = 0,
        Echo = 1,
        Mechanics = 2,
        Retro = 3,
        Ticks = 4,
    }

    public enum SoundEffect
    {
        MouseClick = 0
    }

    public record class HotkeyDomain(int ModifierKeys, int Key);

    public record class SettingsDomain
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
        public HotkeyDomain? StartHotkeyDomain { get; set; }
        public HotkeyDomain? PauseHotkeyDomain { get; set; }
    };
}
