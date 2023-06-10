using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ExtendedPomodoro.ViewModels;
using MahApps.Metro.IconPacks;

namespace ExtendedPomodoro.Converters
{
    public class FlashMessageTypeToBootstrapIconKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FlashMessageType val)
            {
                if (val == FlashMessageType.SUCCESS)
                {
                    return PackIconBootstrapIconsKind.Check;
                }

                if (val == FlashMessageType.WARNING)
                {
                    return PackIconBootstrapIconsKind.Info;
                }

                if (val == FlashMessageType.ERROR)
                {
                    return PackIconBootstrapIconsKind.Exclamation;
                }
            }

            return PackIconBootstrapIconsKind.Check;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
