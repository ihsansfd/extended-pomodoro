using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using ExtendedPomodoro.ViewServices.Interfaces;

namespace ExtendedPomodoro.ViewServices
{
    public class StatsViewService : IStatsViewService
    {
        private readonly DialogWindowService _dialogWindowService;
        private readonly IMessenger _messenger;

        public StatsViewService(
            DialogWindowService dialogWindowService,
            IMessenger messenger
            )
        {
            _dialogWindowService = dialogWindowService;
            _messenger = messenger;

        }

        public void OpenScatterPlotStats(ChartDataDomainViewModel axes)
        {
            var userControl = new ScatterPlotStatsUserControl
            {
                Axes = axes
            };
            userControl.Load();

            var dialog = _dialogWindowService.GenerateMaximizableDialogWindow(
                new(800, 450, "Fullscreen Stats", userControl));
            dialog.Show();
            dialog.Focus();
            dialog.Activate();
        }

    }
}
