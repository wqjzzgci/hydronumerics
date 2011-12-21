using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Factory;
using Microsoft.Research.Science.Data.Memory;
using Microsoft.Research.Science.Data.Imperative;

namespace Dfs2NetCDF
{
  public static class DFSExtension
  {
    public static void SaveToNetCDF(this DFSBase dfs, string NetCDFFileName)
    {
      float[,] grid = new float[360, 720];
      double[] x = new double[360];
      double[] y = new double[720];

      for (int i = 0; i < 360; i++)
      {
        grid[i, i] = i ^ 2;
        x[i] = i;
        y[i] = i;

        y[i+360] = i+360;
        grid[i, i+360] = i ^ 2;

      }
    
      // ... compute grid, x and y values
      DataSet ds = DataSet.Open(NetCDFFileName + "?openMode=create");
      ds.Add("grid", grid, "x", "y");
      ds.Add("x", x, "x");
      ds.Add("y", y, "y");
      ds.PutAttr("grid", "units", "m/sec2");

      ds.Commit();
      ds.Dispose();
    }

  }
}
