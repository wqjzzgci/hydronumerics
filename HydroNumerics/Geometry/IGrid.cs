using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Geometry
{
  /// <summary>
  /// Interface to define a grid
  /// </summary>
  public interface IGrid
  {
     int NumberOfColumns { get; }
     int NumberOfRows { get; }
     double GridSize { get; }
     double XOrigin { get; }
     double YOrigin { get; }
     double Orientation { get; }
     bool OriginAtCenter { get;}
  }
}
