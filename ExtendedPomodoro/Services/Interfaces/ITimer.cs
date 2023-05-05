using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface ITimer
    {
        event EventHandler Tick;
        TimeSpan Interval { get; set; }
        void Start();
        void Stop();
    }
}
