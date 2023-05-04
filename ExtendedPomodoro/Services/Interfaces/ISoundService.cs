using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface ISoundService
    {
        int Volume { get; set; }
        TimeSpan RepeatFor { get; set; }
        void Open(Uri source);
        void Play();
        void Stop();

    }
}
