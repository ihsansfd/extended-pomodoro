using ExtendedPomodoro.Helpers;
using ExtendedPomodoro.Models.DbConfigs;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.DbSetup;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Repositories.Sqlite;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views;
using Microsoft.Extensions.DependencyInjection;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
            base.OnStartup(e);

            EnsureOnlyOneInstanceIsRunning();

            await InitializeDb();

            await InitializeMainWindow();

            await RegisterHotkeys();
        }

        private async Task InitializeMainWindow()
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            await mainWindowViewModel.Initialize();
            mainWindow.DataContext = mainWindowViewModel;

            mainWindow.Show();
        }

        private async Task RegisterHotkeys()
        {
            var settingsRepo = Services.GetRequiredService<ISettingsRepository>();
            var settings = await settingsRepo.GetSettings();

            var hotkeyService = Services.GetRequiredService<HotkeyLoaderService>();
            hotkeyService.RegisterOrUpdateStartTimerHotkey(settings.StartHotkeyDomain.ConvertToHotkey());
            hotkeyService.RegisterOrUpdatePauseTimerHotkey(settings.PauseHotkeyDomain.ConvertToHotkey());
        }

        private void EnsureOnlyOneInstanceIsRunning()
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

        private async Task InitializeDb()
        {
            IDatabaseSetup dbSetup = Services.GetRequiredService<IDatabaseSetup>();
            await dbSetup.Setup();
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
            services.AddSingleton<SqliteTasksRepository>();
            services.AddSingleton<ITasksRepository, SqliteTasksRepository>();
            services.AddSingleton<ISettingsRepository, SqliteSettingsRepository>();
            services.AddSingleton<ISessionsRepository, SqliteSessionsRepository>();
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
            services.AddSingleton<NavigationViewModel>((service) => new(service.GetRequiredService<TimerViewModel>()));
            services.AddSingleton<HotkeyManager>((_s) => HotkeyManager.Current);
            services.AddSingleton<HotkeyLoaderService>();
            services.AddSingleton<MessageBoxService>();

            return services.BuildServiceProvider();
        }
    }
}
