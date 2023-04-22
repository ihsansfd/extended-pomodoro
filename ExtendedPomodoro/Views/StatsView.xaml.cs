using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.ViewModels;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtendedPomodoro.Views
{
    /// <summary>
    /// Interaction logic for StatsView.xaml
    /// </summary>
    public partial class StatsView : Page
    {
        private StatsViewModel? _viewModel;

        public StatsView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;

            FormatChart();
        }

        private void FormatChart()
        {
            StatsPlotView.Configuration.ScrollWheelZoom = false;
            var plotColorResource = ((SolidColorBrush)FindResource("Black")).Color;
            StatsPlotView.Plot.XAxis.TickLabelFormat("M/dd/yyyy", true);
            StatsPlotView.Plot.YAxis.TickLabelFormat((val) => ((int)val).ToString());
            StatsPlotView.Plot.YAxis2.SetSizeLimit(min: 40);
            StatsPlotView.Plot.Style(figureBackground: System.Drawing.Color.Transparent,
                dataBackground: System.Drawing.Color.Transparent,
                tick: System.Drawing.Color.FromArgb(plotColorResource.R,
                plotColorResource.G, plotColorResource.B),
                grid: System.Drawing.Color.Transparent);
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

        private void OnNewStatsAxes(object? sender, StatAxesDomainViewModel e)
        {
            if (!e.Display) return;
            GenerateChartFrom(e);
        }

        private void GenerateChartFrom(StatAxesDomainViewModel axes)
        {
            StatsPlotView.Plot.Clear();
            var scatter = StatsPlotView.Plot.AddScatter(axes.XAxis, axes.YAxis);
            var primaryColorResource = ((SolidColorBrush)FindResource("Primary")).Color;
            var primaryColor = System.Drawing.Color.FromArgb(primaryColorResource.R, 
                primaryColorResource.G, primaryColorResource.B);
            scatter.LineColor = primaryColor;
            scatter.MarkerColor = primaryColor;
            StatsPlotView.Refresh();
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
