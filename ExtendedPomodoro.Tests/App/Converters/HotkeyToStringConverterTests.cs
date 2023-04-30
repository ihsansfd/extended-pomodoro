using ExtendedPomodoro.Converters;
using ExtendedPomodoro.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class HotkeyToStringConverterTests
    {
        private readonly HotkeyToStringConverter _converter;

        public HotkeyToStringConverterTests()
        {
            _converter = new HotkeyToStringConverter();
        }

        [Fact]
        public void Convert_WhenInputValid_ReturnCorrectString()
        {
            var res = _converter.Convert(new Hotkey(Key.A, ModifierKeys.Shift), 
                null!, null!, null!);

            Assert.Equal("Shift + A", res);
        }

        [Fact]
        public void Convert_WhenInputInvalid_ReturnEmptyString()
        {
            var res = _converter.Convert(5,
                null!, null!, null!);

            Assert.Equal(string.Empty, res);
        }

        [Fact]
        public void ConvertBack_WhenInputValid_ReturnCorrectString()
        {
            var res = _converter.ConvertBack(new Hotkey(Key.A, ModifierKeys.Shift),
                null!, null!, null!);

            Assert.Equal("Shift + A", res);
        }

        [Fact]
        public void ConvertBack_WhenInputInvalid_ReturnEmptyString()
        {
            var res = _converter.ConvertBack(5,
                null!, null!, null!);

            Assert.Equal(string.Empty, res);
        }
    }
}
