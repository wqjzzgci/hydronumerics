using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Mike1D.CrossSections;

using HydroNumerics.Geometry;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class CrossSection
  {
    private DHI.Mike1D.CrossSections.CrossSection _cs;

    public IXYPoint point { get; internal set; }

    internal CrossSection(DHI.Mike1D.CrossSections.CrossSection cs)
    {
      _cs = cs;
    }

    public string BranchName
    {
      get { return _cs.RouteLocation.Branch; }
    }

    public string TopoID
    {
      get { return _cs.RouteLocation.TopoID; }
    }

    public double Chainage
    {
      get { return _cs.RouteLocation.Chainage; }
    }

  }
}
