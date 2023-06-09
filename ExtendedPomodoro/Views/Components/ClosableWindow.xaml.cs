﻿using System;
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
using System.Windows.Shapes;

namespace ExtendedPomodoro.Views.Components
{
    /// <summary>
    /// Interaction logic for ClosableWindow.xaml
    /// </summary>
    public partial class ClosableWindow : Window
    {
        public ClosableWindow()
        {
            InitializeComponent();
        }

        public bool IsOKButtonShown
        {
            get { return (bool)GetValue(IsOKButtonShownProperty); }
            set { SetValue(IsOKButtonShownProperty, value); }
        }

        public static readonly DependencyProperty IsOKButtonShownProperty =
            DependencyProperty.Register("IsOKButtonShown", typeof(bool), typeof(ClosableWindow), new PropertyMetadata(true));

        public bool IsLogoShown
        {
            get { return (bool)GetValue(IsLogoShownProperty); }
            set { SetValue(IsLogoShownProperty, value); }
        }
        public static readonly DependencyProperty IsLogoShownProperty =
            DependencyProperty.Register("IsLogoShown", typeof(bool), typeof(ClosableWindow), new PropertyMetadata(false));
    }
}
