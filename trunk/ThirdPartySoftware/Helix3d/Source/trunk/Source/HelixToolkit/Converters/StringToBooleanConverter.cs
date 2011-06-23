using System;
using System.Globalization;
using System.Windows.Data;

namespace HelixToolkit
{
    [ValueConversion(typeof (String), typeof (bool))]
    public class StringToBooleanConverter : SelfProvider, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;
            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if true, return the ConverterParameter
            if ((bool)value)
                return parameter;
            return Binding.DoNothing;
        }

        #endregion
    }
}