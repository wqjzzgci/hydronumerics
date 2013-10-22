using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

using Microsoft.Maps.MapControl.WPF;
using HydroNumerics.Geometry;
using HydroNumerics.Core.WPF;

using DotSpatial.Data;


namespace HydroNumerics.Nitrate.View
{
  public class GeometryToLocationConverter : ConverterMarkupExtension<GeometryToLocationConverter>
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var Locations = new LocationCollection();

       if (value is XYPolygon)
      {
        foreach (var p in ((XYPolygon)value).Points.Cast<XYPoint>().Where(pp => pp.Latitude != 0))
          Locations.Add(new Location(p.Latitude, p.Longitude));
        Locations.Add(Locations.First());
      }
      else if (value is XYPolyline)
      {
        foreach (var p in (( XYPolyline )value).Points.Cast<XYPoint>().Where(pp => pp.Latitude != 0))
          Locations.Add(new Location(p.Latitude, p.Longitude));
      }
      else if (value is IFeature)
      {
        foreach (var p in ((IFeature)value).Coordinates)
          Locations.Add(new Location(p.Y, p.X));
      }
      return Locations;
    }
  }
}


