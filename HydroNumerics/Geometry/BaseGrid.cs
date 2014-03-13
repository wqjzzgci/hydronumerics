using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Geometry
{
  public class BaseGrid:IGrid
  {
    public int NumberOfColumns { get; set; }
    public int NumberOfRows { get; set; }
    public double GridSize { get; set; }
    public double XOrigin { get; set; }
    public double YOrigin { get; set; }
    public bool OriginAtCenter { get; set; }
    public double Orientation { get; set; }

    /// <summary>
    /// Gets the Column index for this coordinate. Lower left is (0,0). 
    /// Returns -1 if UTMY is left of the grid and -2 if it is right.
    /// </summary>
    /// <param name="UTMY"></param>
    /// <returns></returns>
    public int GetColumnIndex(double UTMX)
    {
      double d = (UTMX - XOrigin) / GridSize;
      if (Math.IEEERemainder((UTMX - XOrigin), GridSize) == 0.0)
      {
        d--;
        d = Math.Max(0, d);
      }
      //Calculate as a double to prevent overflow errors when casting 
      double ColumnD = Math.Max(-1, Math.Floor(d));

      if (ColumnD > NumberOfColumns - 1)
        return -2;
      return (int)ColumnD;
    }

    /// <summary>
    /// Gets the Row index for this coordinate. Lower left is (0,0). 
    /// Returns -1 if UTMY is below the grid and -2 if it is above.
    /// If a point is exactly between grid blocks the lower is chosen 
    /// </summary>
    /// <param name="UTMY"></param>
    /// <returns></returns>
    public int GetRowIndex(double UTMY)
    {
      double d = (UTMY - YOrigin) / GridSize;
      if (Math.IEEERemainder((UTMY - YOrigin), GridSize) == 0.0)
      {
        d--;
        d = Math.Max(0, d);
      }

      //Calculate as a double to prevent overflow errors when casting 
      double RowD = Math.Max(-1, Math.Floor(d));

      if (RowD > NumberOfRows - 1)
        return -2;
      return (int)RowD;
    }

    /// <summary>
    /// Returns the X-coordinate of the left cell-boundary
    /// </summary>
    /// <param name="Column"></param>
    /// <returns></returns>
    public double GetX(int Column)
    {
      return XOrigin + GridSize * Column;
    }
    /// <summary>
    /// Returns the Y-coordinate of the lower cell-boundary
    /// </summary>
    /// <param name="Row"></param>
    /// <returns></returns>
    public double GetY(int Row)
    {
      return YOrigin + GridSize * Row;
    }
    /// <summary>
    /// Returns the X-coordinate of the cell-center
    /// </summary>
    /// <param name="Column"></param>
    /// <returns></returns>
    public double GetXCenter(int Column)
    {
      return GetX(Column) + 0.5 * GridSize;
    }
    /// <summary>
    /// Returns the Y-coordinate of the cell-center
    /// </summary>
    /// <param name="Row"></param>
    /// <returns></returns>
    public double GetYCenter(int Row)
    {
      return GetY(Row) + 0.5 * GridSize;
    }

    /// <summary>
    /// Gets the indeces of gridblocks where the centers are contained in the polygon
    /// </summary>
    /// <param name="Polygon"></param>
    /// <returns></returns>
    public List<Tuple<int, int>> GetSubSet(IXYPolygon Polygon)
    {
      List<Tuple<int, int>> toreturn = new List<Tuple<int, int>>();

      List<XYPolygon> pols = new List<XYPolygon>();
      if(Polygon is XYPolygon)
        pols.Add((XYPolygon)Polygon);
      else if (Polygon is MultiPartPolygon)
        pols.AddRange( ((MultiPartPolygon)Polygon).Polygons);

      foreach(var pol in pols)
      {
        var bbox = pol.BoundingBox;

        for(int i = GetRowIndex(bbox.Points.Min(p=>p.Y));i<=GetRowIndex(bbox.Points.Max(p=>p.Y));i++)
        {
          for (int j = GetColumnIndex(bbox.Points.Min(p => p.X)); j <= GetColumnIndex(bbox.Points.Max(p => p.X)); j++)
          {
            if (pol.Contains(GetXCenter(j), GetYCenter(i)))
              toreturn.Add(new Tuple<int, int>(j, i));
          }
        }
      }
      return toreturn;
    }


  }
}
