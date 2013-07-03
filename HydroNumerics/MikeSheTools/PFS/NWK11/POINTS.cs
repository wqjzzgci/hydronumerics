using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public partial class POINTS
  {

    public point AddPoint()
    {
      point p = new point("point");
      _pfsHandle.AddKeyword(p._keyword);
      this.points.Add(p);
      return p;
    }
  }
}
