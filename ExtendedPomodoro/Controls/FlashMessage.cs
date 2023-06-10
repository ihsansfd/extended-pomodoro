using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ExtendedPomodoro.Factories;
using ExtendedPomodoro.ViewModels;

namespace ExtendedPomodoro.Controls
{
    public class FlashMessage : Control
    {

        static FlashMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlashMessage), 
                new FrameworkPropertyMetadata(typeof(FlashMessage)));
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), 
                typeof(FlashMessage), new PropertyMetadata(string.Empty));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), 
                typeof(FlashMessage), new PropertyMetadata(false));

        public FlashMessageType FlashMessageType
        {
            get => (FlashMessageType)GetValue(FlashMessageTypeProperty);
            set => SetValue(FlashMessageTypeProperty, value);
        }

        public static readonly DependencyProperty FlashMessageTypeProperty =
            DependencyProperty.Register(nameof(FlashMessageType), typeof(FlashMessageType), 
                typeof(FlashMessage), new PropertyMetadata(FlashMessageType.SUCCESS));

        public double TextMaxWidth
        {
            get { return (double)GetValue(TextMaxWidthProperty); }
            set { SetValue(TextMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty TextMaxWidthProperty =
            DependencyProperty.Register(nameof(TextMaxWidth), typeof(double), typeof(FlashMessage),
                new PropertyMetadata(999.0));

        public UIElement PlacementTarget
        {
            get => (UIElement)GetValue(PlacementTargetProperty);
            set => SetValue(PlacementTargetProperty, value);
        }

        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.Register(nameof(PlacementTarget), typeof(UIElement), typeof(FlashMessage));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template.FindName("PART_Popup", this) is Popup itsPopup)
            {
                itsPopup.CustomPopupPlacementCallback = GeneratePopupPlacement;
            }
        }

        private CustomPopupPlacement[] GeneratePopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            var point = new Point(MainWindowFactory.MainWindow.ActualWidth, MainWindowFactory.MainWindow.ActualHeight);
            return new[] { new CustomPopupPlacement { Point = new Point(targetSize.Width - popupSize.Width + offset.X, targetSize.Height - popupSize.Height + offset.Y) } };
        }
    }
}
