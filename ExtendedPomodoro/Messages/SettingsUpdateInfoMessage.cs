using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;

namespace ExtendedPomodoro.Messages
{
    public record SettingsUpdateInfoMessage(AppSettings AppSettings);
}
