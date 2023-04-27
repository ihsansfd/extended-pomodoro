using ExtendedPomodoro.Models.Domains;

namespace ExtendedPomodoro.Models.Repositories
{
    public interface ISettingsRepository
    {
        Task<SettingsDomain> GetSettings();
        Task UpdateSettings(SettingsDomain settingsDomain);
        Task ResetToDefaultSettings();
    }
}
