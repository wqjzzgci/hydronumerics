using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

using MathNet.Numerics.Interpolation.Algorithms;

using DHI.Mike1D.CrossSections;

using HydroNumerics.Geometry;
using HydroNumerics.Core.WPF;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class CrossSection:BaseViewModel
  {
    private DHI.Mike1D.CrossSections.CrossSection _cs;
    public XYPolyline Line { get; private set; }
    XYPoint UnityVector;

    public IXYPoint MidStreamLocation { get; internal set; }

    public CrossSection()
    { }

    internal CrossSection(DHI.Mike1D.CrossSections.CrossSection cs)
    {
      _cs = cs;
      Line = new XYPolyline();
    }


    
    

    /// <summary>
    /// Set the two POINTS that defines the line where the CrossSection is located
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    internal void SetPoints(M11Point P1, M11Point P2)
    {
      SetPoints(P1, P2, P1.Chainage, P2.Chainage, Chainage);
    }

    /// <summary>
    /// Method splitted to permit testing without reading DHI CrossSection
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    /// <param name="Chainage"></param>
    public void SetPoints(IXYPoint P1, IXYPoint P2, double Chainage1, double Chainage2, double Chainage)
    {
      double dx = P2.X - P1.X;
      double dy = P2.Y - P1.Y;
      double dchainage = (Chainage - Chainage1)/( Chainage2 - Chainage1);

      MidStreamLocation = new XYPoint(P1.X + dchainage * dx, P1.Y + dchainage * dy); 

      double lenght = Math.Pow(Math.Pow(dx,2)+ Math.Pow(dy,2),0.5);

      UnityVector = new XYPoint(dx / lenght, dy / lenght);

      //Now build the line where the cross section is located.
      if (_cs != null)
      {
        Line = new XYPolyline();
        //MidPoint is set to where Marker 2 is placed
        double xOffset = _cs.Points.GetPointAtMarker(2).X;

        for (int i = 0; i < _cs.Points.Count(); i++)
        {
          Line.Points.Add(new XYPoint(MidStreamLocation.X - UnityVector.Y * (_cs.Points[i].X-xOffset), MidStreamLocation.Y + UnityVector.X * (_cs.Points[i].X-xOffset)));
        }
      }
    }




    private ObservableCollection<XYPoint> xzPoints;

    /// <summary>
    /// Gets and sets XZPoints;
    /// </summary>
    public ObservableCollection<XYPoint> XZPoints
    {
      get {
        if (xzPoints == null)
        {
          xzPoints = new ObservableCollection<XYPoint>();
          double xOffset = _cs.Points.GetPointAtMarker(2).X;
          for (int i = 0; i < _cs.Points.Count(); i++)
            xzPoints.Add(new XYPoint(_cs.Points[i].X -xOffset, _cs.Points[i].Z + _cs.Datum));
        }
        return xzPoints; }
      set
      {
        if (value != xzPoints)
        {
          xzPoints = value;
          NotifyPropertyChanged("XZPoints");
        }
      }
    }



    private ObservableCollection<XYPoint> xz2Points;

    /// <summary>
    /// Gets and sets XZPoints;
    /// </summary>
    public ObservableCollection<XYPoint> XZ2Points
    {
      get
      {
        if (xz2Points == null)
        {
          xz2Points = new ObservableCollection<XYPoint>();
            xz2Points.Add(new XYPoint(Chainage, BottomLevel));
            xz2Points.Add(new XYPoint(Chainage, MaxHeightMrk1and3));
        }
        return xz2Points;
      }
      set
      {
        if (value != xz2Points)
        {
          xz2Points = value;
          NotifyPropertyChanged("XZ2Points");
        }
      }
    }

   

    /// <summary>
    /// Returns a list of 3d POINTS. Uses cubic spline to interpolate.
    /// </summary>
    /// <param name="NumberOfPoints"></param>
    /// <returns></returns>
    public List<Point3D> Get3DPoints(int NumberOfPoints)
    {
              List<Point3D> interpolatedpoints = new List<Point3D>();
        CubicSplineInterpolation spline = new CubicSplineInterpolation();
      List<double> xcoors = new List<double>();
      List<double> zcoors = new List<double>();

      for (int i =0; i<_cs.Points.Count();i++ )
      {
       xcoors.Add(_cs.Points[i].X);
        zcoors.Add(_cs.Points[i].Z);
      }

        spline.Initialize(xcoors, zcoors);
        double xOffset = _cs.Points.GetPointAtMarker(2).X;

      double dx = (xcoors.Last() - xcoors.First())/NumberOfPoints;

        for (int i = 0; i < NumberOfPoints; i++)
        {
          double x = xcoors.First()+i*dx;
          double z = spline.Interpolate(x);

          interpolatedpoints.Add(new Point3D(MidStreamLocation.X - UnityVector.Y * (x - xOffset), MidStreamLocation.Y + UnityVector.X * (x - xOffset), z));

        }
        return interpolatedpoints;
    }


    /// <summary>
    /// Gets and sets the level at Marker 2. Sets by adjusting the datum
    /// </summary>
    public double BottomLevel
    {
      get
      {
        return _cs.Points.GetPointAtMarker(2).Z + _cs.Datum;
      }
      set
      {
        _cs.Datum = value - _cs.Points.GetPointAtMarker(2).Z;
        NotifyPropertyChanged("BottomLevel");
        NotifyPropertyChanged("MaxHeightMrk1and3");
        NotifyPropertyChanged("HeightDifference");
        XZPoints = null;
        XZ2Points = null;
      }
    }

    /// <summary>
    /// Gets and sets the height at the maximum of Marker 1 and 3;
    /// </summary>
    public double MaxHeightMrk1and3
    {
      get
      {
        return Math.Max(_cs.Points.GetPointAtMarker(1).Z, _cs.Points.GetPointAtMarker(3).Z) + _cs.Datum;
      }
      set
      {
        _cs.Datum = (value - Math.Max(_cs.Points.GetPointAtMarker(1).Z, _cs.Points.GetPointAtMarker(3).Z));
        NotifyPropertyChanged("BottomLevel");
        NotifyPropertyChanged("MaxHeightMrk1and3");
        NotifyPropertyChanged("HeightDifference");
        XZPoints = null;
        XZ2Points = null;
      }
    }

    /// <summary>
    /// Gets the number of POINTS in the Cross keyword
    /// </summary>
    public int NumberOfPoints
    {
      get
      {
        return _cs.Points.Count();
      }
    }

    /// <summary>
    /// Gets and sets a DEM height. This is just an attached property that is not really related to the Cross keyword
    /// </summary>

    private double? demHeight;

    /// <summary>
    /// Gets and sets DEMHeight;
    /// </summary>
    public double? DEMHeight
    {
      get { return demHeight; }
      set
      {
        if (value != demHeight)
        {
          demHeight = value;
          NotifyPropertyChanged("DEMHeight");
        }
      }
    }

    


    /// <summary>
    /// Returns the width as the distance between Marker 1 and 3
    /// </summary>
    public double Width
    {
      get
      {
        return Math.Abs(_cs.Points.GetPointAtMarker(1).X - _cs.Points.GetPointAtMarker(3).X);
      }
    }



    /// <summary>
    /// Gets the height difference between the heoght at mid stream and the dem height.
    /// </summary>
    public double? HeightDifference {
      get
      {
        if (DEMHeight.HasValue)
          return Math.Abs(DEMHeight.Value - MaxHeightMrk1and3);
        else
          return null;
      }
    }
    /// <summary>
    /// Gets the branch name
    /// </summary>
    public string BranchName
    {
      get { return _cs.RouteLocation.Branch; }
    }

    /// <summary>
    /// Gets the topoID
    /// </summary>
    public string TopoID
    {
      get { return _cs.RouteLocation.TopoID; }
    }

    /// <summary>
    /// Gets the Chainage
    /// </summary>
    public double Chainage
    {
      get { return _cs.RouteLocation.Chainage; }
    }

  }
}
