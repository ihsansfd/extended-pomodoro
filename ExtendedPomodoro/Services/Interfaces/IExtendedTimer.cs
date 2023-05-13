using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface IExtendedTimer
    {
        event EventHandler<RemainingTimeChangedEventArgs>? RemainingTimeChanged;
        TimeSpan Interval { get; set; }
        void Initialize(TimeSpan timerSetFor);
        void Start();
        void Resume();
        void Pause();
        void Stop();
    }
}
