using ExtendedPomodoro.Converters;
using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Tests.App.Converters
{
    public class TaskStateConverterTests
    {
        private readonly TaskStateConverter _converter = new();

        [Fact]
        public void Convert_WhenValueIsInProgress_Return0()
        {
            var res = _converter.Convert(TaskState.IN_PROGRESS, null!, null!, null!);
            Assert.Equal(0, res);
        }

        [Fact]
        public void Convert_WhenValueIsCompleted_Return1()
        {
            var res = _converter.Convert(TaskState.COMPLETED, null!, null!, null!);
            Assert.Equal(1, res);
        }

        [Fact]
        public void Convert_WhenValueIsInt_ReturnInt()
        {
            var res = _converter.Convert(1, null!, null!, null!);
            Assert.Equal(1, res);
        }

        [Fact]
        public void Convert_WhenValueIsInvalid_ReturnNull()
        {
            var res = _converter.Convert("hello", null!, null!, null!);
            Assert.Null(res);
        }
    }
}
