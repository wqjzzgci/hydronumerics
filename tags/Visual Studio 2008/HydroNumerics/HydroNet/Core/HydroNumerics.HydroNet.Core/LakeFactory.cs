using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.HydroNet.Core
{
  public class LakeFactory
  {
    public static Lake GetLake(string Name)
    {
      ShapeReader psp = new ShapeReader(@"soervp1.shp");
      foreach (var l in psp.GeoData)
      {
        Lake L = new Lake((string)l.Data[0], (XYPolygon)l.Geometry);
        
        if (L.Name.ToLower().Equals(Name.ToLower()))
        {
          psp.Dispose();
          return L;
        }
      }
      psp.Dispose();
      return null;
    }
  }
}
