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

    public IXYPoint MarkerOneLocation { get; internal set; }

    internal CrossSection(DHI.Mike1D.CrossSections.CrossSection cs)
    {
      _cs = cs;
      Line = new XYPolyline();
    }

    public IEnumerable<double> XValues
    {
      get
      {
        int np = _cs.Points.PointCount;
        for (int i = 0; i < np; i++)
          yield return _cs.Points[i].X;
      }
    }

    /// <summary>
    /// Set the two points that defines the line where the CrossSection is located
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    public void SetPoints(M11Point P1, M11Point P2)
    {
      double dx = P2.X -
    }

    /// <summary>
    /// Gets and sets the height at marker 1. Adjusts the datum.
    /// </summary>
    public double HeigthAtMarker1
    {
      get
      {
        return _cs.Points.GetPointAtMarker(1).Z + _cs.Datum;
      }
      set
      {

        _cs.AdjustDatumTo(value - _cs.Points.GetPointAtMarker(1).Z); 
      }
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
