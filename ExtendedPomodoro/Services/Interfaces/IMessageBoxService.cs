using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string messageBoxText);
        MessageBoxResult Show(string messageBoxText, string caption);
        MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button);
        MessageBoxResult Show(string messageBoxText, string caption, 
            MessageBoxButton button, MessageBoxImage icon);
    }
}
