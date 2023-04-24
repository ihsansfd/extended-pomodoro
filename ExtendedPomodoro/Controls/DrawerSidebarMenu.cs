using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtendedPomodoro.Controls
{

    public class DrawerSidebarMenu : ContentControl
    {
        public DrawerSidebarMenu()
        {
            Width = ClosedWidth;
        }

        public double ClosedWidth
        {
            get { return (double)GetValue(ClosedWidthProperty); }
            set { SetValue(ClosedWidthProperty, value); }
        }

        public static readonly DependencyProperty ClosedWidthProperty =
            DependencyProperty.Register("ClosedWidth", typeof(double), typeof(DrawerSidebarMenu), new PropertyMetadata(50.0));

        public bool? IsOpened
        {
            get { return (bool?)GetValue(IsOpenedProperty); }
            set { SetValue(IsOpenedProperty, value); }
        }

        public static readonly DependencyProperty IsOpenedProperty =
            DependencyProperty.Register("IsOpened", typeof(bool?), typeof(DrawerSidebarMenu),
                new PropertyMetadata(null, new PropertyChangedCallback(OnIsOpenedChanged)));



        public double OpenedWidth
        {
            get { return (double)GetValue(OpenedWidthProperty); }
            set { SetValue(OpenedWidthProperty, value); }
        }


        public static readonly DependencyProperty OpenedWidthProperty =
            DependencyProperty.Register("OpenedWidth", typeof(double), typeof(DrawerSidebarMenu), new PropertyMetadata(250.0));

        static DrawerSidebarMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerSidebarMenu), new FrameworkPropertyMetadata(typeof(DrawerSidebarMenu)));
        }


        private static void OnIsOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrawerSidebarMenu drawerMenu)
            {
                if (drawerMenu.IsOpened == true)
                {
                    drawerMenu.OpenAnimation();
                }

                else
                {
                    drawerMenu.CloseAnimation();
                }
            }
        }

        private void CloseAnimation()
        {
            DoubleAnimation closingAnimation = new(ClosedWidth, new Duration(TimeSpan.FromSeconds(0.2)));
            BeginAnimation(WidthProperty, closingAnimation);
        }

        private void OpenAnimation()
        {
            DoubleAnimation openingAnimation = new(OpenedWidth, new Duration(TimeSpan.FromSeconds(0.2)));
            BeginAnimation(WidthProperty, openingAnimation);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (IsOpened == null) // if the user has defined his logic for IsOpened, don't bother using our own logic.
            {
                Binding bindingForIsOpened = new("IsChecked") { Source = GetTemplateChild("PART_MenuToggle") };
                SetBinding(IsOpenedProperty, bindingForIsOpened);
            }

        }

        public Style MenuTogglerStyle
        {
            get { return (Style)GetValue(MenuTogglerStyleProperty); }
            set { SetValue(MenuTogglerStyleProperty, value); }
        }

        public static readonly DependencyProperty MenuTogglerStyleProperty =
            DependencyProperty.Register("MenuTogglerStyle", typeof(Style), typeof(DrawerSidebarMenu), new PropertyMetadata(null));

    }

    public class DrawerSidebarMenuItem : Control
    {
        //public Brush ForegroundTemp { get; }

        public Brush ForegroundTemp
        {
            get { return (Brush)GetValue(ForegroundTempProperty); }
            set { SetValue(ForegroundTempProperty, value); }
        }

        public static readonly DependencyProperty ForegroundTempProperty =
            DependencyProperty.Register("ForegroundTemp", typeof(Brush), typeof(DrawerSidebarMenuItem), new PropertyMetadata(OnForegroundTempChanged));

        static DrawerSidebarMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DrawerSidebarMenuItem), new FrameworkPropertyMetadata(typeof(DrawerSidebarMenuItem)));
        }

        //public DrawerSidebarMenuItem()
        //{
        //    ForegroundTemp = Foreground;
        //}

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(DrawerSidebarMenuItem),
            new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(DrawerSidebarMenuItem), new PropertyMetadata(null));

        public PackIconBootstrapIconsKind Kind
        {
            get { return (PackIconBootstrapIconsKind)GetValue(KindProperty); }
            set { SetValue(KindProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Kind.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(PackIconBootstrapIconsKind), typeof(DrawerSidebarMenuItem), new PropertyMetadata(PackIconBootstrapIconsKind.BookFill));

        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(DrawerSidebarMenuItem), new PropertyMetadata(30.0));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DrawerSidebarMenuItem), new PropertyMetadata(""));

        public double Space
        {
            get { return (double)GetValue(SpaceProperty); }
            set { SetValue(SpaceProperty, value); }
        }

        public static readonly DependencyProperty SpaceProperty =
            DependencyProperty.Register("Space", typeof(double), typeof(DrawerSidebarMenuItem), new PropertyMetadata(0.0));


        public SolidColorBrush HoverColor
        {
            get { return (SolidColorBrush)GetValue(HoverColorProperty); }
            set { SetValue(HoverColorProperty, value); }
        }

        public static readonly DependencyProperty HoverColorProperty =
            DependencyProperty.Register("HoverColor", typeof(SolidColorBrush), typeof(DrawerSidebarMenuItem), new PropertyMetadata(null));



        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(DrawerSidebarMenuItem), new PropertyMetadata(false, OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DrawerSidebarMenuItem menuItem)
            {
                if (menuItem.IsActive)
                {
                    menuItem.Foreground = menuItem.HoverColor;
                }

                else
                {
                    menuItem.Foreground = menuItem.ForegroundTemp;

                }
            }
        }

        private static void OnForegroundTempChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnIsActiveChanged(d, e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (HoverColor != null)
            {
                MouseEnter += (object sender, MouseEventArgs e) =>
                {
                    Foreground = HoverColor;
                };

                MouseLeave += (object sender, MouseEventArgs e) =>
                {
                    if (this.IsActive) Foreground = HoverColor;
                    else Foreground = ForegroundTemp;
                };
            }

        }



    }
}
