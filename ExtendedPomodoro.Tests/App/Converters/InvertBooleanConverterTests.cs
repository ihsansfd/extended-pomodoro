using ExtendedPomodoro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class InvertBooleanConverterTests
    {

        private readonly InvertBooleanConverter _converter = new();

        [Fact]
        public void Convert_WhenTrue_ReturnFalse()
        {
            var res = (bool)_converter.Convert(true, null!, null!, null!);

            Assert.False(res);
        }

        [Fact]
        public void Convert_WhenFalse_ReturnTrue()
        {
            var res = (bool)_converter.Convert(false, null!, null!, null!);

            Assert.True(res);
        }

    }
}
