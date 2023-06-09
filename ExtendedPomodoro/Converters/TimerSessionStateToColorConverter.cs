﻿using ExtendedPomodoro.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ExtendedPomodoro.Services;

namespace ExtendedPomodoro.Converters
{
    public class TimerSessionStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is TimerSessionState session)
            {
                if (session is PomodoroSessionState)
                    return ((SolidColorBrush) Application.Current.FindResource("Primary")).Color;

                if (session is ShortBreakSessionState)
                    return ((SolidColorBrush)Application.Current.FindResource("Info")).Color;

                if (session is LongBreakSessionState)
                    return ((SolidColorBrush)Application.Current.FindResource("Success")).Color;
            }

            if (value is string sessionString)
            {
                if (sessionString == nameof(PomodoroSessionState))
                    return ((SolidColorBrush)Application.Current.FindResource("Primary")).Color;

                if (sessionString is nameof(ShortBreakSessionState))
                    return ((SolidColorBrush)Application.Current.FindResource("Info")).Color;

                if (sessionString is nameof(LongBreakSessionState))
                    return ((SolidColorBrush)Application.Current.FindResource("Success")).Color;
            }

            return ((SolidColorBrush)Application.Current.FindResource("Primary")).Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
