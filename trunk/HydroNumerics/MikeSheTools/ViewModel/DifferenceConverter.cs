using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class DifferenceConverter : ConverterMarkupExtension<DifferenceConverter>
  {
    public override object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      double first;
      double second;

      bool firstfound = double.TryParse(values[0].ToString(), out first);
      bool secondfound = double.TryParse(values[1].ToString(), out second);

      if (firstfound & secondfound)
        return Math.Abs((first - second)).ToString();

      return null;
    }
  }
}
