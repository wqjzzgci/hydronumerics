using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  [ValueConversion(typeof(object), typeof(bool))]
  public class NullToBooleanConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      if (value != null)
        return true;
      else
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      return Convert(value, targetType, parameter, culture);
    }

    #endregion
  }
}
