using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ExtendedPomodoro.ViewModels
{
    public partial class NavigationViewModel : ObservableObject
    {
        private readonly IMessenger _messenger;

        [ObservableProperty]
        private ObservableObject _currentViewModel = null!;

        public NavigationViewModel(ObservableObject currentViewModel, IMessenger messenger)
        {
            CurrentViewModel = currentViewModel;
            _messenger = messenger;

        }

        public void SetCurrentViewModel(ObservableObject currentViewModel)
        {
            CurrentViewModel = currentViewModel;
            _messenger.Send<CurrentViewModelMessage>(new(currentViewModel));
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
