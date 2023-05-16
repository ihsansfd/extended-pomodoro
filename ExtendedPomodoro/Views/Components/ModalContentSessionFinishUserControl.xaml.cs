using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Factories;
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
using ExtendedPomodoro.Services;

namespace ExtendedPomodoro.Views.Components
{
    /// <summary>
    /// Interaction logic for ModalContentSessionFinishUserControl.xaml
    /// </summary>
    public partial class ModalContentSessionFinishUserControl : UserControl
    {
        public ModalContentSessionFinishUserControl()
        {
            DataContext = this;
            InitializeComponent();
        }

        public TimerSessionState FinishedSession
        {
            get { return (TimerSessionState)GetValue(FinishedSessionProperty); }
            set { SetValue(FinishedSessionProperty, value); }
        }

        public static readonly DependencyProperty FinishedSessionProperty =
            DependencyProperty.Register("FinishedSession", typeof(TimerSessionState), typeof(ModalContentSessionFinishUserControl));

        public TimerSessionState NextSession
        {
            get { return (TimerSessionState)GetValue(NextSessionProperty); }
            set { SetValue(NextSessionProperty, value); }
        }

        public static readonly DependencyProperty NextSessionProperty =
            DependencyProperty.Register("NextSession", typeof(TimerSessionState), typeof(ModalContentSessionFinishUserControl));
    }
}
