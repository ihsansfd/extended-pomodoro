using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TimerViewModel : ObservableObject
    {
        public ReadTasksViewModel ReadTasksViewModel { get; }
        public CreateTaskViewModel CreateTaskViewModel { get; }

        public TimerViewModel(ReadTasksViewModel readTasksViewModel, CreateTaskViewModel createTaskViewModel)
        {
            ReadTasksViewModel = readTasksViewModel;
            CreateTaskViewModel = createTaskViewModel;
        }

        public async Task InstantiateTasks()
        {
            await ReadTasksViewModel.DisplayInProgressTasks();
        }

        [RelayCommand]
        public void NotifTasksComboBoxAddNewButtonPressed() => 
            StrongReferenceMessenger.Default.Send(new TasksComboBoxAddNewButtonPressedMessage());
    }

}
