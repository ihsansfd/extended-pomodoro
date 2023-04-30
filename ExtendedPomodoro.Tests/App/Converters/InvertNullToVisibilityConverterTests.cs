using ExtendedPomodoro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class InvertNullToVisibilityConverterTests
    {
        private readonly InvertNullToVisibilityConverter _converter = new();

        [Fact]
        public void Convert_WhenNull_ReturnVisible()
        {
            var res = _converter.Convert(null!, null!, null!, null!);
            Assert.Equal(Visibility.Visible, res);
        }

        [Fact]
        public void Convert_WhenNotNull_ReturnCollapsed()
        {
            var res = _converter.Convert(5, null!, null!, null!);
            Assert.Equal(Visibility.Collapsed, res);
        }
    }
}
