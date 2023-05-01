using CommunityToolkit.Mvvm.Messaging;

namespace ExtendedPomodoro.Factories
{
    public class MessengerFactory
    {
        public static IMessenger Messenger { get; } = StrongReferenceMessenger.Default;
    }
}
