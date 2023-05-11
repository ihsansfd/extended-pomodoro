using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.ViewModels.Interfaces;

namespace ExtendedPomodoro.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INavigableViewModel _timerViewModel;
        private readonly INavigableViewModel _statsViewModel;
        private readonly INavigableViewModel _tasksViewModel;
        private readonly INavigableViewModel _settingsViewModel;
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private INavigableViewModel _currentViewModel;

        public MainWindowViewModel(
            INavigableViewModel timerViewModel,
            INavigableViewModel tasksViewModel,
            INavigableViewModel statsViewModel,
            INavigableViewModel settingsViewModel,       
            IMessenger messenger
            )
        {
            _timerViewModel = timerViewModel;
            _settingsViewModel = settingsViewModel;
            _tasksViewModel = tasksViewModel;
            _statsViewModel = statsViewModel;
            _messenger = messenger;
        }

        public async Task Initialize()
        {
            await NavigateToTimer();
        }

        [RelayCommand]
        private async Task NavigateToTimer()
        {
            CurrentViewModel = _timerViewModel;
            await CurrentViewModel.Load();
        }

        [RelayCommand]
        private async Task NavigateToStats()
        {
            CurrentViewModel = _statsViewModel;
            await CurrentViewModel.Load();
        }

        [RelayCommand]
        private async Task NavigateToSettings() {
            CurrentViewModel = _settingsViewModel;
             await CurrentViewModel.Load();
         }

        [RelayCommand]
        private async Task NavigateToTasks()
        {
            CurrentViewModel = _tasksViewModel;
            await CurrentViewModel.Load();
        }

        [RelayCommand]
        private void HandleWindowClosed()
        {
            _messenger.Send(new MainWindowIsClosingMessage());
        }
    }
}
