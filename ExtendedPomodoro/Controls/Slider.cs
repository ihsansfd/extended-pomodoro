using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ExtendedPomodoro.Controls
{
    public class ExtendedSlider : Slider
    {
        public static readonly RoutedEvent OnClickMeClickedEvent =
    EventManager.RegisterRoutedEvent("OnDrag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ExtendedSlider));
    }
}
