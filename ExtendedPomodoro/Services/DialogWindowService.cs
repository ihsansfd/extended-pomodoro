using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ExtendedPomodoro.Services
{
    //public enum DialogContent
    //{
    //    ModalSessionFinish = 0,
    //    ModalCreateTask = 1,
    //    ModalEditTask  = 2,
    //    ScatterPlotStats = 3,
    //}

    public record class DialogServiceDomain(double Width, double Height, string Title,
        FrameworkElement Content);

    public class DialogWindowService
    {
        private Window GenerateMaximizibleDialogWindow(DialogServiceDomain domain)
        {
            var window = new MaximizableWindow();
            window.Width = domain.Width;
            window.Height = domain.Height;
            window.Title = domain.Title;
            window.Content = domain.Content;
            return window;
        }

        private Window GenerateMinimizibleDialogWindow(DialogServiceDomain domain)
        {
            var window = new Window();
            window.Width = domain.Width;
            window.Height = domain.Height;
            window.Title = domain.Title;
            window.Content = domain.Content;
            return window;
        }

        public void OpenScatterPlotStats(StatAxesDomainViewModel axes)
        {
            var userControl = new ScatterPlotStatsUserControl
            {
                Axes = axes
            };
            userControl.Initialize();

            var dialog = GenerateMaximizibleDialogWindow(new(800, 450, "Fullscreen Stats", 
                userControl));
            dialog.ShowDialog();
        }


        //public GenerateDialog(DialogServiceDomain domain)
        //{
        //    var window = new Window();
        //    window.Width = domain.Width;
        //    window.Height = domain.Height;
        //    window.Title = domain.Title;
        //    window.Content = domain.Content;
        //    window.ShowDialog();
        //}
    }
}
