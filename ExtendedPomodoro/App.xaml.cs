﻿using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.Helpers;
using ExtendedPomodoro.Models.DbConfigs;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.DbSetup;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Repositories.Sqlite;
using ExtendedPomodoro.Models.Services;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.Proxies;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.ViewServices;
using Microsoft.Extensions.DependencyInjection;
using NHotkey.Wpf;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // order matters
            base.OnStartup(e);
            EnsureOnlyOneInstanceIsRunning();
            await InitializeDb();
            await InitializeAppSettings();
            RegisterHotkeys();
            SwitchThemeToCurrentSettings();
            await InitializeMainWindow();
        }

        private async Task InitializeAppSettings()
        {
            var settingsProvider = Services.GetRequiredService<IAppSettingsProvider>();
            await settingsProvider.Initialize();
        }

        private void SwitchThemeToCurrentSettings()
        {
            var settingsProvider = Services.GetRequiredService<IAppSettingsProvider>();

            var appThemeService = Services.GetRequiredService<AppThemeService>();
            appThemeService.SwitchThemeTo(
                settingsProvider.AppSettings.DarkModeEnabled ? AppTheme.Dark : AppTheme.Light);
        }

        private async Task InitializeMainWindow()
        {
            var mainWindow = MainWindowFactory.MainWindow;
            mainWindow.ShowInTaskbar = true;
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            await mainWindowViewModel.Initialize();
            mainWindow.DataContext = mainWindowViewModel;

            mainWindow.Show();
        }

        private void RegisterHotkeys()
        {
            var settingsProvider = Services.GetRequiredService<IAppSettingsProvider>();
            var hotkeyService = Services.GetRequiredService<HotkeyLoaderService>();
            hotkeyService.RegisterOrUpdateStartTimerHotkey(settingsProvider.AppSettings.StartHotkey);
            hotkeyService.RegisterOrUpdatePauseTimerHotkey(settingsProvider.AppSettings.PauseHotkey);
        }

        private async Task InitializeDb()
        {
            IDatabaseSetup dbSetup = Services.GetRequiredService<IDatabaseSetup>();
            await dbSetup.Setup();
        }

        private static void EnsureOnlyOneInstanceIsRunning()
        {
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p =>
                p.ProcessName == proc.ProcessName).Count();

            if (count > 1)
            {
                MessageBox.Show("Application is already running...");
                App.Current.Shutdown();
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<DbConfig>(
                (_) => new(ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString)
                );
            services.AddSingleton<TasksHelper>();
            services.AddTransient<ReadTasksViewModel>();
            services.AddTransient<CreateTaskViewModel>();
            services.AddTransient<UpdateTaskViewModel>();
            services.AddSingleton<DeleteTaskViewModel>();
            services.AddSingleton<ITasksService, TasksService>();
            services.AddSingleton<ITasksRepository, SqliteTasksRepository>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<ISettingsRepository, SqliteSettingsRepository>();
            services.AddSingleton<IDailySessionsService, DailySessionsService>();
            services.AddSingleton<IDailySessionsRepository, SqliteDailySessionsRepository>();
            services.AddSingleton<SqliteDbConnectionFactory>();
            services.AddSingleton<SqliteDbSetup>();
            services.AddSingleton<IDbConnectionFactory, SqliteDbConnectionFactory>();
            services.AddSingleton<IDatabaseSetup, SqliteDbSetup>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SoundService>();
            services.AddSingleton<TimerViewModel>();
            services.AddSingleton<TimerSessionState>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<StatsViewModel>();
            services.AddSingleton<TasksViewModel>();
            services.AddSingleton<NavigationViewModel>
                ((s) => new(s.GetRequiredService<TimerViewModel>(), s.GetRequiredService<IMessenger>()));
            services.AddSingleton<HotkeyManager>((_) => HotkeyManager.Current);
            services.AddSingleton<HotkeyLoaderService>();
            services.AddSingleton<IMessageBoxService, MessageBoxService>();
            services.AddSingleton<IMessenger>((_) => MessengerFactory.Messenger);
            services.AddSingleton<AppThemeService>();
            services.AddSingleton<DialogWindowService>();
            services.AddSingleton<StatsViewService>();
            services.AddSingleton<TimerViewService>();
            services.AddSingleton<SettingsViewService>();
            services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
            return services.BuildServiceProvider();
        }
    }
}
