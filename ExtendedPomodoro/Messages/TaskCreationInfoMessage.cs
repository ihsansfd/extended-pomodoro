namespace ExtendedPomodoro.Messages
{
    public record TaskCreationInfoMessage(bool IsSuccess, string Message, object Source);
}
