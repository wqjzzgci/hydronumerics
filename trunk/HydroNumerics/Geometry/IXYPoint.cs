using System;
namespace HydroNumerics.Geometry
{
  public interface IXYPoint:IGeometry
  {
    bool Equals(object obj);
    int GetHashCode();
    double X { get; set; }
    double Y { get; set; }
  }
}
