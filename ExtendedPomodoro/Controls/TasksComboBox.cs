using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExtendedPomodoro.Controls
{
    public class TasksComboBox : ComboBox
    {
        static TasksComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TasksComboBox), new FrameworkPropertyMetadata(typeof(TasksComboBox)));
        }

        public ICommand CompleteTaskCommand
        {
            get { return (ICommand)GetValue(CompleteTaskCommandProperty); }
            set { SetValue(CompleteTaskCommandProperty, value); }
        }

        public static readonly DependencyProperty CompleteTaskCommandProperty =
            DependencyProperty.Register("CompleteTaskCommand", typeof(ICommand), typeof(TasksComboBox));

        public ICommand CancelTaskCommand
        {
            get { return (ICommand)GetValue(CancelTaskCommandProperty); }
            set { SetValue(CancelTaskCommandProperty, value); }
        }

        public static readonly DependencyProperty CancelTaskCommandProperty =
            DependencyProperty.Register("CancelTaskCommand", typeof(ICommand), typeof(TasksComboBox));



        public ICommand AddNewTaskCommand
        {
            get { return (ICommand)GetValue(AddNewTaskCommandProperty); }
            set { SetValue(AddNewTaskCommandProperty, value); }
        }

        public static readonly DependencyProperty AddNewTaskCommandProperty =
            DependencyProperty.Register("AddNewTaskCommand", typeof(ICommand), typeof(TasksComboBox));

    }
}
