using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.ViewModels.Interfaces;
using ExtendedPomodoro.ViewServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    public record StatAxesDomainViewModel(double[] XAxis, double[] YAxis); 
    public record StatAxesDisplayDomainViewModel(StatAxesDomainViewModel Axes, bool Display = true); 

    public partial class StatsViewModel : ObservableObject, INavigableViewModel
    {

        private readonly IDailySessionsService _repository;
        private readonly StatsViewService _statsViewService;

        private IEnumerable<DailySessionDomain> _dailySessions;

        public double[] XAxis { get; set; }
        public double[] YAxis { get; set; }

        [ObservableProperty]
        private DateTime _fromDate;

        [ObservableProperty]
        private DateTime _toDate;

        [ObservableProperty]
        private DateTime _minDate; // the minimum date allowed to be selected

        [ObservableProperty]
        private DateTime _maxDate; // the maximum date allowed to be selected

        [ObservableProperty]
        private bool _displayChart;

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
        private int _statsValueToDisplay = (int)StatsValue.POMODORO_COMPLETED;

        [RelayCommand]
        public void GenerateAxes()
        {
            LoadAxes();
        }

        [RelayCommand]
        public async Task GenerateStats()
        {
            await LoadStats();
        }

        [RelayCommand]
        public void ViewStatsInFullScreen()
        {
            _statsViewService.OpenScatterPlotStats(new(XAxis, YAxis));
        }

        public event EventHandler<StatAxesDisplayDomainViewModel> NewStatsAxes;

        public StatsViewModel(IDailySessionsService sessionsRepository, 
            StatsViewService statsService
            )
        {
            _repository = sessionsRepository;
            _statsViewService = statsService;
        }

        public async Task Load()
        {
            FromDate = ConvertToDateWithMinTime(DateTime.Today).AddDays(-7);
            ToDate = ConvertToDateWithMaxTime(DateTime.Today);

            await LoadDateRange();
        }

        private async Task LoadDateRange()
        {
            var dateRange = await _repository.GetDateRangeDailySessions();
            MinDate = dateRange.MinDate;
            MaxDate = dateRange.MaxDate;
        }

        private async Task LoadStats() {

            var sumsTask = _repository.GetSumDailySessions(
                ConvertToDateWithMinTime(FromDate), ConvertToDateWithMaxTime(ToDate));
            var dailySessionsTask = _repository.GetDailySessions(
                ConvertToDateWithMinTime(FromDate), ConvertToDateWithMaxTime(ToDate));
            _dailySessions = dailySessionsTask.ToBlockingEnumerable();

            LoadPropertiesFrom(await sumsTask);
            LoadAxes();
        }

        private void LoadAxes()
        {
             XAxis = _dailySessions.Select(prop =>
                 prop.SessionDate.ToDateTime(TimeOnly.MinValue).ToOADate()).ToArray();
             YAxis = Array.Empty<double>();

            switch((StatsValue)StatsValueToDisplay)
            {
                case StatsValue.POMODORO_COMPLETED:
                {
                    YAxis = _dailySessions.Select(prop => 
                        Convert.ToDouble(prop.TotalPomodoroCompleted)).ToArray();
                    break;
                }

                case StatsValue.SHORT_BREAKS_COMPLETED:
                {
                    YAxis = _dailySessions.Select(prop => 
                        Convert.ToDouble(prop.TotalShortBreaksCompleted)).ToArray();
                    break;
                }

                case StatsValue.LONG_BREAKS_COMPLETED:
                {
                    YAxis = _dailySessions.Select(prop => 
                        Convert.ToDouble(prop.TotalLongBreaksCompleted)).ToArray();
                    break;
                }

                case StatsValue.TASKS_COMPLETED:
                {
                    YAxis = _dailySessions.Select(prop => 
                        Convert.ToDouble(prop.TotalTasksCompleted)).ToArray();
                    break;
                }

                case StatsValue.TIME_SPENT:
                {
                    YAxis = _dailySessions.Select(prop => 
                        prop.TimeSpent.TotalMinutes).ToArray();
                    break;
                }
            }

            DisplayChart = XAxis.Length > 0 && YAxis.Length > 0;

            NewStatsAxes?.Invoke(this, new(new(XAxis, YAxis), DisplayChart));
        }

        public void LoadPropertiesFrom(SumDailySessionsDomain properties)
        {
            TotalPomodoroCompleted = properties.TotalPomodoroCompleted;
            TotalShortBreaksCompleted = properties.TotalShortBreaksCompleted;
            TotalLongBreaksCompleted = properties.TotalLongBreaksCompleted;
            TotalTimeSpentInMinutes = (int) properties.TotalTimeSpent.TotalMinutes;
            TotalTasksCompleted = properties.TotalTasksCompleted;
        }

        private static DateTime ConvertToDateWithMinTime(DateTime date)
        {
            return DateOnly.FromDateTime(date).ToDateTime(TimeOnly.MinValue);
        }

        private static DateTime ConvertToDateWithMaxTime(DateTime date)
        {
            return DateOnly.FromDateTime(date).ToDateTime(TimeOnly.MaxValue);
        }

    }
}
