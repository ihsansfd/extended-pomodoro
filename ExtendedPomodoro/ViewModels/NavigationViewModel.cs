using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.ViewModels
{
    public partial class NavigationViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableObject _currentViewModel;

        public NavigationViewModel(ObservableObject currentViewModel)
        {
            CurrentViewModel = currentViewModel;

        }

        public void SetCurrentViewModel(ObservableObject currentViewModel)
        {
            CurrentViewModel = currentViewModel;
            StrongReferenceMessenger.Default.Send<CurrentViewModelMessage>(new(currentViewModel));
        }

    }

    public class CurrentViewModelMessage
    {
        public ObservableObject Message { get; }

        public CurrentViewModelMessage(ObservableObject message)
        {
            Message = message;
        }
    }
}
