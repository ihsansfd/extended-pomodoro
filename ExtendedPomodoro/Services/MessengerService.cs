using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services
{
    public class MessengerService
    {
        public static IMessenger Messenger { get; } = StrongReferenceMessenger.Default;
    }
}
