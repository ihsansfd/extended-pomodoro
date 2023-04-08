using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
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
    /// Interaction logic for TimerView.xaml
    /// </summary>
    public partial class TimerView : Page, IRecipient<TasksComboBoxAddNewButtonPressedMessage>
    {
        public TimerView()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.RegisterAll(this);
        }

        void IRecipient<TasksComboBoxAddNewButtonPressedMessage>.Receive(TasksComboBoxAddNewButtonPressedMessage message)
        {
            ModalCreateTask.IsShown = true;
            TasksComboBox.IsDropDownOpen = false;
        }

        ~TimerView()
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
