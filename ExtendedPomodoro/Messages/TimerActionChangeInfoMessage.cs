using ExtendedPomodoro.ViewModels;

namespace ExtendedPomodoro.Messages
{
    public enum TimerAction
    {
        Start = 0,
        Pause = 1,
        Reset = 2,
        Skip = 3,
    }

    public record class TimerActionChangeInfoMessage(TimerSessionState CurrentSession,
        TimerAction TimerAction, bool TriggeredByHotkey = false, bool PushNotificationEnabled = true);
}
