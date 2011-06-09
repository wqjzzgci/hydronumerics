using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class MIKESHECOUPLING
  {
    private PFSKeyword _keyword;

    internal MIKESHECOUPLING(PFSKeyword keyword)
    {
      _keyword = keyword;
    }

    public string BranchName
    {
      get
      {
        return _keyword.GetParameter(1).ToString();
      }
    }

    public double UpStreamChainage
    {
      get
      {
        return _keyword.GetParameter(2).ToDouble();
      }
    }

    public double DownStreamChainage
    {
      get
      {
        return _keyword.GetParameter(3).ToDouble();
      }
    }

    public int Conductance
    {
      get
      {
        return _keyword.GetParameter(4).ToInt();
      }
    }

    public double LeakageCoefficient
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
