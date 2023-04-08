using ExtendedPomodoro.Converters;
using ExtendedPomodoro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ExtendedPomodoro.Controls
{
    public class HotkeyTextBox : TextBox
    {
        public Hotkey? Hotkey
        {
            get => (Hotkey)GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        public static readonly DependencyProperty HotkeyProperty =
        DependencyProperty.Register(nameof(Hotkey),typeof(Hotkey),typeof(HotkeyTextBox), 
            new FrameworkPropertyMetadata(default(Hotkey),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PreviewKeyDown += HotkeyTextBox_PreviewKeyDown;
        }

        public HotkeyTextBox()
        {
            Binding binding = new Binding(nameof(Hotkey));
            binding.Source = this;
            binding.Converter = new HotkeyToStringConverter();
            SetBinding(TextProperty, binding);
        }
        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool IsNotActualKey(Key key)
            {
                return key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt ||
                key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LWin ||
                key == Key.RWin ||
                key == Key.Clear ||
                key == Key.OemClear ||
                key == Key.Apps;
            }

            base.OnKeyDown(e);

            // Don't let the event pass further
            // because we don't want standard textbox shortcuts working
            e.Handled = true;

            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // When Alt is pressed, SystemKey is used instead
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None &&
                (key == Key.Delete || key == Key.Back || key == Key.Escape))
            {
                Hotkey = null;
                return;
            }

            if (IsNotActualKey(key))
            {
                return;
            };

            Hotkey = new Hotkey(key, modifiers);

        }
    }
}
