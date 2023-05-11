using ExtendedPomodoro.Services;
using ExtendedPomodoro.ViewModels;
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

namespace ExtendedPomodoro.Views.Components
{
    /// <summary>
    /// Interaction logic for ScatterPlotStatsUserControl.xaml
    /// </summary>
    public partial class ScatterPlotStatsUserControl : UserControl
    {
        public ChartDataDomainViewModel Axes { get; set; }

        public ScatterPlotStatsUserControl()
        {
            InitializeComponent();
        }

        public void Load()
        {
            GenerateTheChart();
        }

        private void GenerateTheChart()
        {
            ScatterPlotService.FormatChart(StatsPlotView);
            ScatterPlotService.GenerateChartFrom(StatsPlotView, Axes);
        }
    }

   
}
