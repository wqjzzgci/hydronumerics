using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Geometry
{
  public class MultiPartPolygon:IGeometry,IXYPolygon
  {

    public MultiPartPolygon()
    {
      Polygons = new List<XYPolygon>();
    }

    public List<XYPolygon> Polygons { get; private set; }



    public bool Contains(IXYPoint p)
    {
      return Polygons.Any(poly => poly.Contains(p));
    }

    public bool Contains(double X, double Y)
    {
      return Polygons.Any(poly => poly.Contains(X,Y));
    }

    public double GetArea()
    {
      return Polygons.Sum(poly => poly.GetArea());
    }

    public bool OverLaps(IXYPolygon Poly)
    {
      return Polygons.Any(poly => poly.OverLaps(Poly));
    }
  }
}
