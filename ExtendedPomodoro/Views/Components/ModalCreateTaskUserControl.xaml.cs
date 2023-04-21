using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
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

namespace ExtendedPomodoro.Views.Components
{
    /// <summary>
    /// Interaction logic for ModalCreateTaskUserControl.xaml
    /// </summary>
    public partial class ModalCreateTaskUserControl : UserControl, IRecipient<TaskCreationInfoMessage>
    {
        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register("IsShown", typeof(bool), typeof(ModalCreateTaskUserControl), new PropertyMetadata(false));

        public string TaskName
        {
            get { return (string)GetValue(TaskNameProperty); }
            set { SetValue(TaskNameProperty, value); }
        }
        public static readonly DependencyProperty TaskNameProperty =
            DependencyProperty.Register("TaskName", typeof(string), typeof(ModalCreateTaskUserControl));

        public CreateTaskViewModel CreateTaskViewModel
        {
            get { return (CreateTaskViewModel)GetValue(CreateTaskViewModelProperty); }
            set { SetValue(CreateTaskViewModelProperty, value); }
        }

        public static readonly DependencyProperty CreateTaskViewModelProperty =
            DependencyProperty.Register("CreateTaskViewModel", typeof(CreateTaskViewModel), typeof(ModalCreateTaskUserControl));

        public ModalCreateTaskUserControl()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.RegisterAll(this);
        }

        public void Receive(TaskCreationInfoMessage taskCreationInfo)
        {
            if (!taskCreationInfo.IsTaskCreationSuccess) { 
                MessageBox.Show(taskCreationInfo.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        ~ModalCreateTaskUserControl()
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }
    }

}