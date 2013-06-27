using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.MEX
{
  public class Node : PFSMapper
  {
    PFSKeyword _keyword;

    internal Node(PFSKeyword Section)
    {
      _keyword = Section;
    }


    public string NodeID
    {
      get
      {
        return _keyword.GetParameter(1).ToString();
      }
      set
      {
        _keyword.GetParameter(1).Value = value;
      }
    }

    public int TypeNo
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

    public double X
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


    public double Y
    {
      get
      {
        return _keyword.GetParameter(4).ToDouble();
      }
      set
      {
        _keyword.GetParameter(4).Value = value;
      }
    }


    public double Diameter
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

    public double InvertLevel
    {
      get
      {
        return _keyword.GetParameter(6).ToDouble();
      }
      set
      {
        _keyword.GetParameter(6).Value = value;
      }
    }

    public double GroundLevel
    {
      get
      {
        return _keyword.GetParameter(7).ToDouble();
      }
      set
      {
        _keyword.GetParameter(7).Value = value;
      }
    }

    public double WaterLevel
    {
      get
      {
        return _keyword.GetParameter(8).ToDouble();
      }
      set
      {
        _keyword.GetParameter(8).Value = value;
      }
    }

    public double CriticalLevel
    {
      get
      {
        return _keyword.GetParameter(9).ToDouble();
      }
      set
      {
        _keyword.GetParameter(9).Value = value;
      }
    }

    public string LossParID
    {
      get
      {
        return _keyword.GetParameter(17).ToString();
      }
      set
      {
        _keyword.GetParameter(17).Value = value;
      }
    }

  }
}
