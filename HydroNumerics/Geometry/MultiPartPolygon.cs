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

    /// <summary>
    /// Returns true if the polygon is inside or on the edge of this polygon
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool Contains(IXYPolygon p)
    {
      var inner = Polygons.Where(po => po.GetArea() > 0).Any(poly => poly.Contains(p));

      if (inner)
      {
        return !Polygons.Where(po => po.GetArea() < 0).Any(poly => poly.Contains(p));
      }
      return false;
    }



    public bool Contains(IXYPoint p)
    {
      return Contains(p.X, p.Y);
      //return Polygons.Any(poly => poly.Contains(p));
    }

    public bool Contains(double X, double Y)
    {

      var inner = Polygons.Where(po => po.GetArea() > 0).Any(poly => poly.Contains(X,Y));

      if (inner)
      {
        return !Polygons.Where(po => po.GetArea() < 0).Any(poly => poly.Contains(X,Y));
      }
      return false;

      
      //  return Polygons.Any(poly => poly.Contains(X,Y));
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
