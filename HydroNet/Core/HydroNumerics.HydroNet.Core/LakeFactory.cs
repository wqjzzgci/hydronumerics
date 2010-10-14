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
        Lake L = new Lake(1);
        
        L.SurfaceArea = (XYPolygon)l.Geometry;
        L.Name = (string)l.Data[0];
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
