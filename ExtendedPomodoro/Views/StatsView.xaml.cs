using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for StatsView.xaml
    /// </summary>
    public partial class StatsView : Page
    {
        private readonly ScatterPlotService _scatterPlotService = new();
        private StatsViewModel _viewModel = null!;

        public StatsView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            FormatChart();
        }

        private void FormatChart()
        {
            _scatterPlotService.FormatChart(StatsPlotView);
        }

        private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is not StatsViewModel vm) return;

            _viewModel = vm;
            _viewModel.NewStatsAxes += OnNewStatsAxes;
            await _viewModel.GenerateStats();
        }

        private void OnNewStatsAxes(object? sender, StatAxesDisplayDomainViewModel domain)
        {
            if (!domain.Display) return;
            GenerateChartFrom(domain.Axes);
        }

        private void GenerateChartFrom(StatAxesDomainViewModel axes)
        {
            _scatterPlotService.GenerateChartFrom(StatsPlotView, axes);
        }

        ~StatsView()
        {
            Unsubscribe();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            DataContextChanged -= OnDataContextChanged;
            _viewModel.NewStatsAxes -= OnNewStatsAxes;
        }

    }
}
