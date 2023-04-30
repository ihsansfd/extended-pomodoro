using ExtendedPomodoro.Models.Domains;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ExtendedPomodoro.Converters
{
    public class TaskStateConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoTheConversion(value);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DoTheConversion(value);
        }

        private object? DoTheConversion(object value)
        {
            if (value is TaskState valTaskState)
            {
                return valTaskState == TaskState.IN_PROGRESS ? 0 : 1;
            }

            else if (value is int) return value;

            return null;
        }
    }
}
