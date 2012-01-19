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
      double[,,] grid = new double[360, 720,10];
      double[] x = new double[360];
      double[] y = new double[720];
      double[] t = new double[10];

      for (int k = 0; k < 10; k++)
      {
        for (int i = 0; i < 360; i++)
        {
          x[i] = i;
          for (int j = 0; j < 720; j++)
          {
            y[j] = j;
            grid[i, j,k] = i ^ 2 + j ^ 2;
          }
        }
      }
      // ... compute grid, x and y values
      DataSet ds = DataSet.Open(NetCDFFileName + "?openMode=create");

     // MemoryDataSet ds = new MemoryDataSet();

      

      int vid = ds.AddVariable<Int16>("values","x","y","t").ID;



      //ds.Append("values", grid, 2);
      //ds.IsAutocommitEnabled = false;
      //for (int i = 0; i < 10; i++)
      //{
//        ds.Append("values", grid, 2);
      //  ds.Append("t", i*1.1,0);
      //}



     

      ds.Commit();

      //ds.Clone(NetCDFFileName + "?openMode=create").Dispose();

      ds.Dispose();
    }

  }
}
