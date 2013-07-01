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
    private point _pfsPoint;

    /// <summary>
    /// Gets and sets the Chainage
    /// </summary>
    public double Chainage
    {
      get
      {
        return _pfsPoint.Par5;
      }
      set
      {
        _pfsPoint.Par5 = value;
      }
    }

    internal M11Point(point P):base(P.Par2, P.Par3)
    {
      _pfsPoint = P;
    }
  
  }
}
