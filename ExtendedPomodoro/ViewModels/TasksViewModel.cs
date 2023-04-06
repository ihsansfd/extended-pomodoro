using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Helpers;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro.ViewModels
{
    public class TasksViewModel : ObservableObject
    {

        public ReadTasksViewModel ReadTasksViewModel { get;}

        public CreateTaskViewModel CreateTaskViewModel { get; }

        public UpdateTaskViewModel UpdateTaskViewModel { get; }

        public DeleteTaskViewModel DeleteTaskViewModel { get;}

        public TasksViewModel(ReadTasksViewModel readTaskViewModel, 
            UpdateTaskViewModel updateTaskViewModel,
            CreateTaskViewModel createTaskViewModel, 
            DeleteTaskViewModel deleteTaskViewModel)
        {
            ReadTasksViewModel = readTaskViewModel;
            CreateTaskViewModel = createTaskViewModel;
            UpdateTaskViewModel = updateTaskViewModel;
            DeleteTaskViewModel = deleteTaskViewModel;
        }

    }


    public partial class ReadTasksViewModel : ObservableObject, 
        IRecipient<TaskDeletionInfoMessage>, IRecipient<TaskCreationInfoMessage>, IRecipient<TaskUpdateStateInfoMessage>
    {
       private readonly ITasksRepository _repository;
       private readonly TasksHelper _helper;
       public ObservableCollection<TaskDomainViewModel> Tasks { get; } = new();

       [ObservableProperty]
       private bool _isDisplayingCompletedTasks = false; // true = completed tasks, false = in progress tasks

       public ReadTasksViewModel(ITasksRepository repository, TasksHelper helper)
        {
            _repository = repository;
            _helper = helper;
            StrongReferenceMessenger.Default.RegisterAll(this);
        }

        [RelayCommand]
        public async Task DisplayInProgressTasks()
        {
            IsDisplayingCompletedTasks = false;
            await LoadTasks();
        }

        [RelayCommand]
        public async Task DisplayCompletedTasks()
        {
            IsDisplayingCompletedTasks = true;
            await LoadTasks();
        }

        public async Task LoadTasks()
        {
            ClearTasks();
            TaskState taskState = IsDisplayingCompletedTasks ? TaskState.COMPLETED : TaskState.IN_PROGRESS;
            try
            {
                await foreach (var record in _repository.GetTasks(taskState: taskState, limit: 20))
                {
                    Tasks.Add(new(
                        record.Id,
                        record.Name,
                        record.Description,
                        record.EstPomodoro,
                        record.ActPomodoro,
                        DateOnly.FromDateTime(record.CreatedAt).ToString("MMMM dd, yyyy"),
                        _helper.ConvertTaskStateToString(record.TaskState),
                        (int) record.TimeSpent.TotalMinutes
                        )) ;
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

        internal async Task OnTaskDeleted(TaskDomainViewModel deletedTask)
        {
            await LoadTasks();
        }

        public async void Receive(TaskDeletionInfoMessage taskDeletionInfo)
        {
            if (taskDeletionInfo.IsTaskDeletionSuccess) await LoadTasks();
        }

        public async void Receive(TaskCreationInfoMessage taskCreationInfo)
        {
            if (taskCreationInfo.IsTaskCreationSuccess) await LoadTasks();
        }

        public async void Receive(TaskUpdateStateInfoMessage taskUpdateInfo)
        {
            if (taskUpdateInfo.IsTaskUpdateSuccess) await LoadTasks();
        }
    }

    public partial class CreateTaskViewModel : ObservableValidator
    {
        private readonly ITasksRepository _repository;

        public CreateTaskViewModel(ITasksRepository repository)
        {
            _repository = repository;
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Task name is required")]
        private string _name;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        [CustomValidation(typeof(TasksHelper), nameof(TasksHelper.ValidateEstPomodoro))]
        private string? _estPomodoro;

        [RelayCommand]
        public async Task CreateTask()
        {
            ValidateAllProperties();

            if (HasErrors) return;

            int? estPomodoro = null;

            if(int.TryParse(EstPomodoro, out _))estPomodoro = int.Parse(EstPomodoro);

            try
            {
                await _repository.CreateTask(new(Name, Description, estPomodoro));
                StrongReferenceMessenger.Default.Send(new TaskCreationInfoMessage(true, "Task successfully added."));
            }
            catch (Exception ex)
            {
                StrongReferenceMessenger.Default.Send(new TaskCreationInfoMessage(false, ex.Message));
            }
        }


    }

    public partial class UpdateTaskViewModel : ObservableValidator
    {
        private readonly ITasksRepository _repository;
        private readonly TasksHelper _helper;

        [Required]
        [ObservableProperty]
        private int _id;

        [Required]
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        private int? _estPomodoro;

        [ObservableProperty]
        private TaskState _taskState;

        public UpdateTaskViewModel(ITasksRepository repository, TasksHelper helper) {
            _repository = repository;
            _helper = helper;
        }

        [RelayCommand]
        public async Task UpdateTask()
        {
            ValidateAllProperties();

            if (HasErrors) return;

            try
            {
                await _repository.UpdateTask(new(Id, Title, Description, EstPomodoro, TaskState));

                StrongReferenceMessenger.Default.Send(new TaskUpdateInfoMessage(true, "The task is successfully updated."));

            } catch(Exception ex)
            {
                StrongReferenceMessenger.Default.Send(new TaskUpdateInfoMessage(false, ex.Message));
            }
        }

        [RelayCommand]
        public async Task UpdateTaskState(UpdateTaskStateDomainViewModel args)
        {
            var confirmationRes = MessageBox.Show(string.Format("Are you sure you want to mark this task as {0}", 
                _helper.ConvertIntegerToTaskStateString(args.IntendedTaskState)),
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmationRes == MessageBoxResult.No) return;

            try
            {
                await _repository.UpdateTaskState(args.TaskId, _helper.ConvertIntegerToTaskState(args.IntendedTaskState));
                StrongReferenceMessenger.Default.Send(new TaskUpdateStateInfoMessage(true,
                    string.Format("Task succesfully marked as {0}", _helper.ConvertIntegerToTaskStateString(args.IntendedTaskState))));
            }

            catch(Exception ex)
            {
                StrongReferenceMessenger.Default.Send(new TaskUpdateStateInfoMessage(false, ex.Message));
            }
        }

    }

    public partial class DeleteTaskViewModel : ObservableObject
    {
        public ITasksRepository _repository;

        public DeleteTaskViewModel(ITasksRepository repository)
        {
            _repository = repository;
        }        

        [RelayCommand]
        public async void DeleteTask(int taskId)
        {
            try
            {
                var confirmationRes = 
                    MessageBox.Show("Are you sure want to delete the task?", 
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmationRes == MessageBoxResult.No) return;

                await _repository.DeleteTask(taskId);

                StrongReferenceMessenger.Default.Send(new TaskDeletionInfoMessage(true, "The task successfully deleted."));
            }

            catch (Exception ex)
            {
                StrongReferenceMessenger.Default.Send(new TaskDeletionInfoMessage(false, ex.Message));
            }
        }
    }

    public class UpdateTaskStateDomainViewModel : DependencyObject
    {
        public int TaskId
        {
            get { return (int)GetValue(TaskIdProperty); }
            set { SetValue(TaskIdProperty, value); }
        }

        public static readonly DependencyProperty TaskIdProperty =
            DependencyProperty.Register("TaskId", typeof(int), typeof(UpdateTaskStateDomainViewModel), new PropertyMetadata(0));

        public int IntendedTaskState
        {
            get { return (int)GetValue(IntendedTaskStateProperty); }
            set { SetValue(IntendedTaskStateProperty, value); }
        }
        public static readonly DependencyProperty IntendedTaskStateProperty =
            DependencyProperty.Register("IntendedTaskState", typeof(int), typeof(UpdateTaskStateDomainViewModel), new PropertyMetadata(0));

    }

}
