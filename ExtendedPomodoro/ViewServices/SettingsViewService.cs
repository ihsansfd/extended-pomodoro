using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedPomodoro.ViewServices.Interfaces;

namespace ExtendedPomodoro.ViewServices
{
    public class SettingsViewService : ISettingsViewService
    {
        private readonly AlarmSoundService _alarmSoundService;

        public SettingsViewService(AlarmSoundService alarmSoundService ) {
            _alarmSoundService = alarmSoundService;
        }

        public void PlaySound(AlarmSound alarmSound, int volume, TimeSpan repeatFor)
        {
            _alarmSoundService.Volume = volume;
            _alarmSoundService.RepeatFor = repeatFor;
            _alarmSoundService.AlarmSound = alarmSound;
            _alarmSoundService.Play();
        }
    }
}
