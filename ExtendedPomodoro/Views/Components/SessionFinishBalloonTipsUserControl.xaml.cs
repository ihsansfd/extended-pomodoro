using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Messages;
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
using System.Windows.Threading;
using ExtendedPomodoro.Views.Interfaces;

namespace ExtendedPomodoro.Views.Components
{
    /// <summary>
    /// Interaction logic for SessionFinishBalloonTipsUserControl.xaml
    /// </summary>
    public partial class SessionFinishBalloonTipsUserControl : UserControl
    {
        public SessionFinishBalloonTipsUserControl()
        {
            InitializeComponent();
        }
    }
}