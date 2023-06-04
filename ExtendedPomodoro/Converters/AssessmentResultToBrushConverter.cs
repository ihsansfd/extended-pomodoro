using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ExtendedPomodoro.Services;

namespace ExtendedPomodoro.Converters
{
    public class AssessmentResultToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AssessmentResult val)
            {
                if (val == AssessmentResult.SUCCESS)
                {
                    return ((SolidColorBrush)Application.Current.FindResource("SuccessDarker"));
                }

                if (val == AssessmentResult.WARNING)
                {
                    return ((SolidColorBrush)Application.Current.FindResource("WarningDarker"));
                }

                if (val == AssessmentResult.FAILURE)
                {
                    return ((SolidColorBrush)Application.Current.FindResource("DangerDarker"));
                }
            }

            return ((SolidColorBrush)Application.Current.FindResource("Success"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
