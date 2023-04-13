using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ExtendedPomodoro.Services
{
    public static class MainWindowService
    {
        public static MainWindow ItsMainWindow { get;} = new MainWindow();
        public static Border BorderBlocker = (Border) FindName("BorderBlocker");

        private static object FindName(string Name)
        {
            return ItsMainWindow.FindName(Name);
        }

        public static void ActivateBorderBlocker()
        {
            BorderBlocker.Visibility = System.Windows.Visibility.Visible;
        }

        public static void DeactivateBorderBlocker()
        {
            BorderBlocker.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
