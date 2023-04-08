using ExtendedPomodoro.Helpers;
using ExtendedPomodoro.Models.DbConfigs;
using ExtendedPomodoro.Models.DbConnections;
using ExtendedPomodoro.Models.DbSetup;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Repositories;
using ExtendedPomodoro.Models.Repositories.Sqlite;
using ExtendedPomodoro.ViewModels;
using ExtendedPomodoro.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

            InitializeDb();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var mainWindow = Services.GetRequiredService<MainWindow>();
            //mainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            //mainWindow.Show();

            new windowtest().Show();
        }

        private void InitializeDb()
        {
            IDatabaseSetup dbSetup = Services.GetRequiredService<IDatabaseSetup>();
            dbSetup.Setup();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<DbConfig>(
                (s) => new(ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString)
                );
            services.AddSingleton<TasksHelper>();
            services.AddTransient<ReadTasksViewModel>();
            services.AddTransient<CreateTaskViewModel>();
            services.AddTransient<UpdateTaskViewModel>();
            services.AddSingleton<DeleteTaskViewModel>();
            services.AddSingleton<SqliteTasksRepository>();
            services.AddSingleton<ITasksRepository, SqliteTasksRepository>();
            services.AddSingleton<SqliteDbConnectionFactory>();
            services.AddSingleton<SqliteDbSetup>();
            services.AddSingleton<IDbConnectionFactory, SqliteDbConnectionFactory>();
            services.AddSingleton<IDatabaseSetup, SqliteDbSetup>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<TimerViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<StatsViewModel>();
            services.AddSingleton<TasksViewModel>();
            services.AddSingleton<NavigationViewModel>((service) => new(service.GetRequiredService<TimerViewModel>()));

            return services.BuildServiceProvider();
        }
    }
}
