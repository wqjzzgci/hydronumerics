using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra.Double;
using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.Nitrate.Model
{
  public class UZAgeDefinition
  {
    public DFS2 dfs { get; set; }
    public DenseMatrix Data { get; set; }
    public double MinHorizontalTravelDistance { get; set; }
    public double MinTravelTime { get; set; }
    
    public SafeFile FileName { get; set; }

    public void Initialize()
    {
      dfs = new DFS2(FileName.FileName);
      int itemnumber = 1;
      if (FileName.ColumnNames.Count > 0)
      {
        var dfsitem = dfs.Items.FirstOrDefault(I => I.Name == FileName.ColumnNames[0]);
        if (dfsitem != null)
          itemnumber = dfsitem.ItemNumber;
      }
      Data= dfs.GetData(0, itemnumber);
    }
  }
}
