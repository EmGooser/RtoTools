using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RtoTools.View.Converters
{
    internal class BoolToDoubleConverter : IValueConverter
    {
        public double OnTrue { get; set; }

        public double OnFalse { get; set; }

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
            else if (value is double)
            {
                return (double)value == OnTrue;
            }
            else if (value is double?)
            {
                return (double?)value == OnTrue;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
