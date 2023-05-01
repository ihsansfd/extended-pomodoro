using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using System;
using System.Windows;

namespace ExtendedPomodoro.Services
{
    public enum AppTheme
    {
        Light = 0,
        Dark = 1,
    }

    public class AppThemeService : IRecipient<SettingsUpdateInfoMessage>
    {
        private readonly IMessenger _messenger;

        private static readonly ResourceDictionary LIGHT_THEME_RESOURCE = 
            new() { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative)};

        private static readonly ResourceDictionary DARK_THEME_RESOURCE =
            new() { Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) };

        public AppThemeService(IMessenger messenger) {
            _messenger = messenger;

            _messenger.RegisterAll(this);
        }

        public void SwitchThemeTo(AppTheme theme)
        {
            App.Current.Resources.MergedDictionaries.RemoveAt(0);

            switch (theme)
            {
                case AppTheme.Light:
                    App.Current.Resources.MergedDictionaries.Insert(0, LIGHT_THEME_RESOURCE);
                    break;

                case AppTheme.Dark:
                    App.Current.Resources.MergedDictionaries.Insert(0, DARK_THEME_RESOURCE);
                    break;

                default:
                    App.Current.Resources.MergedDictionaries.Insert(0, LIGHT_THEME_RESOURCE);
                    break;
            }
        }

        public void Receive(SettingsUpdateInfoMessage message)
        {
            bool isDarkMode = message.AppSettings.DarkModeEnabled;

            if(isDarkMode) SwitchThemeTo(AppTheme.Dark);
            else SwitchThemeTo(AppTheme.Light);        
        }

        ~AppThemeService() => _messenger.UnregisterAll(this);
    }
}
