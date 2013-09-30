using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using MathNet.Numerics.Interpolation.Algorithms;

using HydroNumerics.Core.WPF;
using HydroNumerics.Geometry;
using HydroNumerics.MikeSheTools.PFS.NWK11;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class M11Branch:BaseViewModel
  {
    private branch _pfsdata;
    private List<M11Point> _points = new List<M11Point>();
    private List<CrossSection> _crossSections = new List<CrossSection>();

    public BranchID ID { get; internal set; }


    private ObservableCollection<CrossSection> selectedCrossSections = new ObservableCollection<CrossSection>();

    /// <summary>
    /// Gets and sets SelectedCrossSections;
    /// </summary>
    public ObservableCollection<CrossSection> SelectedCrossSections
    {
      get { return selectedCrossSections; }
      set
      {
        if (value != selectedCrossSections)
        {
          selectedCrossSections = value;
          NotifyPropertyChanged("SelectedCrossSections");
        }
      }
    }

    

    private ObservableCollection<M11Branch> upstreamBranches = new ObservableCollection<M11Branch>();

    /// <summary>
    /// Gets and sets UpstreamBranches;
    /// </summary>
    public ObservableCollection<M11Branch> UpstreamBranches
    {
      get {
        return upstreamBranches; }
      set
      {
        if (value != upstreamBranches)
        {
          upstreamBranches = value;
          NotifyPropertyChanged("UpstreamBranches");
        }
      }
    }

    public double ChainageOffset { get; set; }

    private M11Branch downstreamBranch;

    /// <summary>
    /// Gets and sets DownstreamBranch;
    /// </summary>
    public M11Branch DownstreamBranch
    {
      get { return downstreamBranch; }
      set
      {
        if (value != downstreamBranch)
        {
          downstreamBranch = value;
          NotifyPropertyChanged("DownstreamBranch");
        }
      }
    }

    
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

      //The cross section is at the end of the branch. Cast to int to avoid rounding problems
      if ((int)_points.Last().Chainage == (int)cs.Chainage)
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




    /// <summary>
    /// Gets and sets EndPointElevation;
    /// </summary>
    public double EndPointElevation
    {
      get { return CrossSections.Last().MaxHeightMrk1and3; }
      set
      {
        if (value != CrossSections.Last().MaxHeightMrk1and3)
        {
          CrossSections.Last().MaxHeightMrk1and3 = value;
          NotifyPropertyChanged("EndPointElevation");
          NotifyPropertyChanged("CrossSections");
          NotifyPropertyChanged("SelectedCrossSections");
        }
      }
    }


    private double chainageOffset = 0;

    /// <summary>
    /// Gets and sets ChainageOffset;
    /// </summary>
    public double ChainageOffset
    {
      get { return chainageOffset; }
      set
      {
        if (value != chainageOffset)
        {
          chainageOffset = value;
          NotifyPropertyChanged("ChainageOffset");
        }
      }
    }


    public double? ConnectionBottomLevelOffset
    {
      get
      {
        if (DownstreamBranch == null)
          return null;
        else
          return DownstreamBranch.GetBottomLevelAtChainage(_pfsdata.connections.Par4) - CrossSections.Last().BottomLevel;
      }
    }

    #endregion


    public XYPoint GetPointAtChainage(double chainage)
    {
      LinearSplineInterpolation lspx = new LinearSplineInterpolation(Points.Select(xc => xc.Chainage).ToList(), Points.Select(xc => xc.X).ToList());
      LinearSplineInterpolation lspy = new LinearSplineInterpolation(Points.Select(xc => xc.Chainage).ToList(), Points.Select(xc => xc.Y).ToList());

      return new XYPoint(lspx.Interpolate(chainage), lspy.Interpolate(chainage));

    }

    /// <summary>
    /// Gets the bottomlevel of the branch. Interpolates linearly between points
    /// </summary>
    /// <param name="chainage"></param>
    /// <returns></returns>
    public double GetBottomLevelAtChainage(double chainage)
    {
      if (chainage <= ChainageStart)
        return CrossSections.First().BottomLevel;
      if (chainage >= ChainageEnd)
        return CrossSections.Last().BottomLevel;

      LinearSplineInterpolation lsp = new LinearSplineInterpolation(CrossSections.Select(xc=>xc.Chainage).ToList(), CrossSections.Select(xc=>xc.BottomLevel).ToList());

      return lsp.Interpolate(chainage);
    }
    

    public override string ToString()
    {
      return Name + "," + TopoID;
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode() ^ ChainageStart.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      M11Branch other = obj as M11Branch;

      if (other == null)
        return false;
      else
        return other.Name == Name & other.ChainageStart == ChainageStart & other.ChainageEnd == ChainageEnd;


    }


  }
}
