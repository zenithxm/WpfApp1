
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1.Helper
{
    public class Bool2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean && targetType == typeof(Visibility))
            {
                // If parameter indicates inversion (e.g., "invert")
                bool invert = !string.IsNullOrEmpty(parameter?.ToString()) &&
                              parameter.ToString().ToLower() == "invert";

                return invert ? boolean ? Visibility.Collapsed : Visibility.Visible :
                               boolean ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility && targetType == typeof(bool))
            {
                // If parameter indicates inversion
                bool invert = !string.IsNullOrEmpty(parameter?.ToString()) &&
                              parameter.ToString().ToLower() == "invert";

                return invert ? visibility != Visibility.Visible :
                               visibility == Visibility.Visible;
            }

            return false;
        }
    }
}
