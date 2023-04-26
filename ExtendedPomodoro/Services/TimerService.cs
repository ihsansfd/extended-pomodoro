using ExtendedPomodoro.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services
{
    public class TimerService
    {
        private readonly DialogWindowService _dialogWindowService;
        public TimerService(DialogWindowService dialogWindowService) {
            _dialogWindowService = dialogWindowService;
        }

        public bool? OpenSessionFinishDialog()
        {
            var userControl = new ModalContentSessionFinishUserControl();
            var dialog = _dialogWindowService.GenerateClosableDialogWindow(
                new(320, 225, "Session Has Finished", userControl));
            return dialog.ShowDialog();
        }
    }
}
