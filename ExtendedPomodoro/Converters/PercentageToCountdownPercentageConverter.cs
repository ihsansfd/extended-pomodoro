using System;
using System.Globalization;
using System.Windows.Data;

namespace ExtendedPomodoro.Converters
{
    public class PercentageToCountdownPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double val) return 100 - val;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
