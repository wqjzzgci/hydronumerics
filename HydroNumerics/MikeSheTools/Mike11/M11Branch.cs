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
    private branch _pfsdata;
    private List<M11Point> _points = new List<M11Point>();
    private List<CrossSection> _crossSections = new List<CrossSection>();

    public BranchID ID { get; internal set; }

    public List<M11Branch> UpstreamBranches = new List<M11Branch>();
    public M11Branch DownStreamBranch { get; set; }

    internal M11Branch(branch BranchFromPFS, SortedDictionary<int, point> Points)
    {
      _pfsdata = BranchFromPFS;

      //Loop the POINTS
      for (int i=0; i<_pfsdata.points.NumberOfParameters;i++)
      {
        M11Point mp =new M11Point(Points[_pfsdata.points.GetValue(i)]);
        _points.Add(mp);
      }
      
      //Sort by chainage
      _points.Sort(new Comparison<M11Point>((var1,var2) => var1.Chainage.CompareTo(var2.Chainage)));

      //Add to polyline
      Line = new XYPolyline();
      foreach (var mp in _points)
        Line.Points.Add(mp);


      ID = new BranchID { Branchname = Name, StartChainage = ChainageStart };
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

      //Find upstream POINTS.

      //The cross section is at the end of the branch
      if (_points.Last().Chainage == cs.Chainage)
        p_upstream = _points.Last();
      else
        p_upstream = _points.FirstOrDefault(var => var.Chainage > cs.Chainage);

      //Downstream point is the previous point
      p_downstream = _points[_points.IndexOf(p_upstream) - 1];

      //Set the POINTS on the cross section
      cs.SetPoints(p_downstream, p_upstream);
    }

    #region Public properties

    /// <summary>
    /// Gets a polyline of this branch
    /// </summary>
    public XYPolyline Line { get; private set; }

    /// <summary>
    /// Gets the POINTS on this branch
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
        return _pfsdata.definitions.Par1;
      }
      set
      {
        _pfsdata.definitions.Par1 = value;
      }
    }

    /// <summary>
    /// Gets and sets the TOPOID of the Branch. This is actually just another name
    /// </summary>
    public string TopoID
    {
      get
      {
        return _pfsdata.definitions.Par2;
      }
      set
      {
        _pfsdata.definitions.Par2 = value;
      }
    }

    /// <summary>
    /// Gets the chainage value at the start
    /// </summary>
    public double ChainageStart
    {
      get
      {
        return _pfsdata.definitions.Par3;
      }
    }

    /// <summary>
    /// Gets the chainage value at the end
    /// </summary>
    public double ChainageEnd
    {
      get
      {
        return _pfsdata.definitions.Par4;
      }
    }

    public bool IsEndPoint
    {
      get
      {
        return string.IsNullOrEmpty(_pfsdata.connections.Par3);
      }
    }


    public BranchID UpstreamConnection
    {
      get
      {
        return new BranchID() { Branchname = _pfsdata.connections.Par1, StartChainage = _pfsdata.connections.Par2 };
      }
    }

    public BranchID DownStreamConnection
    {
      get
      {
        if (IsEndPoint)
          return null;
        return new BranchID() { Branchname = _pfsdata.connections.Par3, StartChainage = _pfsdata.connections.Par4 };
      }
    }


    public double EndPointElevation
    {
      get
      {
        return CrossSections.Last().MaxHeightMrk1and3;
      }
      set
      {
        CrossSections.Last().MaxHeightMrk1and3 = value;
      }
    }


    #endregion



    public double GetBottomLevelAtChainage(double chainage)
    {
      if (chainage <= ChainageStart)
        return CrossSections.First().BottomLevel;
      if (chainage >= ChainageEnd)
        return CrossSections.Last().BottomLevel;

      int counter = 1;
      while (_crossSections[counter].Chainage < chainage)
        counter++;

      _crossSections[counter].Chainage - _crossSections[counter-1].Chainage

    }
    

    public override string ToString()
    {
      return Name + "," + TopoID;
    }

  }
}
