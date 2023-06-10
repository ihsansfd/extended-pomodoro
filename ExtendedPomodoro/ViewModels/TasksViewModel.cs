using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Helpers;
using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using ExtendedPomodoro.FrameworkExtensions.Extensions;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Services.Interfaces;

namespace ExtendedPomodoro.ViewModels
{
    public partial class TasksViewModel : 
        ObservableObject, 
        IRecipient<TaskCreationInfoMessage>, 
        IRecipient<TaskUpdateInfoMessage>,
        IRecipient<TaskUpdateStateInfoMessage>,
        IRecipient<TaskDeletionInfoMessage>
    {
        public IFlashMessageServiceViewModel FlashMessageServiceViewModel { get; init; }

        public ReadTasksViewModel ReadTasksViewModel { get;}

        public CreateTaskViewModel CreateTaskViewModel { get; }

        public UpdateTaskViewModel UpdateTaskViewModel { get; }

        public DeleteTaskViewModel DeleteTaskViewModel { get; }

        private readonly IMessenger _messenger;

        public TasksViewModel(ReadTasksViewModel readTaskViewModel, 
            UpdateTaskViewModel updateTaskViewModel,
            CreateTaskViewModel createTaskViewModel, 
            DeleteTaskViewModel deleteTaskViewModel,
            IMessenger messenger,
            IFlashMessageServiceViewModel flashMessageServiceViewModel
            )
        {
            ReadTasksViewModel = readTaskViewModel;
            CreateTaskViewModel = createTaskViewModel;
            UpdateTaskViewModel = updateTaskViewModel;
            DeleteTaskViewModel = deleteTaskViewModel;
            FlashMessageServiceViewModel = flashMessageServiceViewModel;
            _messenger = messenger;
            _messenger.RegisterAll(this);
        }

        [RelayCommand]
        private async Task Load()
        {
            await ReadTasksViewModel.LoadTasks();
        }

        public void Receive(TaskUpdateInfoMessage message)
        {
            FlashMessageServiceViewModel.NewFlashMessage(
                message.IsSuccess ? FlashMessageType.SUCCESS : FlashMessageType.ERROR, message.Message);
        }

        public void Receive(TaskCreationInfoMessage message)
        {
            FlashMessageServiceViewModel.NewFlashMessage(
                message.IsSuccess ? FlashMessageType.SUCCESS : FlashMessageType.ERROR, message.Message);
        }

        public void Receive(TaskDeletionInfoMessage message)
        {
            FlashMessageServiceViewModel.NewFlashMessage(
                message.IsSuccess ? FlashMessageType.SUCCESS : FlashMessageType.ERROR, message.Message);
        }

        public void Receive(TaskUpdateStateInfoMessage message)
        {
            FlashMessageServiceViewModel.NewFlashMessage(
                message.IsSuccess ? FlashMessageType.SUCCESS : FlashMessageType.ERROR, message.Message);
        }
    }

    public partial class ReadTasksViewModel : ObservableObject, 
        IRecipient<TaskDeletionInfoMessage>, 
        IRecipient<TaskCreationInfoMessage>, 
        IRecipient<TaskUpdateStateInfoMessage>,
        IRecipient<TaskUpdateInfoMessage>
    {
       private readonly ITasksService _service;
       private readonly IMessenger _messenger;

       private int _currentPage = 1;
       private int _totalPages = 1;

       public ObservableCollection<TaskDomainViewModel> Tasks { get; } = new();

       [ObservableProperty]
       private bool _isDisplayingCompletedTasks = false; // true = completed tasks, false = in progress tasks

        [ObservableProperty]
        private bool _areThereMoreTasks;

        public ReadTasksViewModel(
            ITasksService service,
            IMessenger messenger)
        {
            _service = service;
            _messenger = messenger;

            messenger.RegisterAll(this);
        }

        [RelayCommand]
        private async Task DisplayInProgressTasks()
        {
            IsDisplayingCompletedTasks = false;
            await LoadTasks();
        }

        [RelayCommand]
        private async Task DisplayCompletedTasks()
        {
            IsDisplayingCompletedTasks = true;
            await LoadTasks();
        }

        [RelayCommand]
        private async Task DisplayMoreTasks()
        {
            _currentPage++;
            OnAreThereMoreTasksChanged();

            await LoadTasks(instantiateNew: false, page: _currentPage);
        }

        public async Task LoadTasks(bool instantiateNew = true, int page = 1)
        {
            if(instantiateNew) ClearTasks();

            TaskState taskState = IsDisplayingCompletedTasks ? TaskState.COMPLETED : TaskState.IN_PROGRESS;

            if (instantiateNew)
            {
                _totalPages = await _service.GetTotalPages(taskState);
                OnAreThereMoreTasksChanged();
            }
            await foreach (var record in _service.GetTasks(taskState: taskState, page: page, limit: 20))
            {
                Tasks.Add(new TaskDomainViewModel()
                    {
                        Id = record.Id,
                        Name = record.Name,
                        Description = record.Description,
                        EstPomodoro = record.EstPomodoro,
                        ActPomodoro = record.ActPomodoro,
                        CreatedAt = DateOnly.FromDateTime(record.CreatedAt).ToString("MMMM dd, yyyy"),
                        CompletedAt = record.CompletedAt != null ? 
                            DateOnly.FromDateTime((DateTime)record.CompletedAt).ToString("MMMM dd, yyyy") : null,
                        TaskStatus = TasksHelper.ConvertTaskStateToInteger(record.TaskState),
                        TimeSpentInMinutes = record.TimeSpent.TotalMinutes
                    }
                ) ;
            }
        }

        public async void Receive(TaskDeletionInfoMessage taskDeletionInfo)
        {
            if (taskDeletionInfo.IsSuccess) await LoadTasks();
        }

        public async void Receive(TaskCreationInfoMessage taskCreationInfo)
        {
            if (taskCreationInfo.IsSuccess) await LoadTasks();
        }

        public async void Receive(TaskUpdateStateInfoMessage taskUpdateStateInfo)
        {
            if (taskUpdateStateInfo.IsSuccess) await LoadTasks();
        }

        public async void Receive(TaskUpdateInfoMessage taskUpdateInfo)
        {
            if (taskUpdateInfo.IsSuccess) await LoadTasks();
        }

        private void ClearTasks()
        {
            Tasks.Clear();
            _currentPage = 1;
        }

        private void OnAreThereMoreTasksChanged()
        {
            AreThereMoreTasks = _currentPage < _totalPages;
        }

        ~ReadTasksViewModel()
        {
            _messenger.UnregisterAll(this);
        }
    }

    public partial class CreateTaskViewModel : ObservableValidator
    {
        private readonly ITasksService _repository;
        private readonly IMessenger _messenger;

        public CreateTaskViewModel(
            ITasksService repository, 
            IMessenger messenger)
        {
            _repository = repository;
            _messenger = messenger;
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Task name is required", AllowEmptyStrings = false)]
        private string? _name;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        [CustomValidation(typeof(TasksHelper), nameof(TasksHelper.ValidateEstPomodoro))]
        [Range(1, int.MaxValue, ErrorMessage = "Please specify value >= 1")]
        private string? _estPomodoro;

        [ObservableProperty]
        private bool _isModalShown = false;

        [RelayCommand]
        private void OpenModal() => IsModalShown = true;

        [RelayCommand]
        private void CloseModal() => IsModalShown = false;

        [RelayCommand]
        private async Task CreateTask()
        {
            ValidateAllProperties();

            if (HasErrors) return;

            int? estPomodoro = EstPomodoro.TryParseEmptiableStringToNullableInteger();

            try
            {
                await _repository.CreateTask(new CreateTaskDomain(Name, Description, estPomodoro));
                ClearAll();
                CloseModal();

                _messenger.Send(new TaskCreationInfoMessage(true, "Task added successfully.", this));
            }
            catch (Exception ex)
            {
                _messenger.Send(new TaskCreationInfoMessage(false, $"Task failed to be added: {ex.Message}", this));
            }
        }

        private void ClearAll()
        {
            Name = null;
            Description = null;
            EstPomodoro = null;
        }
    }

    public partial class UpdateTaskViewModel : ObservableValidator
    {
        private readonly ITasksService _service;
        private readonly IMessageBoxService _messageBox;
        private readonly IMessenger _messenger;

        [Required]
        [ObservableProperty]
        private int _id;

        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Task name is required", AllowEmptyStrings = false)]
        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        [CustomValidation(typeof(TasksHelper), nameof(TasksHelper.ValidateEstPomodoro))]
        private string? _estPomodoro;

        [ObservableProperty]
        private int _actPomodoro;

        [ObservableProperty]
        private int _taskStatus;

        [ObservableProperty]
        private int _timeSpentInMinutes;

        [ObservableProperty] 
        private string _createdAt;

        [ObservableProperty]
        private string? _completedAt;

        [ObservableProperty]
        private bool _isModalShown = false;

        public UpdateTaskViewModel(
            ITasksService repository, 
            IMessageBoxService messageBoxService,
            IMessenger messenger)
        {
            _service = repository;
            _messageBox = messageBoxService;
            _messenger = messenger;
        }

        [RelayCommand]
        private async Task UpdateTask()
        {
            ValidateAllProperties();

            if (HasErrors) return;

            int? estPomodoro = EstPomodoro.TryParseEmptiableStringToNullableInteger();

            try
            {
                await _service.UpdateTask(
                    new() {
                        Id = Id,
                        Name = Name,
                        Description = Description,
                        EstPomodoro = estPomodoro,
                        Taskstate = TasksHelper.ConvertIntegerToTaskState(TaskStatus)
                    }
                    );
                CloseModal();
                _messenger.Send(new TaskUpdateInfoMessage(true, "Task updated successfully."));

            } catch(Exception ex)
            {
                _messenger.Send(new TaskUpdateInfoMessage(false, $"Task failed to be updated {ex.Message}"));
            }

        }

        [RelayCommand]
        private async Task UpdateTaskState(IUpdateTaskStateDomainViewModel args)
        {
            var confirmationRes = _messageBox.Show(string.Format("Are you sure you want to mark this task as {0}",
                TasksHelper.ConvertIntegerToTaskStateString(args.IntendedTaskState)),
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmationRes != MessageBoxResult.Yes) return;

            try
            {
                await _service.UpdateTaskState(args.TaskId, TasksHelper.ConvertIntegerToTaskState(args.IntendedTaskState));
                _messenger.Send(new TaskUpdateStateInfoMessage(true, "Task status updated successfully."));
            }

            catch(Exception ex)
            {
                _messenger.Send(new TaskUpdateStateInfoMessage(false, $"Task status failed to be updated: {ex.Message}"));
            }
        }

        [RelayCommand]
        private void LoadTaskDetail(TaskDomainViewModel args)
        {
            Id = args.Id;
            Name = args.Name;
            Description = args.Description;
            EstPomodoro = args.EstPomodoro.ToString();
            ActPomodoro = args.ActPomodoro;
            TaskStatus = args.TaskStatus;
            TimeSpentInMinutes = (int)args.TimeSpentInMinutes;
            CompletedAt = args.CompletedAt;
            CreatedAt = args.CreatedAt;

            IsModalShown = true;
        }

        [RelayCommand]
        private void CloseModal() => IsModalShown = false;

    }

    public partial class DeleteTaskViewModel : ObservableObject
    {
        private readonly ITasksService _repository;
        private readonly IMessageBoxService _messageBox;
        private readonly IMessenger _messenger;

        public DeleteTaskViewModel(
            ITasksService repository,
            IMessageBoxService messageBoxService,
            IMessenger messenger)
        {
            _repository = repository;
            _messageBox = messageBoxService;
            _messenger = messenger;
        }        

        [RelayCommand]
        private async Task DeleteTask(int taskId)
        {
            try
            {
                var confirmationRes =
                    _messageBox.Show("Are you sure want to delete the task?", 
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirmationRes != MessageBoxResult.Yes) return;

                await _repository.DeleteTask(taskId);

                _messenger.Send(new TaskDeletionInfoMessage(true, "Task deleted successfully."));
            }

            catch (Exception ex)
            {
                _messenger.Send(new TaskDeletionInfoMessage(false, $"Task failed to be deleted: {ex.Message}"));
            }
        }
    }

    public interface IUpdateTaskStateDomainViewModel
    {
        int TaskId { get; set; }
        int IntendedTaskState { get; set; }
    }

    public class UpdateTaskStateDomainViewModel : DependencyObject, IUpdateTaskStateDomainViewModel
    {
        public int TaskId
        {
            get => (int)GetValue(TaskIdProperty);
            set => SetValue(TaskIdProperty, value);
        }

        public static readonly DependencyProperty TaskIdProperty =
            DependencyProperty.Register(nameof(TaskId), typeof(int), typeof(UpdateTaskStateDomainViewModel), new PropertyMetadata(0));

        public int IntendedTaskState
        {
            get => (int)GetValue(IntendedTaskStateProperty);
            set => SetValue(IntendedTaskStateProperty, value);
        }
        public static readonly DependencyProperty IntendedTaskStateProperty =
            DependencyProperty.Register(nameof(IntendedTaskState), typeof(int), typeof(UpdateTaskStateDomainViewModel), new PropertyMetadata(0));
    }

}
