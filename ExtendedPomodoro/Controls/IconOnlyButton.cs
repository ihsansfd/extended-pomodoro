using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ExtendedPomodoro.Controls
{
    public class IconOnlyButton : Button
    {

        static IconOnlyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconOnlyButton), new FrameworkPropertyMetadata(typeof(IconOnlyButton)));
        }

        public PackIconBootstrapIconsKind Kind
        {
            get { return (PackIconBootstrapIconsKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(PackIconBootstrapIconsKind), typeof(IconOnlyButton), new PropertyMetadata(PackIconBootstrapIconsKind.BookFill));

        public Brush HoverColor
        {
            get { return (Brush)GetValue(HoverColorProperty); }
            set { SetValue(HoverColorProperty, value); }
        }

        public static readonly DependencyProperty HoverColorProperty =
            DependencyProperty.Register("HoverColor", typeof(Brush), typeof(IconOnlyButton), new PropertyMetadata(null));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (HoverColor != null)
            {
                var hoverColor = HoverColor;
                var foreground = Foreground;
                MouseEnter += (object sender, MouseEventArgs e) =>
                {
                    Foreground = hoverColor;
                };

                MouseLeave += (object sender, MouseEventArgs e) =>
                {
                    Foreground = foreground;
                };
            }
        }
    }
}
