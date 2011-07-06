using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.Core
{
  public class OutputGenerator
  {

    public static void KSTResults(Model mshe)
    {
      double[] percentiles = new double[] { 0.1, 0.5, 0.9 };

      if (File.Exists(mshe.Files.SZ3DFileName))
      {

        var dfs = DfsFileFactory.OpenFile(mshe.Files.SZ3DFileName);
        string filename = Path.Combine(mshe.Files.ResultsDirectory, "SZ3D_percentiles.dfs3");
        var dfsout = DfsFileFactory.CreateFile(filename, percentiles.Count());
        dfsout.CopyFromTemplate(dfs);
        dfs.Percentile(1, dfsout, percentiles, 300);
        dfsout.Dispose();

        for (int i = 1; i <= 12; i++)
        {
          //Get the timesteps
          List<int> timesteps = new List<int>();

          for (int j = 0; j < dfs.TimeSteps.Count; j++)
            if (dfs.TimeSteps[j].Month == i)
              timesteps.Add(j);

          if (timesteps.Count > 3)
          {
            string filename2 = Path.Combine(mshe.Files.ResultsDirectory, "SZ3D_percentiles_Month_" + i + ".dfs3");
            var dfsoutm = DfsFileFactory.CreateFile(filename2, percentiles.Count());
            dfsoutm.CopyFromTemplate(dfs);
            dfs.Percentile(1, timesteps.ToArray(), dfsoutm, percentiles, 300);
            dfsoutm.Dispose();
          }
        }
        dfs.Dispose();
      }

      if (File.Exists(mshe.Files.SZ2DFileName))
      {
        var dfs = DfsFileFactory.OpenFile(mshe.Files.SZ2DFileName);
        var filename = Path.Combine(mshe.Files.ResultsDirectory, "Phreatic_percentiles.dfs2");
        var dfsout = DfsFileFactory.CreateFile(filename, percentiles.Count());
        dfsout.CopyFromTemplate(dfs);
        dfs.Percentile(1, dfsout, percentiles, 300);
        dfsout.Dispose();

        for (int i = 1; i <= 12; i++)
        {
          //Get the timesteps
          List<int> timesteps = new List<int>();

          for (int j = 0; j < dfs.TimeSteps.Count; j++)
            if (dfs.TimeSteps[j].Month == i)
              timesteps.Add(j);

          if (timesteps.Count > 3)
          {
            string filename2 = Path.Combine(mshe.Files.ResultsDirectory, "Phreatic_percentiles_Month_" + i + ".dfs2");
            var dfsoutm = DfsFileFactory.CreateFile(filename2, percentiles.Count());
            dfsoutm.CopyFromTemplate(dfs);
            dfs.Percentile(1, timesteps.ToArray(), dfsoutm, percentiles, 300);
            dfsoutm.Dispose();
          }
        }
        dfs.Dispose();
      }
    }
  }
}
