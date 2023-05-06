using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewServices.Interfaces
{
    public interface ISettingsViewService
    {
        void PlaySound(AlarmSound alarmSound, int volume, TimeSpan repeatFor);
    }
}
