using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class Point
  {

    private PFSKeyword _keyword;

    internal Point(PFSKeyword keyword)
    {
      _keyword = keyword; 
    }

    public int Number
    {
      get
      {
        return _keyword.GetParameter(1).ToInt();
      }
      set
      {
        _keyword.GetParameter(1).Value = value;
      }
    }

    public double X
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

    public double Y
    {
      get
      {
        return _keyword.GetParameter(3).ToDouble();
      }
      set
      {
        _keyword.GetParameter(3).Value = value;
      }
    }

    public double Chainage
    {
      get
      {
        return _keyword.GetParameter(5).ToDouble();
      }
      set
      {
        _keyword.GetParameter(5).Value = value;
      }
    }

  }
}
