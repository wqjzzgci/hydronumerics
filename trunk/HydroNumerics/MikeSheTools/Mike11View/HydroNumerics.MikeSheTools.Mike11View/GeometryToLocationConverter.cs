using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

using Microsoft.Maps.MapControl.WPF;
using HydroNumerics.Geometry;
using HydroNumerics.MikeSheTools.Mike11;
using HydroNumerics.Core.WPF;

namespace HydroNumerics.MikeSheTools.Mike11View
{
  public class GeometryToLocationConverter : ConverterMarkupExtension<GeometryToLocationConverter>
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

      if (value is XYPolyline)
      {
        var Locations = new LocationCollection();
        foreach (var p in ((XYPolyline)value).Points.Cast<XYPoint>().Where(pp => pp.Latitude != 0))
          Locations.Add(new Location(p.Latitude, p.Longitude));
        return Locations;
      }
      else if(value is IXYPoint)
      {
        XYPoint p = new XYPoint(((IXYPoint)value).X, ((IXYPoint)value).Y);

        return new Location(p.Latitude, p.Longitude);
          }
      return new LocationCollection(); 
    }
  }
}


