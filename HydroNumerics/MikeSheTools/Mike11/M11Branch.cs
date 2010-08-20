using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.MikeSheTools.PFS.NWK11;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class M11Branch
  {
    private Branch _pfsdata;
    private List<M11Point> _points = new List<M11Point>();

    public XYPolyline Line { get; private set; }
    public IEnumerable<M11Point> Points { get { return _points; } }

    private List<CrossSection> _crossSections = new List<CrossSection>();

    public IEnumerable<CrossSection> CrossSections { get { return _crossSections; } }


    internal M11Branch(Branch BranchFromPFS, SortedDictionary<int, Point> Points)
    {
      _pfsdata = BranchFromPFS;
      Line = new XYPolyline();

      foreach (int PointNumber in _pfsdata.PointNumbers)
      {
        M11Point mp =new M11Point(Points[PointNumber]);
        _points.Add(mp);
      }
  //Sort by chainage
      _points.Sort(new Comparison<M11Point>((var1,var2) =>var1.Chainage.CompareTo(var2.Chainage)));

      //Add to polyline
      foreach(var mp in _points)
        Line.Points.Add(mp);
    }

    public void AddCrossection(CrossSection cs)
    {

      _crossSections.Add(cs);

      //ToDO: Calculate where it is to be located. Should be the same way Mike11 does it.
    }

    /// <summary>
    /// Gets and sets the name of the Branch
    /// </summary>
    public string Name
    {
      get
      {
        return _pfsdata.BranchID;
      }
      set
      {
        _pfsdata.BranchID = value;
      }
    }

    /// <summary>
    /// Gets and sets the TOPOID of the Branch. This is actually just another name
    /// </summary>
    public string TopoID
    {
      get
      {
        return _pfsdata.TopoID;
      }
      set
      {
        _pfsdata.TopoID = value;
      }
    }

    public double ChainageStart
    {
      get
      {
        return _pfsdata.UpstreamChainage;
      }
    }

    public double ChainageEnd
    {
      get
      {
        return _pfsdata.DownstreamChainage;
      }
    }


  }
}
