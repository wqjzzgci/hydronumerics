using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;

namespace Dfs3plotdfs0
{
  public class ExtractData
  {
    private Model mShe;

    public ExtractData(string FileName, string Dfs3FileName)
    {
      mShe = new Model(FileName);

      DFS3 dfs = new DFS3(Dfs3FileName);

      List<List<double>> wells = new List<List<double>>();

      foreach (var W in mShe.ExtractionWells)
      {
        wells.Add(new List<double>());
      }

      for (int i = 0; i < dfs.NumberOfTimeSteps; i++ )
      {
        for (int j = 0; j < mShe.ExtractionWells.Count; j++)
        {
          var w = mShe.ExtractionWells[j];

          wells[j].Add(dfs.GetData(i, 1)[w.Row, w.Column, w.Layer]);
        }
      }
    }

  }
}
