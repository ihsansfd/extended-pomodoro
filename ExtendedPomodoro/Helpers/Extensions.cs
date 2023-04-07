using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Helpers
{
    public static class Extensions
    {
        public static int? TryParseEmptiableStringToNullableInteger(this string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (int.TryParse(value, out _)) return int.Parse(value);

            return null;
        }
    }
}
