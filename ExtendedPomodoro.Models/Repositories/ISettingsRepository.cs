using ExtendedPomodoro.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ISettingsRepository
    {
        Task<SettingsDomain> GetSettings();
        Task UpdateSettings(SettingsDomain settingsDomain);
        Task ResetToDefaultSettings();
    }
}
