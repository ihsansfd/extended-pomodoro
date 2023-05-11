using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Core.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ToMinTime(this DateTime date)
        {
            return DateOnly.FromDateTime(date).ToDateTime(TimeOnly.MinValue);
        }

        public static DateTime ToMaxTime(this DateTime date)
        {
            return DateOnly.FromDateTime(date).ToDateTime(TimeOnly.MaxValue);
        }

    }
}
