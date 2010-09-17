using System;
using System.Windows.Data;
using System.Globalization;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  /// <summary>
  /// Returns the negation of the given boolean value
  /// </summary>
  public class NotConverter : ConverterMarkupExtension<NotConverter>
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !(bool)value;
    }
  }
}