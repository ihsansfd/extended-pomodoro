using ExtendedPomodoro.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class NullToVisibilityConverterTests
    {
        private readonly NullToVisibilityConverter _converter = new();

        [Fact]
        public void Convert_WhenNull_ReturnCollapsed()
        {
            var res = _converter.Convert(null!, null!, null!, null!);
            Assert.Equal(Visibility.Collapsed, res);
        }

        [Fact]
        public void Convert_WhenNotNull_ReturnVisible()
        {
            var res = _converter.Convert(5, null!, null!, null!);
            Assert.Equal(Visibility.Visible, res);
        }
    }
}
