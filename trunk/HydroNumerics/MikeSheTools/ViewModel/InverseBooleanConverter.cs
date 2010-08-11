using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  [ValueConversion(typeof(bool), typeof(bool))]
  public class InverseBooleanConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {

      if (targetType == typeof(bool))
        return !(bool)value;
      else if(targetType == typeof(bool?))
      {
        bool? v=(bool?)value;
        if (v.HasValue)
          v = !((bool?)value).Value;
        return v;
      }
      else
          throw new InvalidOperationException("The target must be a boolean");
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      return Convert(value,targetType, parameter,culture);
    }

    #endregion
  }
}
