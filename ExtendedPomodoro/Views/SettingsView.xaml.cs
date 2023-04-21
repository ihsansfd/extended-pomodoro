﻿using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Page
    {
        private readonly SoundService _soundService = new();
        public SettingsView()
        {
            InitializeComponent();

        }
        private void VolumeIconOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            _soundService.Volume = (int)VolumeSlider.Value;
            _soundService.RepeatFor = TimeSpan.FromSeconds(3);
            _soundService.Play((AlarmSound)AlarmSoundComboBox.SelectedValue);
        }
    }
}
