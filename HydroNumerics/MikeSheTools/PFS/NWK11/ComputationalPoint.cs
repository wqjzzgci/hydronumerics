using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class ComputationalPoint
  {
        private PFSKeyword _keyword;

    internal ComputationalPoint(PFSKeyword keyword)
    {
      _keyword = keyword; 
    }

        public double Chainage
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


    public int Par1
    {
      get
      {
        return _keyword.GetParameter(2).ToInt();
      }
      set
      {
        _keyword.GetParameter(2).Value = value;
      }
    }

    public int Par2
    {
      get
      {
        return _keyword.GetParameter(3).ToInt();
      }
      set
      {
        _keyword.GetParameter(3).Value = value;
      }
    }
    public int Par3
    {
      get
      {
        return _keyword.GetParameter(4).ToInt();
      }
      set
      {
        _keyword.GetParameter(4).Value = value;
      }
    }


  }
}
