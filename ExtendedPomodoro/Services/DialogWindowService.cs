using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Views.Components;
using System.Windows;

namespace ExtendedPomodoro.Services
{
    public record MaximizableDialogServiceDomain(double Width, double Height, string Title,
        FrameworkElement Content, bool IsLogoShown = false);

    public record ClosableDialogServiceDomain(double Width, double Height, string Title,
        FrameworkElement Content, bool IsLogoShown = false, bool IsOKButtonShown = true);

    public class DialogWindowService
    {
        public Window GenerateMaximizableDialogWindow(MaximizableDialogServiceDomain domain)
        {
            var window = new MaximizableWindow
            {
                IsLogoShown = domain.IsLogoShown,
                Width = domain.Width,
                Height = domain.Height,
                Title = domain.Title,
                Content = domain.Content,
                Owner = MainWindowFactory.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            return window;
        }

        public Window GenerateClosableDialogWindow(ClosableDialogServiceDomain domain)
        {
            var window = new ClosableWindow
            {
                IsLogoShown = domain.IsLogoShown,
                IsOKButtonShown = domain.IsOKButtonShown,
                Width = domain.Width,
                Height = domain.Height,
                Title = domain.Title,
                Content = domain.Content,
                Owner = MainWindowFactory.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            return window;
        }
    }
}
