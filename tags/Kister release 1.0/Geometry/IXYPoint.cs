using System;
namespace HydroNumerics.Geometry
{
  public interface IXYPoint
  {
    bool Equals(object obj);
    int GetHashCode();
    double X { get; set; }
    double Y { get; set; }
  }
}
