﻿using ExtendedPomodoro.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ExtendedPomodoro.Converters
{
    public class HotkeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Hotkey val) return val.ToString();
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Hotkey val) return val.ToString();
            return string.Empty;
        }
    
    }
}
