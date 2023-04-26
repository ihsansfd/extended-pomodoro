using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ExtendedPomodoro.Services
{

    public record class MaximizableDialogServiceDomain(double Width, double Height, string Title,
        FrameworkElement Content, bool IsLogoShown = false);

    public record class ClosableDialogServiceDomain(double Width, double Height, string Title,
        FrameworkElement Content, bool IsLogoShown = false, bool IsOKButtonShown = true);

    public class DialogWindowService
    {
        public Window GenerateMaximizibleDialogWindow(MaximizableDialogServiceDomain domain)
        {
            var window = new MaximizableWindow() { IsLogoShown = domain.IsLogoShown };
            window.Width = domain.Width;
            window.Height = domain.Height;
            window.Title = domain.Title;
            window.Content = domain.Content;
            window.Owner = MainWindowService.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window;
        }

        public Window GenerateClosableDialogWindow(ClosableDialogServiceDomain domain)
        {
            var window = new ClosableWindow() { 
                IsLogoShown = domain.IsLogoShown, 
                IsOKButtonShown = domain.IsOKButtonShown };
            window.Width = domain.Width;
            window.Height = domain.Height;
            window.Title = domain.Title;
            window.Content = domain.Content;
            window.Owner = MainWindowService.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            return window;
        }
    }
}
