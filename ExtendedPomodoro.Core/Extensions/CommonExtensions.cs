using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.FrameworkExtensions.Extensions
{
    public static class CommonExtensions
    {
        public static int? TryParseEmptiableStringToNullableInteger(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (int.TryParse(value, out _)) return int.Parse(value);

            return null;
        }

        public static double SecondsToMinutes(this double value)
        {
            return value / 60;
        }
    }
}
