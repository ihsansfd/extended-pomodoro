using CommunityToolkit.Mvvm.Messaging;

namespace ExtendedPomodoro.Services
{
    public class MessengerService
    {
        public static IMessenger Messenger { get; } = StrongReferenceMessenger.Default;
    }
}
