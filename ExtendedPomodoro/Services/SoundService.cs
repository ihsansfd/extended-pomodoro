using ExtendedPomodoro.Models.Domains;
using System;
using System.Threading;
using System.Windows.Media;

namespace ExtendedPomodoro.Services
{
    // This is to be used in view layer, look at System.Windows.Media namespace
    public class SoundService
    {
        private static readonly char SEPERATOR = System.IO.Path.DirectorySeparatorChar;
        private static readonly string PROJECT_PATH = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(SEPERATOR);

        private MediaPlayer _soundPlayer = new();

        private MediaPlayer _soundPlayerMouseClick = new();

        public int Volume { get; set; } = 50;
        public TimeSpan RepeatFor { get; set; } = TimeSpan.Zero;

        public SoundService() {

            InitializeMouseClickEffect();
        }

        private bool stillRepeat = false;

        public void Play(string filePath)
        {
            _soundPlayer.Close();

            _soundPlayer.Open(new Uri(filePath));

            if (RepeatFor != Timeout.InfiniteTimeSpan)
            {
                ThreadPool.RegisterWaitForSingleObject(new AutoResetEvent(false),
                    (_, _) => stillRepeat = false, null, RepeatFor, true);
            }

            stillRepeat = true;

            _soundPlayer.Volume = Volume;
            _soundPlayer.Play();

            _soundPlayer.MediaEnded += OnSoundEnd;

            void OnSoundEnd(object? sender, EventArgs e)
            {
                if (stillRepeat)
                {
                    _soundPlayer.Position = TimeSpan.Zero; // reset back to zero so we can play again from the start.
                    _soundPlayer.Play();
                }

                else
                {
                    _soundPlayer.Close();
                    _soundPlayer.MediaEnded -= OnSoundEnd;
                    stillRepeat = false;
                };
            }
        }

        public void Play(AlarmSound alarmSound)
        {
            string fileName = alarmSound.ToString();

            var theFile = string.Format("{0}" + SEPERATOR + "Assets" + SEPERATOR + "AlarmSounds"
                + SEPERATOR + "{1}.mp3", PROJECT_PATH, fileName);

            Play(theFile);
        }

        // as this need fast, we load the media player directly.
        public void PlayMouseClickEffect()
        {
            _soundPlayerMouseClick.Play();
            _soundPlayerMouseClick.Volume = Volume;
            _soundPlayerMouseClick.MediaEnded += OnSoundEnd;

            void OnSoundEnd(object? sender, EventArgs e)
            {
                _soundPlayerMouseClick.Position = TimeSpan.Zero;
                _soundPlayerMouseClick.Pause();
            }
        }

        private void InitializeMouseClickEffect()
        {
            var theFile = string.Format("{0}" + SEPERATOR + "Assets" + SEPERATOR + "SoundEffects"
              + SEPERATOR + "{1}.mp3", PROJECT_PATH, "MouseClick");

            _soundPlayerMouseClick.Open(new Uri(theFile));
        }

        public void Stop()
        {
            _soundPlayer.Stop();
            stillRepeat = false;
        }
    }
}
