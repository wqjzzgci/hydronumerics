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

    public static string KSTResults(Model mshe)
    {

      string returnstring="";
      double[] percentiles = new double[] { 0.1, 0.5, 0.9, 0.95 };

      if (!File.Exists(Path.GetFullPath(mshe.Files.SZ3DFileName)))
      {
        returnstring += "Could not find:" + Path.GetFullPath(mshe.Files.SZ3DFileName); 
      }
      else
      {
        var dfs = DfsFileFactory.OpenFile(mshe.Files.SZ3DFileName);

        //Find first time step to use by skipping the firstyear
        int firsttimestep = 0;

        while (dfs.TimeOfFirstTimestep.Year+5 == dfs.TimeSteps[firsttimestep].Year)
          firsttimestep++;

        List<int> timesteps= new List<int>();
        for (int j = firsttimestep; j < dfs.NumberOfTimeSteps; j++)
          timesteps.Add(j);


        string filename = Path.Combine(mshe.Files.ResultsDirectory, "SZ3D_percentiles.dfs3");
        var dfsout = DfsFileFactory.CreateFile(filename, percentiles.Count());
        dfsout.CopyFromTemplate(dfs);
        dfs.Percentile(1, timesteps.ToArray(), dfsout, percentiles, 300);
        dfsout.Dispose();

        for (int i = 1; i <= 12; i++)
        {
          //Get the timesteps
          timesteps.Clear();

          for (int j = firsttimestep; j < dfs.TimeSteps.Count; j++)
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

      if (!File.Exists(Path.GetFullPath(mshe.Files.SZ2DFileName)))
      {
        returnstring += "\n Could not find:" + Path.GetFullPath(mshe.Files.SZ2DFileName); 
      }
      else
      {
        var dfs = DfsFileFactory.OpenFile(mshe.Files.SZ2DFileName);

        //Find first time step to use by skipping the firstyear
        int firsttimestep = 0;

        while (dfs.TimeOfFirstTimestep.Year + 5 == dfs.TimeSteps[firsttimestep].Year)
          firsttimestep++;

        List<int> timesteps = new List<int>();
        for (int j = firsttimestep; j < dfs.NumberOfTimeSteps; j++)
          timesteps.Add(j);


        var filename = Path.Combine(mshe.Files.ResultsDirectory, "Phreatic_percentiles.dfs2");
        var dfsout = DfsFileFactory.CreateFile(filename, percentiles.Count());
        dfsout.CopyFromTemplate(dfs);
        dfs.Percentile(1, timesteps.ToArray(), dfsout, percentiles, 300);
        dfsout.Dispose();

        for (int i = 1; i <= 12; i++)
        {
          //Get the timesteps
          timesteps.Clear();

          for (int j = firsttimestep; j < dfs.TimeSteps.Count; j++)
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
      return returnstring;
    }
  }
}
