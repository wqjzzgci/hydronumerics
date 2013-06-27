using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;


namespace HydroNumerics.MikeSheTools.PFS.MEX
{
  public class Link:PFSMapper
  {

    PFSKeyword _keyword;

    internal Link(PFSKeyword Section)
    {
      _keyword = Section;
    }


    public string LinkID
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

    public string FromNode
    {
      get
      {
        return _keyword.GetParameter(2).ToString();
      }
      set
      {
        _keyword.GetParameter(2).Value = value;
      }
    }

    public string ToNode
    {
      get
      {
        return _keyword.GetParameter(3).ToString();
      }
      set
      {
        _keyword.GetParameter(3).Value = value;
      }
    }

    public int TypeNo
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

    public string CrsID
    {
      get
      {
        return _keyword.GetParameter(5).ToString();
      }
      set
      {
        _keyword.GetParameter(5).Value = value;
      }
    }

    public int ScalingTypeNo
    {
      get
      {
        return _keyword.GetParameter(6).ToInt();
      }
      set
      {
        _keyword.GetParameter(6).Value = value;
      }
    }

    public double Diameter
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


    public double UpLevel
    {
      get
      {
        return _keyword.GetParameter(11).ToDouble();
      }
      set
      {
        _keyword.GetParameter(11).Value = value;
      }
    }


    public double DwLevel
    {
      get
      {
        return _keyword.GetParameter(12).ToDouble();
      }
      set
      {
        _keyword.GetParameter(12).Value = value;
      }
    }

    public double SpecifiedLength
    {
      get
      {
        return _keyword.GetParameter(14).ToDouble();
      }
      set
      {
        _keyword.GetParameter(14).Value = value;
      }
    }

    public int RMApproachNo
    {
      get
      {
        return _keyword.GetParameter(15).ToInt();
      }
      set
      {
        _keyword.GetParameter(15).Value = value;
      }
    }

    public string MaterialID
    {
      get
      {
        return _keyword.GetParameter(19).ToString();
      }
      set
      {
        _keyword.GetParameter(19).Value = value;
      }
    }

    public double Manning
    {
      get
      {
        return _keyword.GetParameter(22).ToDouble();
      }
      set
      {
        _keyword.GetParameter(22).Value = value;
      }
    }

  }
}
