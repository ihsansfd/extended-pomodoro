using System;

namespace ExtendedPomodoro.Entities
{
    public record class TimerTimeChangeInfoMessage(TimeSpan TimerSetFor, TimeSpan RemainingTime);
}
