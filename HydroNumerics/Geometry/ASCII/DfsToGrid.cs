using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.Geometry.ASCII
{
  public static class DfsToGrid
  {
    public static string GetASCIIGrid(this DFS2 dfsfile, int TimeStep, int Item)
    {
      ASCIIGrid ascii = new ASCIIGrid();
      ascii.NumberOfColumns = dfsfile.NumberOfColumns;
      ascii.NumberOfRows = dfsfile.NumberOfRows;
      ascii.XOrigin = dfsfile.XOrigin;
      ascii.YOrigin = dfsfile.YOrigin;
      ascii.GridSize = dfsfile.GridSize;
      ascii.DeleteValue = dfsfile.DeleteValue;
      ascii.Data = dfsfile.GetData(TimeStep, Item);

      return ascii.ToString();
    }

    public static string GetASCIIGrid(this DFS3 dfsfile, int TimeStep, int Item, int Layer)
    {
      ASCIIGrid ascii = new ASCIIGrid();
      ascii.NumberOfColumns = dfsfile.NumberOfColumns;
      ascii.NumberOfRows = dfsfile.NumberOfRows;
      ascii.XOrigin = dfsfile.XOrigin;
      ascii.YOrigin = dfsfile.YOrigin;
      ascii.GridSize = dfsfile.GridSize;
      ascii.DeleteValue = dfsfile.DeleteValue;
      ascii.Data = dfsfile.GetData(TimeStep, Item)[Layer];

      return ascii.ToString();
    }

  }
}
