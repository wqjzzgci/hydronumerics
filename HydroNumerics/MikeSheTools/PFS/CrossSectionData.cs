using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS
{
  public class CrossSectionData
  {

        PFSKeyword _keyword;

        internal CrossSectionData(PFSKeyword keyword)
    {
      _keyword = keyword;
    }

        public double X
    {
      get
      {
        return _keyword.GetParameter(1).ToDouble();
      }
      set
      {
        _keyword.GetParameter(1).Value = value;
      }
    }


    public double Z
    {
      get
      {
        return _keyword.GetParameter(2).ToDouble();
      }
      set
      {
        _keyword.GetParameter(2).Value = value;
      }
    }

  }
}
