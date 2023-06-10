using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedPomodoro.Services.Interfaces
{
    public interface IFlashMessageService
    {
        public TimeSpan Duration { get; set; }
        public bool IsFlashMessageOpened { get; }
        public event EventHandler<IsFlashMessageOpenedChangedEventArgs>? IsFlashMessageOpenedChanged;

        public void NewFlashMessage();
    }
}
