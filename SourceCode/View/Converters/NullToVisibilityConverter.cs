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
    internal class NullToVisibilityConverter : IValueConverter
    {
        public Visibility OnNull { get; set; } = Visibility.Collapsed;
        public Visibility OnNotNull { get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return OnNull;
            else
                return OnNotNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Can't convert back to of type object from NullToVisibility");
        }
    }
}
