using ExtendedPomodoro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class SpaceToMarginConverterTests
    {
        private readonly SpaceToMarginConverter _converter = new();

        [Fact]
        public void Convert_WhenValueIsDouble_ReturnLeftMargin()
        {
            var res = _converter.Convert(10.0, null!, null!, null!);
            Assert.Equal(new Thickness(10, 0, 0, 0), res);
        }

        [Fact]
        public void Convert_WhenValueIsntDoble_Return0LeftMargin()
        {
            var res = _converter.Convert("hello", null!, null!, null!);
            Assert.Equal(new Thickness(0, 0, 0, 0), res);
        }
    }
}
