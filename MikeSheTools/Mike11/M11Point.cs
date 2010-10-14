using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.MikeSheTools.PFS.NWK11;

namespace HydroNumerics.MikeSheTools.Mike11
{
  /// <summary>
  /// Wrapper class for Point in Mike11. You CANNOT change X and Y coordinate through this class
  /// </summary>
  public class M11Point:XYPoint
  {
    private Point _pfsPoint;

    /// <summary>
    /// Gets and sets the Chainage
    /// </summary>
    public double Chainage
    {
      get
      {
        return _pfsPoint.Chainage;
      }
      set
      {
        _pfsPoint.Chainage = value;
      }
    }

    internal M11Point(Point P):base(P.X, P.Y)
    {
      _pfsPoint = P;
    }
  
  }
}
