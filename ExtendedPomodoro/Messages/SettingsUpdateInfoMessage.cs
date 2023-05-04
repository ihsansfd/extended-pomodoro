using ExtendedPomodoro.Entities;
using ExtendedPomodoro.Models.Domains;

namespace ExtendedPomodoro.Messages
{
    public record class SettingsUpdateInfoMessage(AppSettings AppSettings);
}
