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

    public CrossSection()
    { }

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
      SetPoints(P1, P2, P1.Chainage, P2.Chainage, Chainage);


    }

    /// <summary>
    /// Method splitted to permit testing without reading DHI CrossSection
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    /// <param name="Chainage"></param>
    private void SetPoints(IXYPoint P1, IXYPoint P2, double Chainage1, double Chainage2, double Chainage)
    {
      double dx = P2.X - P1.X;
      double dy = P2.Y - P1.Y;
      double dchainage = (Chainage - Chainage1)/( Chainage2 - Chainage1);

      MarkerOneLocation = new XYPoint(P1.X + dchainage * dx, P1.Y + dchainage * dy); 

      double lenght = Math.Pow(Math.Pow(dx,2)+ Math.Pow(dy,2),0.5);

      XYPoint UnityVector = new XYPoint(dx / lenght, dy / lenght);


      if (_cs != null)
      {
        Line = new XYPolyline();
        int M1Index = _cs.Points.GetPointAtMarker(1).Index;

        for (int i = M1Index; i < _cs.Points.Count(); i++)
        {
          Line.Points.Add(new XYPoint(MarkerOneLocation.X + UnityVector.X * _cs.Points[i].X, MarkerOneLocation.Y - UnityVector.Y * _cs.Points[i].X));
        }

        for (int i = 0; i < M1Index; i++)
        {
          Line.Points.Add(new XYPoint(MarkerOneLocation.X - UnityVector.X * _cs.Points[i].X, MarkerOneLocation.Y + UnityVector.Y * _cs.Points[i].X));
        }

      }


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
