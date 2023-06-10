using ExtendedPomodoro.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ExtendedPomodoro.ViewModels;

namespace ExtendedPomodoro.Converters
{
    public class FlashMessageTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FlashMessageType val)
            {
                if (val == FlashMessageType.SUCCESS)
                {
                    return ((SolidColorBrush)Application.Current.FindResource("SuccessDarker"));
                }

                if (val == FlashMessageType.WARNING)
                {
                    return ((SolidColorBrush)Application.Current.FindResource("WarningDarker"));
                }

                if (val == FlashMessageType.ERROR)
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
