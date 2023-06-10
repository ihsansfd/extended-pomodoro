using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ExtendedPomodoro.Services;
using ExtendedPomodoro.Services.Interfaces;

namespace ExtendedPomodoro.ViewModels
{
    public enum FlashMessageType
    {
        SUCCESS = 0,
        WARNING = 1,
        ERROR = 2
    }

    public interface IFlashMessageServiceViewModel
    {
        public FlashMessageType Type { get; set; }
        public bool IsOpened { get; set; }
        public string Message { get; set; }
        public void NewFlashMessage(FlashMessageType type, string message);
    }


    // TODO: UNTESTED
    public partial class FlashMessageServiceViewModel : ObservableObject, IFlashMessageServiceViewModel
    {
        [ObservableProperty] 
        private FlashMessageType _type;

        [ObservableProperty] 
        private bool _isOpened;

        [ObservableProperty] 
        private string _message = string.Empty;

        private readonly IFlashMessageService _flashMessageService;

        public FlashMessageServiceViewModel(IFlashMessageService flashMessageService)
        {
            _flashMessageService = flashMessageService;
            _flashMessageService.IsFlashMessageOpenedChanged += FlashMessageService_OnIsFlashMessageOpenedChanged;

        }

        public void NewFlashMessage(FlashMessageType type, string message)
        {
            Message = message;
            Type = type;
            _flashMessageService.NewFlashMessage();
        }

        private void FlashMessageService_OnIsFlashMessageOpenedChanged(object? sender, IsFlashMessageOpenedChangedEventArgs args)
        {
            IsOpened = args.IsFlashMessageOpened;
        }

        ~FlashMessageServiceViewModel()
        {
            _flashMessageService.IsFlashMessageOpenedChanged -= FlashMessageService_OnIsFlashMessageOpenedChanged;
        }
    }
}
