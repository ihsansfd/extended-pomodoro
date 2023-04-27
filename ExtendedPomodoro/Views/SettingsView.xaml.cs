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
        private void VolumeIconOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            _soundService.Volume = (int)VolumeSlider.Value;
            _soundService.RepeatFor = TimeSpan.FromSeconds(3);
            _soundService.Play((AlarmSound)AlarmSoundComboBox.SelectedValue);
        }
    }
}
