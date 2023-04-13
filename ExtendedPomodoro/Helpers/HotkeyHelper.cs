using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtendedPomodoro.Helpers
{
    public class HotkeyHelper
    {
    }

    public static class HotkeyExtension
    {
        public static Hotkey? ConvertToHotkey(this HotkeyDomain? hotkeyDomain)
        {
            if (hotkeyDomain == null) return null;

            return new Hotkey((Key)hotkeyDomain.Key, (ModifierKeys)hotkeyDomain.ModifierKeys);
        }

        public static HotkeyDomain? ConvertToHotkeyDomain(this Hotkey? hotkey)
        {
            if (hotkey == null) return null;

            return new HotkeyDomain((int)hotkey.Modifiers, (int)hotkey.Key);
        }
    }
}
