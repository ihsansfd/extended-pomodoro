namespace ExtendedPomodoro.Models.Domains
{

    public record class DailySessionTaskLinkDomain(DateOnly SessionDate, int TaskId, bool IsTaskCompletedInThisSession);
}
