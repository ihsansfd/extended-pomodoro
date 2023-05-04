using ExtendedPomodoro.Models.Domains;
using System;
using System.Threading;
using System.Windows.Media;
using ExtendedPomodoro.Services.Interfaces;
using static System.IO.Path;
using static System.AppDomain;

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

    public class SoundService : ISoundService
    {
        public int Volume { get => (int)_soundPlayer.Volume; set => _soundPlayer.Volume = value; }

        public TimeSpan RepeatFor { get; set; } = TimeSpan.Zero;
        private readonly IMediaPlayer _soundPlayer;
        private bool _stillRepeat = false;

        public SoundService(IMediaPlayer mediaPlayer)
        {
            _soundPlayer = mediaPlayer;
        }

        public void Open(Uri source)
        {
            _soundPlayer.Close();
            _soundPlayer.Open(source);
        }

        public void Play()
        {
            if (RepeatFor != Timeout.InfiniteTimeSpan)
            {
                ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false),
                    (_, _) => _stillRepeat = false, null, RepeatFor, true);
            }

            _stillRepeat = RepeatFor != TimeSpan.Zero;

            Replay();

            _soundPlayer.MediaEnded += OnSoundEnd;

            void OnSoundEnd(object? sender, EventArgs e)
            {
                if (_stillRepeat) Replay();

                else
                {
                    _soundPlayer.Pause();
                    _soundPlayer.MediaEnded -= OnSoundEnd;
                    _stillRepeat = false;
                }
            }
        }

        public void Stop()
        {
            _soundPlayer.Stop();
            _stillRepeat = false;
        }

        private void Replay()
        {
            _soundPlayer.Position = TimeSpan.Zero;
            _soundPlayer.Play();
        }
    }

    public class AlarmSoundService
    {
        private string _alarmSoundFilePath;

        private AlarmSound _alarmSound = AlarmSound.Chimes;

        public AlarmSound AlarmSound
        {
            get => _alarmSound;
            set
            {
                _alarmSound = value;
                _alarmSoundFilePath = GetFilePath(value);
                _soundService.Open(new Uri(_alarmSoundFilePath));
            }

        }

        private readonly ISoundService _soundService;

        public int Volume
        {
            get => _soundService.Volume;
            set => _soundService.Volume = value;
        }

        public TimeSpan RepeatFor
        {
            get => _soundService.RepeatFor;
            set => _soundService.RepeatFor = value;
        }

        public bool RepeatForeverOrDefault
        {
            set => _soundService.RepeatFor = value
                ? Timeout.InfiniteTimeSpan
                : TimeSpan.FromSeconds(15);
        }

        public AlarmSoundService(ISoundService soundService)
        {
            _soundService = soundService;
            _alarmSoundFilePath = GetFilePath(AlarmSound);
            _soundService.Open(new Uri(_alarmSoundFilePath));
        }

        public void Play()
        {

            _soundService.Play();
        }

        public void Stop() => _soundService.Stop();

        private string GetFilePath(AlarmSound alarmSound)
        {
            return string.Format("{0}" + DirectorySeparatorChar + "Assets" +
                          DirectorySeparatorChar + "AlarmSounds" + DirectorySeparatorChar +
                          "{1}.mp3", CurrentDomain.BaseDirectory, alarmSound.ToString());
        }
    }

    public class MouseClickSoundService
    {
        private static readonly string MouseClickSoundFilePath =
            string.Format("{0}" + DirectorySeparatorChar + "Assets" + DirectorySeparatorChar +
                          "SoundEffects" + DirectorySeparatorChar + "{1}.mp3",
                CurrentDomain.BaseDirectory, "MouseClick");

        private readonly ISoundService _soundService;

        public int Volume
        {
            get => _soundService.Volume;
            set => _soundService.Volume = value;
        }

        public MouseClickSoundService(ISoundService soundService)
        {
            _soundService = soundService;
            _soundService.Open(new Uri(MouseClickSoundFilePath));
            _soundService.RepeatFor = TimeSpan.Zero;
        }

        public void Play()
        {
            _soundService.Play();
        }

    }
}
