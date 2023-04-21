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
            if(!taskUpdateInfo.IsTaskUpdateSuccess)
            {
                MessageBox.Show(taskUpdateInfo.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        ~TasksView() {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }

    }
}
