using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  [ValueConversion(typeof(bool), typeof(FontStyle))]
  public class BooleanToFontStyleConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {

      if ((bool)value)
        return FontStyles.Oblique;
      else
        return FontStyles.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      return Convert(value, targetType, parameter, culture);
    }

    #endregion
  }
}
