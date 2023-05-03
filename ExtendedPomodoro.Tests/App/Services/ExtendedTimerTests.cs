using ExtendedPomodoro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Extensions;

namespace ExtendedPomodoro.Tests.App.Services
{
    public class ExtendedTimerTests
    {
        private readonly ExtendedTimer _sut = new();

        [Theory]
        [InlineData(10, 100, 10)]
        [InlineData(5, 50, 5)]
        [InlineData(20, 30, 1)]
        public void StartPause_GiveCorrectRemainingTimeEveryIntervalPassed(
            int interval, int timerSetFor, int expectedExecutedFor)
        {
            _sut.Interval = TimeSpan.FromMilliseconds(interval);

            var executedFor = 0;

            void OnSutOnRemainingTimeChanged(object? sender, RemainingTimeChangedEventArgs args)
            {
                executedFor++;
            }

            _sut.RemainingTimeChanged += OnSutOnRemainingTimeChanged;

            _sut.Initialize(TimeSpan.FromMilliseconds(timerSetFor));
             _sut.Start();

             Task.Factory.StartNew(() =>
             {
                 Task.Delay(TimeSpan.FromMilliseconds(timerSetFor));
                 _sut.Pause();
                 Assert.Equal(expectedExecutedFor, executedFor);
             });

        }

    }
}
