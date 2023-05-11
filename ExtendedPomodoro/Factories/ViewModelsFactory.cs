using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedPomodoro.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ExtendedPomodoro.Factories
{
    public class StatsViewModelFactory
    {
        public static StatsViewModel GetSingleton() => App.Services.GetRequiredService<StatsViewModel>();
    }

    //public class TasksViewModelFactory
    //{
    //    public static TasksViewModel GetSingleton() => App.Services.GetRequiredService<TasksViewModel>();
    //}

    //public class SettingsViewModelFactory
    //{
    //    public static SettingsViewModel GetSingleton() => App.Services.GetRequiredService<SettingsViewModel>();
    //}

    //public class TimerViewModelFactory
    //{
    //    public static TimerViewModel GetSingleton() => App.Services.GetRequiredService<TimerViewModel>();
    //}
}
