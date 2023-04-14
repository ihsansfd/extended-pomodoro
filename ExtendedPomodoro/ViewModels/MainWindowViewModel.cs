﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtendedPomodoro.ViewModels
{

    public partial class MainWindowViewModel : ObservableObject, 
        IRecipient<CurrentViewModelMessage>
    {
        private readonly NavigationViewModel _navigationViewModel;
        public TimerViewModel TimerViewModel { get; }
        private readonly StatsViewModel _statsViewModel;
        private readonly TasksViewModel _tasksViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public MainWindowViewModel(NavigationViewModel navigationViewModel,
          TimerViewModel timerViewModel, SettingsViewModel settingsViewModel, StatsViewModel statsViewModel, TasksViewModel tasksViewModel)
        {
            _navigationViewModel = navigationViewModel;
            TimerViewModel = timerViewModel;
            _settingsViewModel = settingsViewModel;
            _tasksViewModel = tasksViewModel;
            _statsViewModel = statsViewModel;

            StrongReferenceMessenger.Default.RegisterAll(this);
        }

        public async Task Initialize()
        {
            await NavigateToTimer();

        }

        [RelayCommand]
        public async Task NavigateToTimer()
        {
            _navigationViewModel.SetCurrentViewModel(TimerViewModel);
            await TimerViewModel.Initialize();
        }

        [RelayCommand]
        public void NavigateToStats() => _navigationViewModel.SetCurrentViewModel(_statsViewModel);

        [RelayCommand]

        public async Task NavigateToSettings() { 
             _navigationViewModel.SetCurrentViewModel(_settingsViewModel);
             await _settingsViewModel.Initialize();
         }

        [RelayCommand]
        public async Task NavigateToTasks()
        {
            _navigationViewModel.SetCurrentViewModel(_tasksViewModel);
            await _tasksViewModel.ReadTasksViewModel.LoadTasks();
        }

        public void Receive(CurrentViewModelMessage currentViewModelMessage) => CurrentViewModel = currentViewModelMessage.Message;
    }
}
