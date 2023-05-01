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

        private readonly ResourceDictionary _lightThemeResource;

        private readonly ResourceDictionary _darkThemeResource;

        public AppThemeService(ResourceDictionary lightTheme, ResourceDictionary darkTheme,
            IMessenger messenger) {

            _lightThemeResource = lightTheme;
            _darkThemeResource = darkTheme;
            _messenger = messenger;
            _messenger.RegisterAll(this);
        }

        public AppThemeService(IMessenger messenger)
        {
            _lightThemeResource = new() { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) };
            _darkThemeResource = new() { Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) };
            _messenger = messenger;
            _messenger.RegisterAll(this);
        }

        public void SwitchThemeTo(AppTheme theme)
        {
            App.Current.Resources.MergedDictionaries.RemoveAt(0);

            switch (theme)
            {
                case AppTheme.Light:
                    App.Current.Resources.MergedDictionaries.Insert(0, _lightThemeResource);
                    break;

                case AppTheme.Dark:
                    App.Current.Resources.MergedDictionaries.Insert(0, _darkThemeResource);
                    break;

                default:
                    App.Current.Resources.MergedDictionaries.Insert(0, _lightThemeResource);
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
