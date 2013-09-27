using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

using HydroNumerics.MikeSheTools.Mike11;
using HydroNumerics.Core.WPF;

namespace HydroNumerics.MikeSheTools.Mike11View
{
  public class SelectManyConverter : ConverterMarkupExtension<SelectManyConverter>
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      System.Collections.IList items = (System.Collections.IList)value;
      return items.Cast<M11Branch>().SelectMany(b => b.CrossSections).ToList();

    }

  }
}
