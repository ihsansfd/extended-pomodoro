using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ExtendedPomodoro.Services;

namespace ExtendedPomodoro.Converters
{
    public class AssessmentResultToStringMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AssessmentResult val)
            {
                if (val == AssessmentResult.SUCCESS) return "Passed";
                if (val == AssessmentResult.WARNING) return "OK";
                if (val == AssessmentResult.FAILURE) return "Improvement needed";
            }

            return "Passed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
