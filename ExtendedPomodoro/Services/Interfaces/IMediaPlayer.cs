using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface IMediaPlayer
    {
        TimeSpan Position { get; set; }
        double Volume { get; set; }
        event EventHandler MediaEnded;
        void Play();
        void Pause();
        void Stop();
        void Close();
        void Open(Uri source); 
       
    }
}
