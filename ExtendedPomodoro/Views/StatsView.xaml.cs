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
        private StatsViewModel _viewModel;

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
            if (this.DataContext is StatsViewModel)
            {
                _viewModel = (StatsViewModel)this.DataContext;
                _viewModel.NewStatsAxes += OnNewStatsAxes;
                await _viewModel.GenerateStats();
            }
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
            if (_viewModel != null) _viewModel.NewStatsAxes -= OnNewStatsAxes;
        }

    }
}
