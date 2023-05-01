using ExtendedPomodoro.Services.Interfaces;
using System.Windows;

namespace ExtendedPomodoro.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string messageBoxText)
        {
            return MessageBox.Show(messageBoxText);
        }
        public MessageBoxResult Show(string messageBoxText, string caption)
        {
            return MessageBox.Show(messageBoxText, caption);
        }
        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return MessageBox.Show(messageBoxText, caption, button);

        }
        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return MessageBox.Show(messageBoxText, caption, button, icon);
        }
      
    }
}
