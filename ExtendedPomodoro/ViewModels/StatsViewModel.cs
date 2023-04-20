using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewModels
{
    public enum StatsValue
    {
        POMODORO_COMPLETED = 0,
        SHORT_BREAKS_COMPLETED = 1,
        LONG_BREAKS_COMPLETED = 2,
        TASKS_COMPLETED = 3,
        TIME_SPENT = 4
    }

    public record class StatAxesDomainViewModel(double[] XAxis, double[] YAxis); 

    public partial class StatsViewModel : ObservableObject
    {
        private ISessionsRepository _repository;
        private IEnumerable<DailySessionDomain> _dailySessions;

        public double[] XAxis { get; set; }
        public double[] YAxis { get; set; }

        [ObservableProperty]
        private DateTime _fromDate;

        [ObservableProperty]
        private DateTime _toDate;

        [ObservableProperty]
        private int _totalPomodoroCompleted;

        [ObservableProperty]
        private int _totalShortBreaksCompleted;

        [ObservableProperty]
        private int _totalLongBreaksCompleted;

        [ObservableProperty]
        private int _totalTimeSpentInMinutes;

        [ObservableProperty]
        private int _totalTasksCompleted;

        [ObservableProperty]
        private int _statValueToDisplay = (int)StatsValue.POMODORO_COMPLETED;

        public event EventHandler<StatAxesDomainViewModel> NewStatsAxes;

        public StatsViewModel(ISessionsRepository sessionsRepository)
        {
            _repository = sessionsRepository;
        }

        public void Initialize()
        {
            FromDate = DateTime.Now.AddDays(-7);
            ToDate = DateTime.Now;
        }

        [RelayCommand]
        public async Task LoadStats() {

            var sumsTask = _repository.GetSumDailySessions(FromDate, ToDate);
            var dailySessionsTask = _repository.GetDailySessions(FromDate, ToDate);
            _dailySessions = dailySessionsTask.ToBlockingEnumerable();

            LoadPropertiesFrom(await sumsTask);
            LoadAxesFrom(_dailySessions, (StatsValue)StatValueToDisplay);
        }

        private void LoadAxesFrom(IEnumerable<DailySessionDomain> sessions, 
            StatsValue statValueToDisplay)
        {

            double[] XAxis = sessions.Select(prop => prop.SessionDate.ToDateTime(TimeOnly.MinValue).ToOADate()).ToArray();
            double[] YAxis = Array.Empty<double>();

            switch(statValueToDisplay)
            {
                case StatsValue.POMODORO_COMPLETED:
                {
                    YAxis = sessions.Select(prop => 
                    Convert.ToDouble(prop.TotalPomodoroCompleted)).ToArray();
                    break;
                }

                case StatsValue.SHORT_BREAKS_COMPLETED:
                    {
                        YAxis = sessions.Select(prop => 
                        Convert.ToDouble(prop.TotalShortBreaksCompleted)).ToArray();
                        break;
                    }

                case StatsValue.LONG_BREAKS_COMPLETED:
                    {
                        YAxis = sessions.Select(prop => 
                        Convert.ToDouble(prop.TotalLongBreaksCompleted)).ToArray();
                        break;
                    }

                case StatsValue.TASKS_COMPLETED:
                    {
                        YAxis = sessions.Select(prop => Convert.ToDouble(prop.TotalTasksCompleted)).ToArray();
                        break;
                    }

                case StatsValue.TIME_SPENT:
                    {
                        YAxis = sessions.Select(prop => Convert.ToDouble(prop.TimeSpent)).ToArray();
                        break;
                    }
            }

            NewStatsAxes?.Invoke(this, new(XAxis, YAxis));
        }

        public void LoadPropertiesFrom(SumDailySessionsDomain properties)
        {
            TotalPomodoroCompleted = properties.TotalPomodoroCompleted;
            TotalShortBreaksCompleted = properties.TotalShortBreaksCompleted;
            TotalLongBreaksCompleted = properties.TotalLongBreaksCompleted;
            TotalTimeSpentInMinutes = (int) properties.TotalTimeSpent.TotalMinutes;
            TotalTasksCompleted = properties.TotalTasksCompleted;
        }
    }
}
