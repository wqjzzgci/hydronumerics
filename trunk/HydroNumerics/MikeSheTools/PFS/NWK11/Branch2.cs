using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class Branch2:PFSMapper 
  {

    public List<ComputationalPoint> ComputationalPoints { get; private set; }


    internal Branch2(PFSSection Keyword)
    {
      _pfsHandle = Keyword;

      PFSSection _points = _pfsHandle.GetSection("points", 1);
      ComputationalPoints = new List<ComputationalPoint>(); 

      for (int i =1;i<=_points.GetKeywordsNo();i++)
        ComputationalPoints.Add(new ComputationalPoint(_points.GetKeyword(i) ));
    }

    public string BranchID
    {
      get { return _pfsHandle.GetKeyword(1).GetParameter(1).ToString(); }
      set { _pfsHandle.GetKeyword(1).GetParameter(1).Value = value; }
    }


  }
}
