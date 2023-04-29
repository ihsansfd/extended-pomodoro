using ExtendedPomodoro.Models.Domains;

namespace ExtendedPomodoro.Models.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<SettingsDomain> GetSettings();
        Task UpdateSettings(SettingsDomain settingsDomain);
        Task ResetToDefaultSettings();
    }
}
