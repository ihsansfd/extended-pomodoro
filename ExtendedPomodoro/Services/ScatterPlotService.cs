using ExtendedPomodoro.ViewModels;
using ScottPlot;
using System.Windows;
using System.Windows.Media;

namespace ExtendedPomodoro.Services
{
    public class ScatterPlotService
    {
        public static void FormatChart(WpfPlot wpfPlot)
        {
            wpfPlot.Configuration.ScrollWheelZoom = false;
            var mainBrushResource = ((SolidColorBrush)Application.Current.FindResource("MainBrush")).Color;
            wpfPlot.Plot.XAxis.TickLabelFormat("M/dd/yyyy", true);
            wpfPlot.Plot.YAxis2.SetSizeLimit(min: 40);
            wpfPlot.Plot.Style(figureBackground: System.Drawing.Color.Transparent,
                dataBackground: System.Drawing.Color.Transparent,
                tick: System.Drawing.Color.FromArgb(mainBrushResource.R,
                mainBrushResource.G, mainBrushResource.B),
                grid: System.Drawing.Color.Transparent);
        }

        public static void GenerateChartFrom(WpfPlot wpfPlot, ChartDataDomainViewModel chartData)
        {
            wpfPlot.Plot.Clear();

            if (chartData.DisplayAsPercentage)
            {
                wpfPlot.Plot.YAxis.TickLabelFormat((val) => val.ToString("P1"));
            }
            else
            {
                wpfPlot.Plot.YAxis.TickLabelFormat((val) => ((int)val).ToString());
            }

            var scatter = wpfPlot.Plot.AddScatter(chartData.XAxis, chartData.YAxis);
            var primaryColorResource = ((SolidColorBrush)Application.Current.FindResource("Primary")).Color;
            var primaryColor = System.Drawing.Color.FromArgb(primaryColorResource.R,
                primaryColorResource.G, primaryColorResource.B);
            scatter.LineColor = primaryColor;
            scatter.MarkerColor = primaryColor;
            wpfPlot.Refresh();
        }
    }
}
