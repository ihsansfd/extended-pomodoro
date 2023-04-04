using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TasksViewModel : ObservableObject
    {
        public CreateTaskViewModel CreateTaskViewModel { get;}

        public ReadTasksViewModel ReadTasksViewModel { get;}

        public TasksViewModel(CreateTaskViewModel createTaskViewModel, ReadTasksViewModel readTaskViewModel) {
            CreateTaskViewModel = createTaskViewModel;
            ReadTasksViewModel = readTaskViewModel;
            CreateTaskViewModel.NewTaskCreated += async () => await ReadTasksViewModel.OnNewTaskCreated();
        }

    }

    public class ReadTasksViewModel : ObservableObject
    {
       private readonly ITasksRepository _repository;
       public ObservableCollection<TaskDomain> Tasks { get; } = new();

       public ReadTasksViewModel(ITasksRepository repository)
        {
            _repository = repository;
        }

        public async Task LoadTasks()
        {
            ClearTasks();
            try
            {
                await foreach (var record in _repository.GetTasks())
                {
                    Tasks.Add(record);
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        public void ClearTasks()
        {
            Tasks.Clear();
        }

        internal async Task OnNewTaskCreated()
        {
            await LoadTasks();
        }
    }

    public partial class CreateTaskViewModel : ObservableValidator
    {
        private readonly ITasksRepository _repository;

        public CreateTaskViewModel(ITasksRepository repository)
        {
            _repository = repository;
        }

        public event Action? NewTaskCreated;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Task name is required")]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        [CustomValidation(typeof(CreateTaskViewModel), nameof(ValidateEstPomodoro))]
        private string _estPomodoro;

        public static ValidationResult ValidateEstPomodoro(string estPomodoro, ValidationContext context)
        {
            return estPomodoro == null || int.TryParse(estPomodoro, out _) ? 
                ValidationResult.Success : new("Est. Pomodoro must be an integer");
        }

        [RelayCommand]
        public async Task CreateTask()
        {
            ValidateAllProperties();

            if(!HasErrors)
            {
                int estPomodoro;

                if (EstPomodoro == null) estPomodoro = 0;
                else int.TryParse(EstPomodoro, out estPomodoro);

                await _repository.CreateTask(new(Name, Description, estPomodoro));

                NewTaskCreated?.Invoke();
            }
        }


    }
}
