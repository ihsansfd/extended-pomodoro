using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace ExtendedPomodoro.Core
{

    // A simple extension for making animation for solidcolorbrush instead of color
    public class SolidColorBrushAnimation : ColorAnimation
    {
        public SolidColorBrush? ToBrush
        {
            get { return To == null ? null : new SolidColorBrush(To.Value); }
            set { To = value?.Color; }
        }
    }
}
