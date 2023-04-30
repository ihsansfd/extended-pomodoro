using ExtendedPomodoro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class PercentageToCountdownPercentageConverterTests
    {
        private readonly PercentageToCountdownPercentageConverter _converter = new();

        [Fact]
        public void Convert_WhenValueIsDouble_ReturnPercentage()
        {
            var res = _converter.Convert(30.0, null!, null!, null!);

            Assert.Equal(70.0, res);
        }

        [Fact]
        public void Convert_WhenValueIsNotDouble_Return0()
        {
            var res = _converter.Convert("hello", null!, null!, null!);

            Assert.Equal(0, res);
        }
    }
}
