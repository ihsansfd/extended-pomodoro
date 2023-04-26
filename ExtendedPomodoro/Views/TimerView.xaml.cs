using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Controls;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views.Components;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for TimerView.xaml
    /// </summary>
    public partial class TimerView : Page
    {
        public TimerView()
        {
            InitializeComponent();
        }
    }
}
