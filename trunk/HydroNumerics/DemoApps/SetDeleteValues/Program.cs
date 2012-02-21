using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;


namespace SetDeleteValues
{
  class Program
  {
    static void Main(string[] args)
    {

      DFS2 df = new DFS2(args[0]);

            DFS2 dfnew = new DFS2(Path.ChangeExtension(args[0],"")+"_deletes.dfs2", df);


            double threshold=1;

      if (args.Count()>1)
            double.TryParse(args[1], out threshold);

      for (int i = 0; i < df.TimeSteps.Count; i++)
      {
        for (int j = 1; j <= df.Items.Count(); j++)
        {
          var M = df.GetData(i, j);

          for (int m = 0; m < df.NumberOfColumns; m++)
            for (int n = 0; n < df.NumberOfRows; n++)
              if (M[n, m] < threshold)
                M[n, m] = df.DeleteValue;

          dfnew.SetData(i, j, M);

        }
      }

      dfnew.Dispose();
      df.Dispose();
    }
  }
}
