using CommunityToolkit.Mvvm.Messaging;
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
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ExtendedPomodoro.Core.Timeout;
using ExtendedPomodoro.ViewServices.Interfaces;

namespace ExtendedPomodoro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider Services { get; }

        static App()
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

        // TODO: place this into the Core.
        private static void EnsureOnlyOneInstanceIsRunning()
        {
            var proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Count(p => p.ProcessName == proc.ProcessName);

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
            services.AddSingleton<MainWindowViewModel>(
                (s) => 
                    new MainWindowViewModel(
                        s.GetRequiredService<TimerViewModel>(),
                        s.GetRequiredService<TasksViewModel>(),
                        s.GetRequiredService<StatsViewModel>(),
                        s.GetRequiredService<SettingsViewModel>(),
                        s.GetRequiredService<IMessenger>()
                        ));
            services.AddSingleton<SoundService>();
            services.AddSingleton<TimerViewModel>();
            services.AddSingleton<TimerSessionState>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<StatsViewModel>();
            services.AddSingleton<TasksViewModel>();
            services.AddSingleton<IHotkeyManager, HotkeyManagerAdapter>();
            services.AddSingleton<HotkeyLoaderService>();
            services.AddSingleton<IMessageBoxService, MessageBoxService>();
            services.AddSingleton<IMessenger>((_) => StrongReferenceMessenger.Default);
            services.AddSingleton<AppThemeService>();
            services.AddSingleton<DialogWindowService>();
            services.AddSingleton<IStatsViewService, StatsViewService>();
            services.AddSingleton<TimerViewService>();
            services.AddSingleton<ISettingsViewService, SettingsViewService>();
            services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();
            services.AddTransient<ISoundService, SoundService>();
            services.AddTransient<RegisterWaitTimeoutCallback>((_) => WaitTimeoutProvider.RegisterWaitTimeout);
            services.AddTransient<IMediaPlayer, MediaPlayerAdapter>();
            services.AddTransient<AlarmSoundService>();
            services.AddTransient<MouseClickSoundService>();
            services.AddTransient<ITimer, DispatcherTimerAdapter>();
            services.AddTransient<ExtendedTimer>();
            return services.BuildServiceProvider();
        }
    }
}
