using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class SoilCodesGrid
  {
    public IntegerGrid GlobalGrid = new IntegerGrid();

    public void BuildGrid(string ShapeSoilCodes)
    {
      using (ShapeReader sr = new ShapeReader(ShapeSoilCodes))
      {

        var Allpoints = sr.GeoData.ToList();

        var bbox = XYGeometryTools.BoundingBox(Allpoints.Select(f => (IXYPoint)f.Geometry));

        GlobalGrid.GridSize = 500.0;
        GlobalGrid.XOrigin = bbox.Points.Min(p => p.X);
        GlobalGrid.YOrigin = bbox.Points.Min(p => p.Y);
        GlobalGrid.NumberOfColumns = (int) (Math.Round((bbox.Points.Max(p => p.X) - GlobalGrid.XOrigin)/ GlobalGrid.GridSize,0))+1;
        GlobalGrid.NumberOfRows = (int)(Math.Round((bbox.Points.Max(p => p.Y) - GlobalGrid.YOrigin) / GlobalGrid.GridSize,0)+1);

        foreach (var g in Allpoints)
        {
          GlobalGrid.Data[GlobalGrid.GetColumnIndex(((IXYPoint)g.Geometry).X), GlobalGrid.GetRowIndex(((IXYPoint)g.Geometry).Y)] = int.Parse(g.Data["GRIDID"].ToString());
        }
      }
    }

        /// <summary>
    /// Gets the id for a point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public int GetID(double X, double Y)
    {
      return GlobalGrid.Data[GlobalGrid.GetColumnIndex(X), GlobalGrid.GetRowIndex(Y)];
    }

    /// <summary>
    /// Gets the id for a point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public int GetID(IXYPoint point)
    {
      return GetID(point.X,point.Y);
    }


  }
}
