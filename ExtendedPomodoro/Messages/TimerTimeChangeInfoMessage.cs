using System;

namespace ExtendedPomodoro.Messages
{
    public record class TimerTimeChangeInfoMessage(TimeSpan TimerSetFor, TimeSpan RemainingTime);
}
