﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExtendedPomodoro.Services;

namespace ExtendedPomodoro.ViewModels.Components
{
    public partial class TimerStartedBalloonViewModel : AutoCloseControlViewModel
    {
        [ObservableProperty]
        private TimerSessionState _currentSession = null!;

    }
}
