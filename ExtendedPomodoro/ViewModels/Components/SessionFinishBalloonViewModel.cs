using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExtendedPomodoro.Messages;
using ExtendedPomodoro.Services;

namespace ExtendedPomodoro.ViewModels.Components
{
    public partial class SessionFinishBalloonViewModel : AutoCloseControlViewModel
    {
        public event EventHandler? Closed;
        
        private readonly IMessenger _messenger;

        public SessionFinishBalloonViewModel(IMessenger messenger)
        {
            _messenger = messenger;
        }

        [ObservableProperty]
        private TimerSessionState _finishedSession = null!;

        [ObservableProperty]
        private TimerSessionState _nextSession = null!;

        protected override void Close()
        {
            base.Close();
            Closed?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void StartNextSession()
        {
            _messenger.Send(new StartSessionInfoMessage());
            Close();
        }
    }
}
