using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using System.Windows.Input;
using ExtendedPomodoro.ViewModels;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ExtendedPomodoro.Helpers
{

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
