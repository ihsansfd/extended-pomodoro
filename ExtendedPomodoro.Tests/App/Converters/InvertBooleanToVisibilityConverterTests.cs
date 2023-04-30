using ExtendedPomodoro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class InvertBooleanToVisibilityConverterTests
    {
        private readonly InvertBooleanToVisibilityConverter _converter = new();

        [Fact]
        public void Convert_WhenFalse_ReturnVisible()
        {
            var res = (Visibility)_converter.Convert(false, null!, null!, null!);
            Assert.Equal(Visibility.Visible, res);
        }

        [Fact]
        public void Convert_WhenTrue_ReturnCollapsed()
        {
            var res = (Visibility)_converter.Convert(true, null!, null!, null!);
            Assert.Equal(Visibility.Collapsed, res);
        }
    }
}
