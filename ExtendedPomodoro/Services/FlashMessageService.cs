using ExtendedPomodoro.Core.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtendedPomodoro.Services.Interfaces;

namespace ExtendedPomodoro.Services
{
    public class IsFlashMessageOpenedChangedEventArgs : EventArgs
    {
        public IsFlashMessageOpenedChangedEventArgs(bool isFlashMessageOpened)
        {
            IsFlashMessageOpened = isFlashMessageOpened;
        }

        public bool IsFlashMessageOpened { get; }
    }

    // TODO: UNTESTED
    public class FlashMessageService : IFlashMessageService
    {
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);
        public bool IsFlashMessageOpened { get; private set; }
        public event EventHandler<IsFlashMessageOpenedChangedEventArgs>? IsFlashMessageOpenedChanged; 

        private int _activeFlashMessagesCount = 0;

        private readonly RegisterWaitTimeoutCallback _registerWaitTimeoutCallback;

        public FlashMessageService(RegisterWaitTimeoutCallback registerWaitTimeoutCallback)
        {
            _registerWaitTimeoutCallback = registerWaitTimeoutCallback;
        }

        public void NewFlashMessage()
        {
            IsFlashMessageOpenedChanged?.Invoke(this,
                new IsFlashMessageOpenedChangedEventArgs(IsFlashMessageOpened = true));

            _activeFlashMessagesCount++;

            _registerWaitTimeoutCallback.Invoke(() =>
            {
                _activeFlashMessagesCount--;
                if (_activeFlashMessagesCount <= 0)
                {
                    IsFlashMessageOpened = false;
                    IsFlashMessageOpenedChanged?.Invoke(this, 
                        new IsFlashMessageOpenedChangedEventArgs(IsFlashMessageOpened));
                }

            }, Duration);
        }
    }
}
