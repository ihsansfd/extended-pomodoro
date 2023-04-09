using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public record class HotkeyDomain(int ModifierKeys, int Key);

    public record class SettingsDomain(
        TimeSpan PomodoroDuration,
        TimeSpan ShortBreakDuration,
        TimeSpan LongBreakDuration,
        int LongBreakInterval,
        int DailyPomodoroTarget,
        bool IsAutostart,
        AlarmSound AlarmSound,
        int Volume,
        bool IsRepeatForever,
        bool PushNotificationEnabled,
        bool DarkModeEnabled,
        HotkeyDomain? StartHotkey,
        HotkeyDomain? PauseHotkey
     );
}
