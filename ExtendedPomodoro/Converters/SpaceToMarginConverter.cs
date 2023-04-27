using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ExtendedPomodoro.Converters
{
    public class SpaceToMarginConverter : IValueConverter
    {
        public object Convert(object val, Type targetType, object parameter, CultureInfo culture)
        {
            if (val is double value)
            {
                return new Thickness(value, 0, 0, 0);
            }

            return new Thickness(0, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
