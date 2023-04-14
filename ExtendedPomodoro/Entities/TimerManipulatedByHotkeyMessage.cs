using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Entities
{
    public enum HotkeyManipulation
    {
        Start,
        Pause
    }

    public record class TimerManipulatedByHotkeyMessage(TimerSessionState CurrentSession, HotkeyManipulation Manipulation);
}
