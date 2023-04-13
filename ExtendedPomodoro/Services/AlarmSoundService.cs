using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ExtendedPomodoro.Services
{
    // This is to be used in view layer, look at System.Windows.Media namespace
    public class AlarmSoundService
    {
        private MediaPlayer _soundPlayer = new();

        public int Volume { get; set; } = 50;
        public TimeSpan RepeatFor { get; set; } = TimeSpan.FromSeconds(15);

        private bool stillRepeat = false;

        public void Play(AlarmSound alarmSound)
        {
            _soundPlayer.Close();

            string fileName = alarmSound.ToString();

            var seperator = System.IO.Path.DirectorySeparatorChar;
            var projectPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(seperator);

            var theFile = string.Format("{0}" + seperator + "Assets" + seperator + "AlarmSounds"
                + seperator + "{1}.mp3", projectPath, fileName);
            _soundPlayer.Open(new Uri(theFile));

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
                    _soundPlayer.MediaEnded -= OnSoundEnd;
                    stillRepeat = false;
                };
            }
        }

        public void Stop()
        {
            _soundPlayer.Stop();
            stillRepeat = false;
        }
    }
}
