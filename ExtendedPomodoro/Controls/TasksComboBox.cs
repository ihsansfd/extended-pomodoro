using ExtendedPomodoro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(TasksComboBox));


        public ICommand LoadMoreTasksCommand
        {
            get { return (ICommand)GetValue(LoadMoreTasksCommandProperty); }
            set { SetValue(LoadMoreTasksCommandProperty, value); }
        }

        public static readonly DependencyProperty LoadMoreTasksCommandProperty =
            DependencyProperty.Register("LoadMoreTasksCommand", typeof(ICommand), typeof(TasksComboBox));

        public bool IsLoadMoreButtonShown
        {
            get { return (bool)GetValue(IsLoadMoreButtonShownProperty); }
            set { SetValue(IsLoadMoreButtonShownProperty, value); }
        }

        public static readonly DependencyProperty IsLoadMoreButtonShownProperty =
            DependencyProperty.Register("IsLoadMoreButtonShown", typeof(bool), typeof(TasksComboBox));

        public Brush PlaceholderForeground
        {
            get { return (Brush)GetValue(PlaceholderForegroundProperty); }
            set { SetValue(PlaceholderForegroundProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register("PlaceholderForeground", typeof(Brush), typeof(TasksComboBox), new PropertyMetadata(Brushes.Gray));

        public Brush CompleteTaskButtonForeground
        {
            get { return (Brush)GetValue(CompleteTaskButtonForegroundProperty); }
            set { SetValue(CompleteTaskButtonForegroundProperty, value); }
        }

        public static readonly DependencyProperty CompleteTaskButtonForegroundProperty =
            DependencyProperty.Register("CompleteTaskButtonForeground", typeof(Brush), typeof(TasksComboBox), new PropertyMetadata(Brushes.Black));

        public Brush CompleteTaskButtonHoverColor
        {
            get { return (Brush)GetValue(CompleteTaskButtonHoverColorProperty); }
            set { SetValue(CompleteTaskButtonHoverColorProperty, value); }
        }

        public static readonly DependencyProperty CompleteTaskButtonHoverColorProperty =
            DependencyProperty.Register("CompleteTaskButtonHoverColor", typeof(Brush), typeof(TasksComboBox), new PropertyMetadata(Brushes.Black));

    }
}
