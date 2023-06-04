using ExtendedPomodoro.Models.Domains;
using System;
using System.IO;
using System.Threading;
using System.Windows.Media;
using ExtendedPomodoro.Core.Timeout;
using ExtendedPomodoro.Services.Interfaces;
using static System.AppDomain;

namespace ExtendedPomodoro.Services
{
    public class SoundService : ISoundService
    {
        public int Volume { get => (int)_soundPlayer.Volume; set => _soundPlayer.Volume = value; }

        public TimeSpan RepeatFor { get; set; } = TimeSpan.Zero;
        private readonly IMediaPlayer _soundPlayer;
        private readonly RegisterWaitTimeoutCallback _registerWaitTimeoutCallback;
        private bool _stillRepeat = false;

        public SoundService(IMediaPlayer mediaPlayer, 
            RegisterWaitTimeoutCallback registerWaitTimeoutCallback)
        {
            _soundPlayer = mediaPlayer;
            _registerWaitTimeoutCallback = registerWaitTimeoutCallback;
        }

        public void Open(Uri source)
        {
            _soundPlayer.Close();
            _soundPlayer.Open(source);
        }

        public void Play()
        {
            if (RepeatFor > TimeSpan.Zero && RepeatFor != Timeout.InfiniteTimeSpan)
            {
                _registerWaitTimeoutCallback.Invoke(() =>_stillRepeat = false, RepeatFor);
            }

            _stillRepeat = RepeatFor > TimeSpan.Zero || RepeatFor == Timeout.InfiniteTimeSpan;

            _soundPlayer.MediaEnded += OnSoundEnd;

            Replay();

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

        public void Stop()  {
            _stillRepeat = false;
            _soundPlayer.Stop();
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

        public void Play() => _soundService.Play();

        public void Stop() => _soundService.Stop();

        private string GetFilePath(AlarmSound alarmSound)
        {
            return Path.Combine(CurrentDomain.BaseDirectory, "Assets", "AlarmSounds",
                alarmSound.ToString() + ".mp3");
        }
    }

    public class MouseClickSoundService
    {
        private static readonly string MouseClickSoundFilePath =
            Path.Combine(CurrentDomain.BaseDirectory, "Assets", "SoundEffects", "MouseClick.mp3");

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

        public void Play() => _soundService.Play();
    }
}
