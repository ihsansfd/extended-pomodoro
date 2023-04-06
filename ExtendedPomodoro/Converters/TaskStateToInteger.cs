using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ExtendedPomodoro.Converters
{
    public class TaskStateToInteger : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is TaskState val)
            {
                return val == TaskState.IN_PROGRESS ? 0 : 1;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int val)
            {
                return val == 1 ? TaskState.COMPLETED : 0;
            }

            return TaskState.IN_PROGRESS;
        }
    }
}
