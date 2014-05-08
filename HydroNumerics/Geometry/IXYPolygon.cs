using System;
namespace HydroNumerics.Geometry
{
  public interface IXYPolygon:IGeometry
  {
    bool Contains(IXYPoint p);
    bool Contains(double X, double Y);
    double GetArea();
    bool OverLaps(IXYPolygon Poly);
  }
}
