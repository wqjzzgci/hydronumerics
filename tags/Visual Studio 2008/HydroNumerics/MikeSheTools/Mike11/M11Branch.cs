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
    private List<CrossSection> _crossSections = new List<CrossSection>();



    internal M11Branch(Branch BranchFromPFS, SortedDictionary<int, Point> Points)
    {
      _pfsdata = BranchFromPFS;

      //Loop the points
      foreach (int PointNumber in _pfsdata.PointNumbers)
      {
        M11Point mp =new M11Point(Points[PointNumber]);
        _points.Add(mp);
      }
      
      //Sort by chainage
      _points.Sort(new Comparison<M11Point>((var1,var2) => var1.Chainage.CompareTo(var2.Chainage)));

      //Add to polyline
      Line = new XYPolyline();
      foreach (var mp in _points)
        Line.Points.Add(mp);
    }

    /// <summary>
    /// Adds a cross section to this branch and gives it the right position
    /// </summary>
    /// <param name="cs"></param>
    public void AddCrossection(CrossSection cs)
    {
      _crossSections.Add(cs);
     
      M11Point p_upstream;
      M11Point p_downstream;

      //Find upstream points.

      //The cross section is at the end of the branch
      if (_points.Last().Chainage == cs.Chainage)
        p_upstream = _points.Last();
      else
        p_upstream = _points.FirstOrDefault(var => var.Chainage > cs.Chainage);

      //Downstream point is the previous point
      p_downstream = _points[_points.IndexOf(p_upstream) - 1];

      //Set the points on the cross section
      cs.SetPoints(p_downstream, p_upstream);
    }

    #region Public properties

    /// <summary>
    /// Gets a polyline of this branch
    /// </summary>
    public XYPolyline Line { get; private set; }

    /// <summary>
    /// Gets the points on this branch
    /// </summary>
    public IEnumerable<M11Point> Points { get { return _points; } }


    /// <summary>
    /// Gets the cross sections on this branch
    /// </summary>
    public IEnumerable<CrossSection> CrossSections { get { return _crossSections; } }

    
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

    /// <summary>
    /// Gets the chainage value at the start
    /// </summary>
    public double ChainageStart
    {
      get
      {
        return _pfsdata.UpstreamChainage;
      }
    }

    /// <summary>
    /// Gets the chainage value at the end
    /// </summary>
    public double ChainageEnd
    {
      get
      {
        return _pfsdata.DownstreamChainage;
      }
    }
    #endregion

    public override string ToString()
    {
      return Name + "," + TopoID;
    }

  }
}
