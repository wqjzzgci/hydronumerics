using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace HydroNumerics.Core.WPF
{
  [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
  public class BooleanToColorConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {

      if ((bool)value)
        return Brushes.Red;
      else
        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      return Convert(value, targetType, parameter, culture);
    }

    #endregion
  }
}
