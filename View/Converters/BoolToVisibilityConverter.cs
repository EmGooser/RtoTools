using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RtoTools.View.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility OnTrue { get; set; } = Visibility.Visible;

        public Visibility OnFalse { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return OnFalse;
            }
            else if (value is bool)
            {
                return (bool)value ? OnTrue : OnFalse;
            }
            else if (value is bool?)
            {
                return ((bool?)value == true) ? OnTrue : OnFalse;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is Visibility)
            {
                return (Visibility)value == OnTrue;
            }
            else if (value is Visibility?)
            {
                return (Visibility?)value == OnTrue;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
