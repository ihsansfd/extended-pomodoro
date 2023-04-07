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
    public class TasksListView : ListView
    {
        static TasksListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TasksListView), new FrameworkPropertyMetadata(typeof(TasksListView)));
        }

        public ICommand LoadMoreTasksCommand
        {
            get { return (ICommand)GetValue(LoadMoreTasksCommandProperty); }
            set { SetValue(LoadMoreTasksCommandProperty, value); }
        }

        public static readonly DependencyProperty LoadMoreTasksCommandProperty =
            DependencyProperty.Register("LoadMoreTasksCommand", typeof(ICommand), typeof(TasksListView));




        public Style LoadMoreButtonStyle
        {
            get { return (Style)GetValue(LoadMoreButtonStyleProperty); }
            set { SetValue(LoadMoreButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty LoadMoreButtonStyleProperty =
            DependencyProperty.Register("LoadMoreButtonStyle", typeof(Style), typeof(TasksListView));

        public bool IsLoadMoreButtonShown
        {
            get { return (bool)GetValue(IsLoadMoreButtonShownProperty); }
            set { SetValue(IsLoadMoreButtonShownProperty, value); }
        }

        public static readonly DependencyProperty IsLoadMoreButtonShownProperty =
            DependencyProperty.Register("IsLoadMoreButtonShown", typeof(bool), typeof(TasksListView), new PropertyMetadata(false));

    }
}
