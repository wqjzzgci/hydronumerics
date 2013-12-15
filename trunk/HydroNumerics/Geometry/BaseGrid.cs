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
      //Calculate as a double to prevent overflow errors when casting 
      double ColumnD = Math.Max(-1, Math.Floor((UTMX - (XOrigin - GridSize / 2)) / GridSize));

      if (ColumnD >= NumberOfColumns)
        return -2;
      return (int)ColumnD;
    }

    /// <summary>
    /// Gets the Row index for this coordinate. Lower left is (0,0). 
    /// Returns -1 if UTMY is below the grid and -2 if it is above.
    /// </summary>
    /// <param name="UTMY"></param>
    /// <returns></returns>
    public int GetRowIndex(double UTMY)
    {
      //Calculate as a double to prevent overflow errors when casting 
      double RowD = Math.Max(-1, Math.Floor((UTMY - (YOrigin - GridSize / 2)) / GridSize));

      if (RowD >= NumberOfRows)
        return -2;
      return (int)RowD;
    }


  }
}
