using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;

using MathNet.Numerics.LinearAlgebra.Double;


namespace PhreaticCalculator
{
  class Program
  {
    static void Main(string[] args)
    {
      Model M = new Model(args[0]);

      List<double> MingroundWaterLevels = new List<double>();
      for (int i = 1; i < args.Count(); i++)
        MingroundWaterLevels.Add(double.Parse(args[i]));

      DFS2 CustomPhreaticHead = new DFS2(Path.Combine(M.Files.ResultsDirectory, "CustomPhreaticHeads.dfs2"), MingroundWaterLevels.Count);
      CustomPhreaticHead.NumberOfRows = M.GridInfo.NumberOfRows;
      CustomPhreaticHead.NumberOfColumns = M.GridInfo.NumberOfColumns;
      CustomPhreaticHead.XOrigin = M.GridInfo.XOrigin;
      CustomPhreaticHead.YOrigin = M.GridInfo.YOrigin;
      CustomPhreaticHead.GridSize = M.GridInfo.GridSize;
      CustomPhreaticHead.TimeOfFirstTimestep = M.Results.Heads.TimeSteps.First();
      CustomPhreaticHead.TimeStep = M.Results.Heads.TimeSteps[1].Subtract(M.Results.Heads.TimeSteps[0]);
      CustomPhreaticHead.DeleteValue = M.Results.DeleteValue;


      for (int l = 0; l < MingroundWaterLevels.Count; l++)
      {
        CustomPhreaticHead.Items[l].Name = MingroundWaterLevels[l].ToString();
        CustomPhreaticHead.Items[l].EumItem = DHI.Generic.MikeZero.eumItem.eumIHeadElevation;
        CustomPhreaticHead.Items[l].EumUnit = DHI.Generic.MikeZero.eumUnit.eumUmeter;
      }

      var LowerLevel = M.GridInfo.LowerLevelOfComputationalLayers.Data;

      //Time loop
      for (int t = 0; t < M.Results.Heads.TimeSteps.Count; t++)
      {
        var heads = M.Results.Heads.TimeData(t);

        for (int l = 0; l < MingroundWaterLevels.Count; l++)
        {
          DenseMatrix phreatichead = (DenseMatrix) heads[0].Clone();

          for (int i = 0; i < CustomPhreaticHead.NumberOfRows; i++)
            for (int j = 0; j < CustomPhreaticHead.NumberOfColumns; j++)
            {
              if (heads[0][i, j] != CustomPhreaticHead.DeleteValue)
              {
                for (int k = M.GridInfo.NumberOfLayers - 1; k >= 0; k--)
                {
                  if (heads[k][i, j] >= LowerLevel[k][i, j] + MingroundWaterLevels[l])
                  {
                    phreatichead[i, j] = heads[k][i, j];
                    break;
                  }
                }
              }
            }

          CustomPhreaticHead.SetData(t, l + 1, phreatichead);
        }
      }
      CustomPhreaticHead.Dispose();
      M.Dispose();
    }
  }
}
