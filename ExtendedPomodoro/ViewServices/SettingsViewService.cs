using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewServices
{
    public class SettingsViewService
    {
        SoundService _soundService;

        public SettingsViewService(SoundService soundService ) { 
            _soundService = soundService;
        }

        public void PlaySound(AlarmSound alarmSound, int volume, TimeSpan repeatFor)
        {
            _soundService.Volume = volume;
            _soundService.RepeatFor = repeatFor;
            _soundService.Play(alarmSound);
        }
    }
}
