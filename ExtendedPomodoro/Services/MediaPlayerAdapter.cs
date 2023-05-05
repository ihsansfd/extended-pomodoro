using ExtendedPomodoro.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ExtendedPomodoro.Services
{
    public class MediaPlayerAdapter : IMediaPlayer
    {
        private readonly MediaPlayer _mediaPlayer = new();

        public double Volume
        {
            get => _mediaPlayer.Volume;
            set => _mediaPlayer.Volume = value;
        }

        public TimeSpan Position
        {
            get => _mediaPlayer.Position;
            set => _mediaPlayer.Position = value;
        }

        public event EventHandler MediaEnded
        {
            add => _mediaPlayer.MediaEnded += value;
            remove => _mediaPlayer.MediaEnded -= value;
        }

        public void Open(Uri source) => _mediaPlayer.Open(source);

        public void Play() => _mediaPlayer.Play();

        public void Pause() => _mediaPlayer.Pause();

        public void Stop() => _mediaPlayer.Stop();

        public void Close() => _mediaPlayer.Close();
    }
}
