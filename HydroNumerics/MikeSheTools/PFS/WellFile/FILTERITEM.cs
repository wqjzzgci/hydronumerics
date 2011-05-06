using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.WellFile
{
  public class FILTERITEM:PFSMapper
  {
    internal FILTERITEM(PFSSection Section)
    {
      _pfsHandle = Section;

    }

    public double Top
    {
      get
      {
        return _pfsHandle.GetKeyword("Top", 1).GetParameter(1).ToDouble();
      }
    }

    public double Bottom
    {
      get
      {
        return _pfsHandle.GetKeyword("Bottom", 1).GetParameter(1).ToDouble();
      }
    }



  }
}
