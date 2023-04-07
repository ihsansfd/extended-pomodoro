using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Controls;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for TasksView.xaml
    /// </summary>
    public partial class TasksView : Page, 
        IRecipient<TaskCreationInfoMessage>, 
        IRecipient<TaskDeletionInfoMessage>, 
        IRecipient<TaskUpdateStateInfoMessage>,
        IRecipient<TaskUpdateInfoMessage>
    {
        public TasksView()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.RegisterAll(this);

        }

        private void ButtonCreateTask_Click(object sender, RoutedEventArgs e)
        {
            ModalCreateTask.IsShown = true;
        }

        private void ButtonCancelCreateTaskModal_Click(object sender, RoutedEventArgs e)
        {
            ModalCreateTask.IsShown = false;
        }

        public void Receive(TaskCreationInfoMessage taskCreationInfo)
        {
            if(taskCreationInfo.IsTaskCreationSuccess)
            {
                ModalCreateTask.IsShown = false;
            }

            else
            {
                MessageBox.Show(taskCreationInfo.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Receive(TaskDeletionInfoMessage taskDeletionInfo)
        {
            if (!taskDeletionInfo.IsTaskDeletionSuccess)
            {
                MessageBox.Show(taskDeletionInfo.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Receive(TaskUpdateStateInfoMessage taskUpdateStateInfo)
        {
            if (!taskUpdateStateInfo.IsTaskUpdateSuccess)
            {
                MessageBox.Show(taskUpdateStateInfo.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Receive(TaskUpdateInfoMessage taskUpdateInfo)
        {
            if(taskUpdateInfo.IsTaskUpdateSuccess)
            {
                ModalViewTaskDetail.IsShown = false;
            }

            else
            {
                MessageBox.Show(taskUpdateInfo.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        ~TasksView() {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }

        private void ButtonTaskDetail_Click(object _sender, RoutedEventArgs e)
        {
            var sender = (IconOnlyButton) _sender;
            var taskData = (TaskDomainViewModel) ((BindingProxy) sender.FindResource("CurrentDataContext")).Data;

            var modal = (Modal) FindName("ModalViewTaskDetail");

            var idTextBlock = (TextBlock) modal.FindName("IdTaskDetail");
            idTextBlock.Text = taskData.Id.ToString();
            BindingOperations.GetBindingExpression(idTextBlock, TextBlock.TextProperty).UpdateSource();

            var titleTextBox = (TextBox) modal.FindName("TitleTaskDetail");
            titleTextBox.Text = taskData.Name;
            BindingOperations.GetBindingExpression(titleTextBox, TextBox.TextProperty).UpdateSource();

            var descriptionTextBox = (TextBox)modal.FindName("DescriptionTaskDetail");
            descriptionTextBox.Text = taskData.Description;
            BindingOperations.GetBindingExpression(descriptionTextBox, TextBox.TextProperty).UpdateSource();

            var estPomodoroTaskDetail = (TextBox)modal.FindName("EstPomodoroTaskDetail");
            estPomodoroTaskDetail.Text = taskData.EstPomodoro.ToString();
            BindingOperations.GetBindingExpression(estPomodoroTaskDetail, TextBox.TextProperty).UpdateSource();

            var actPomodoroTextBlock = (TextBlock)modal.FindName("ActPomodoroTaskDetail");
            actPomodoroTextBlock.Text = taskData.ActPomodoro.ToString();

            var statusTextBlock = (ComboBox)modal.FindName("StatusTaskDetail");
            statusTextBlock.SelectedValue = taskData.TaskStatus.ToString();
            statusTextBlock.SelectedItem = taskData.TaskStatus.ToString();
            BindingOperations.GetBindingExpression(statusTextBlock, ComboBox.SelectedValueProperty).UpdateSource();

            var timeSpentTextBlock = (TextBlock)modal.FindName("TimeSpentTaskDetail");
            timeSpentTextBlock.Text = taskData.TimeSpentInMinutes.ToString();

            modal.IsShown = true;
        }

        private void ButtonCloseTaskDetail_Click(object sender, RoutedEventArgs e)
        {
            ModalViewTaskDetail.IsShown = false;
        }

    }
}
