using NHotkey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface IHotkeyManager
    {
        void AddOrReplace(string name, Key key, ModifierKeys modifiers,
            EventHandler<HotkeyEventArgs> handler);

        void Remove(string name);
    }
}
