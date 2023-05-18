using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using ExtendedPomodoro.Messages;

namespace ExtendedPomodoro.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly object _timerViewModel;
        private readonly object _statsViewModel;
        private readonly object _tasksViewModel;
        private readonly object _settingsViewModel;
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private object _currentViewModel;

        public MainWindowViewModel(
            object timerViewModel,
            object tasksViewModel,
            object statsViewModel,
            object settingsViewModel,       
            IMessenger messenger
            )
        {
            _timerViewModel = timerViewModel;
            _settingsViewModel = settingsViewModel;
            _tasksViewModel = tasksViewModel;
            _statsViewModel = statsViewModel;
            _messenger = messenger;
        }

        public void Initialize()
        {
            NavigateToTimer();
        }

        [RelayCommand]
        private void NavigateToTimer()
        {
            CurrentViewModel = _timerViewModel;
        }

        [RelayCommand]
        private void NavigateToStats()
        {
            CurrentViewModel = _statsViewModel;
        }

        [RelayCommand]
        private void NavigateToSettings() {
            CurrentViewModel = _settingsViewModel;
         }

        [RelayCommand]
        private void NavigateToTasks()
        {
            CurrentViewModel = _tasksViewModel;
        }

        [RelayCommand]
        private void HandleWindowClosed()
        {
            _messenger.Send(new MainWindowIsClosingMessage());
        }
    }
}
