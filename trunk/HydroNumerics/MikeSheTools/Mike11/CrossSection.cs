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
    public XYPolyline Line { get; private set; }

    public IXYPoint MidStreamLocation { get; internal set; }

    public CrossSection()
    { }

    internal CrossSection(DHI.Mike1D.CrossSections.CrossSection cs)
    {
      _cs = cs;
      Line = new XYPolyline();
    }


    /// <summary>
    /// Set the two points that defines the line where the CrossSection is located
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    public void SetPoints(M11Point P1, M11Point P2)
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

      XYPoint UnityVector = new XYPoint(dx / lenght, dy / lenght);

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
      }
    }

    /// <summary>
    /// Gets the number of points in the Cross Section
    /// </summary>
    public int NumberOfPoints
    {
      get
      {
        return _cs.Points.Count();
      }
    }

    /// <summary>
    /// Gets and sets a DEM height. This is just an attached property that is not really related to the Cross Section
    /// </summary>
    public double? DEMHeight { get; set; }


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
