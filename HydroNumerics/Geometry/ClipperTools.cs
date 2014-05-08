using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;




namespace HydroNumerics.Geometry
{
  public class ClipperTools
  {

    public static XYPolygon GetIntersection(XYPolygon pol1, XYPolygon pol2)
    {



      List<List<IntPoint>> subj = new List<List<IntPoint>>();
  subj.Add(new List<IntPoint>(pol1.Points.Count));
      foreach(var p in pol1.Points)
        subj[0].Add(new IntPoint(p.X, p.Y));


      List<List<IntPoint>> clip = new List<List<IntPoint>>();
      clip.Add(new List<IntPoint>(pol2.Points.Count));
  foreach (var p in pol2.Points)
    clip[0].Add(new IntPoint(p.X, p.Y));


  List<List<IntPoint>> solution = new List<List<IntPoint>>();

	Clipper c = new Clipper();
	c.AddPaths(subj, PolyType.ptSubject, true);
	c.AddPaths(clip, PolyType.ptClip, true);
	c.Execute(ClipType.ctIntersection, solution, 
	  PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

  XYPolygon ToReturn = new XYPolygon();
  if (solution.Count > 0) 
  foreach (var p in solution[0])
    ToReturn.Points.Add(new XYPoint(p.X,p.Y));

          return ToReturn;
    }

  }
}
