using ExtendedPomodoro.Converters;
using ExtendedPomodoro.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class TimerSessionStateToColorConverterFixture : IDisposable
    {
        public TimerSessionStateToColorConverterFixture()
        {
            if (System.Windows.Application.Current == null)
            { new System.Windows.Application { ShutdownMode = ShutdownMode.OnExplicitShutdown }; }

            Application.Current!.Resources.Add("Primary", new SolidColorBrush(Color.FromRgb(1, 1, 1)));
            Application.Current.Resources.Add("Info", new SolidColorBrush(Color.FromRgb(2, 2, 2)));
            Application.Current.Resources.Add("Success", new SolidColorBrush(Color.FromRgb(3, 3, 3)));
        }

        public void Dispose()
        {
        }
    }

    public class TimerSessionStateToColorConverterTests : IClassFixture<TimerSessionStateToColorConverterFixture>
    {
        private readonly TimerSessionStateToColorConverterFixture _fixture;
        private readonly TimerSessionStateToColorConverter _converter = new();

        public TimerSessionStateToColorConverterTests(TimerSessionStateToColorConverterFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [ClassData(typeof(TimerSessionStateToColorConverterTestData))]
        public void 
            Convert_ReturnCorrectColor(object value, Color expectedColor)
        {
            // Act
            var res = _converter.Convert(value, null!, null!, null!);

            // Assert
            Assert.Equal(expectedColor, res);
        }
    }

    public class TimerSessionStateToColorConverterTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            {new PomodoroSessionState(), Color.FromRgb(1, 1, 1) };
            yield return new object[] 
            { new ShortBreakSessionState(), Color.FromRgb(2, 2, 2) };
            yield return new object[] { new LongBreakSessionState(), Color.FromRgb(3, 3, 3) };
            yield return new object[] { typeof(object), Color.FromRgb(1, 1, 1) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
