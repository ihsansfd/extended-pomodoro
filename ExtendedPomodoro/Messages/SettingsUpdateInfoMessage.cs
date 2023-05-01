using ExtendedPomodoro.Models.Domains;
using ExtendedPomodoro.Services.Entities;

namespace ExtendedPomodoro.Messages
{
    public record class SettingsUpdateInfoMessage(AppSettings AppSettings);
}
