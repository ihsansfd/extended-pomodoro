﻿using ExtendedPomodoro.ViewModels;
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
    public partial class TasksView : Page
    {
        public TasksView()
        {
            InitializeComponent();

        }

        private void ButtonCreateTask_Click(object sender, RoutedEventArgs e)
        {
            ModalCreateTask.IsShown = true;
        }

        private void ButtonCancelCreateTaskModal_Click(object sender, RoutedEventArgs e)
        {
            ModalCreateTask.IsShown = false;
        }
    }
}
