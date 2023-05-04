using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace ExtendedPomodoro.ViewModels.Components
{
    public partial class TimerPausedBalloonViewModel : AutoCloseControlViewModel
    {
        [ObservableProperty]
        private TimerSessionState _currentSession = null!;

        [RelayCommand]
        public override void Close() => base.Close();
    }
}
