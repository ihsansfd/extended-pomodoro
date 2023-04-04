using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewModels
{

    public partial class MainWindowViewModel : ObservableObject, IRecipient<CurrentViewModelMessage>
    {
        private readonly NavigationViewModel _navigationViewModel;
        private readonly TimerViewModel _timerViewModel;
        private readonly StatsViewModel _statsViewModel;
        private readonly TasksViewModel _tasksViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        [RelayCommand]
        public void NavigateToTimer() => _navigationViewModel.SetCurrentViewModel(_timerViewModel);

        [RelayCommand]
        public void NavigateToStats() => _navigationViewModel.SetCurrentViewModel(_statsViewModel); 

        [RelayCommand]

        public void NavigateToSettings() => _navigationViewModel.SetCurrentViewModel(_settingsViewModel);

        [RelayCommand]
        public async Task NavigateToTasks()
        {
            _navigationViewModel.SetCurrentViewModel(_tasksViewModel);
            await _tasksViewModel.ReadTasksViewModel.LoadTasks();
        }

        public void Receive(CurrentViewModelMessage currentViewModelMessage) => CurrentViewModel = currentViewModelMessage.Message;

        public MainWindowViewModel(NavigationViewModel navigationViewModel, 
            TimerViewModel timerViewModel, SettingsViewModel settingsViewModel, StatsViewModel statsViewModel, TasksViewModel tasksViewModel)
        {
            _navigationViewModel = navigationViewModel;
            CurrentViewModel = _navigationViewModel.CurrentViewModel;

            _timerViewModel = timerViewModel;
            _settingsViewModel = settingsViewModel;
            _tasksViewModel = tasksViewModel;
            _statsViewModel = statsViewModel;

            StrongReferenceMessenger.Default.Register<CurrentViewModelMessage>(this);
        }

    }
}
