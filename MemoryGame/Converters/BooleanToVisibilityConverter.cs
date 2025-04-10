using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemoryGame.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool shouldInvert = IsInverted;
            if (parameter is bool paramBool)
            {
                shouldInvert = paramBool;
            }
            else if (parameter is string paramString && bool.TryParse(paramString, out bool parsedBool))
            {
                shouldInvert = parsedBool;
            }

            if (value is bool boolValue)
            {
                boolValue = shouldInvert ? !boolValue : boolValue;
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return shouldInvert ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool shouldInvert = IsInverted;
            if (parameter is bool paramBool)
            {
                shouldInvert = paramBool;
            }
            else if (parameter is string paramString && bool.TryParse(paramString, out bool parsedBool))
            {
                shouldInvert = parsedBool;
            }

            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                return shouldInvert ? !result : result;
            }
            return shouldInvert;
        }
    }
}