using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
using System.Windows;
using System.Windows.Controls;
using ExtendedPomodoro.Factories;

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for StatsView.xaml
    /// </summary>
    public partial class StatsView : Page
    {
        private readonly StatsViewModel _viewModel;

        public StatsView()
        {
            InitializeComponent(); 
            DataContext = _viewModel = StatsViewModelFactory.GetSingleton();
            _viewModel.NewChartData += OnNewChartData;
            ScatterPlotService.GenerateChartFrom(StatsPlotView, _viewModel.ChartData);
            ScatterPlotService.FormatChart(StatsPlotView);
        }

        private void OnNewChartData(object? sender, ChartDataDomainViewModel data)
        {
            ScatterPlotService.GenerateChartFrom(StatsPlotView, data);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.NewChartData -= OnNewChartData;
        }
    }
}
