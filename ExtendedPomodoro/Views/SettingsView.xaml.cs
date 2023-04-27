using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        private readonly SoundService _soundService = new();
        public SettingsView()
        {
            InitializeComponent();

        }
    }
}
