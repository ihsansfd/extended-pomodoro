using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Models.Services.Interfaces;
using ExtendedPomodoro.ViewServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ExtendedPomodoro.Core.Extensions;
using ExtendedPomodoro.Models.Services;
using ExtendedPomodoro.ViewServices.Interfaces;

namespace ExtendedPomodoro.ViewModels
{
    public enum StatsValue
    {
        POMODORO_COMPLETED = 0,
        SHORT_BREAKS_COMPLETED = 1,
        LONG_BREAKS_COMPLETED = 2,
        TASKS_COMPLETED = 3,
        TIME_SPENT = 4,
        DAILY_POMODORO_TARGET = 5
    }

    public record ChartDataDomainViewModel(double[] XAxis, double[] YAxis, bool DisplayAsPercentage = false);

    public partial class StatsViewModel : ObservableObject
    {
        private readonly IDailySessionsService _sessionsService;
        private readonly IStatsViewService _statsViewService;

        private IEnumerable<DailySessionDomain> _dailySessions = Array.Empty<DailySessionDomain>();

        public ChartDataDomainViewModel ChartData { get; set; }

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
        private SumDailyPomodoroTargetDomain _sumDailyPomodoroTarget;

        [ObservableProperty]
        private int _statsValueToDisplay = (int)StatsValue.POMODORO_COMPLETED;

        [RelayCommand]
        private async Task Load()
        {
            FromDate = DateTime.Today.ToMinTime().AddDays(-7);
            ToDate = DateTime.Today.ToMaxTime();

            await LoadDateRange();
            LoadStats();
        }

        [RelayCommand]
        private void GenerateStats() => LoadStats();

        [RelayCommand]
        private void ViewStatsInFullScreen()
        {
            _statsViewService.OpenScatterPlotStats(ChartData);
        }

        public event EventHandler<ChartDataDomainViewModel> NewChartData;

        public StatsViewModel(IDailySessionsService sessionsService, 
            IStatsViewService statsViewService
            )
        {
            _sessionsService = sessionsService;
            _statsViewService = statsViewService;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(StatsValueToDisplay))
            {
                LoadChartData();
            }
        }

        private async Task LoadDateRange()
        {
            var dateRange = await _sessionsService.GetDateRangeDailySessions();
            MinDate = dateRange.MinDate;
            MaxDate = dateRange.MaxDate;
        }

        private void LoadStats() {

            _dailySessions = _sessionsService.GetDailySessions(
                FromDate.ToMinTime(), ToDate.ToMaxTime()).ToBlockingEnumerable();

            LoadSumProperties();
            LoadChartData();
        }

        private void LoadChartData()
        {
            var xAxis = _dailySessions.Select(prop =>
                 prop.SessionDate.ToDateTime(TimeOnly.MinValue).ToOADate()).ToArray();

            var yAxis = (StatsValue)StatsValueToDisplay switch
            {
                StatsValue.POMODORO_COMPLETED => _dailySessions.Select(prop =>
                        Convert.ToDouble(prop.TotalPomodoroCompleted))
                    .ToArray(),
                StatsValue.SHORT_BREAKS_COMPLETED => _dailySessions.Select(prop =>
                        Convert.ToDouble(prop.TotalShortBreaksCompleted))
                    .ToArray(),
                StatsValue.LONG_BREAKS_COMPLETED => _dailySessions.Select(prop =>
                        Convert.ToDouble(prop.TotalLongBreaksCompleted))
                    .ToArray(),
                StatsValue.TASKS_COMPLETED => _dailySessions.Select(prop => Convert.ToDouble(prop.TotalTasksCompleted))
                    .ToArray(),
                StatsValue.DAILY_POMODORO_TARGET => _dailySessions.Select(prop => 
                    Convert.ToDouble(prop.DailyPomodoroTarget == 0 ? 1 : 
                        Math.Min((double)prop.TotalPomodoroCompleted / (double)prop.DailyPomodoroTarget, 1)))
                    .ToArray(),
                StatsValue.TIME_SPENT => _dailySessions.Select(prop => prop.TimeSpent.TotalMinutes).ToArray(),
                _ => Array.Empty<double>()
            };

            ChartData = new ChartDataDomainViewModel(xAxis, yAxis, DisplayAsPercentage: 
                (StatsValue)StatsValueToDisplay == StatsValue.DAILY_POMODORO_TARGET);

            var newChartData = DisplayChart = ChartData.XAxis.Length > 0 
                                              && ChartData.YAxis.Length > 0;

            if(newChartData) 
                NewChartData?.Invoke(this, ChartData);
        }

        private void LoadSumProperties()
        {
            var properties = _dailySessions.SumEach();

            TotalPomodoroCompleted = properties.TotalPomodoroCompleted;
            TotalShortBreaksCompleted = properties.TotalShortBreaksCompleted;
            TotalLongBreaksCompleted = properties.TotalLongBreaksCompleted;
            TotalTimeSpentInMinutes = (int) properties.TotalTimeSpent.TotalMinutes;
            TotalTasksCompleted = properties.TotalTasksCompleted;
            SumDailyPomodoroTarget = properties.SumDailyPomodoroTarget;
        }

    }
}
