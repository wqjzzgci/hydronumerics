using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra.Double;

using HydroNumerics.MikeSheTools.DFS;

namespace WaterOnTerrain
{
  class Program
  {
    static void Main(string[] args)
    {

      string dataFile = args[0];
      string gridFile = args[1];

      string outfilename = Path.Combine(Path.GetDirectoryName(dataFile),"WaterOnTerrain.txt");
      string dfsoutname = Path.Combine(Path.GetDirectoryName(dataFile),"WaterOnTerrain.dfs2");

      double LowerLevel = 0;
      if(args.Count()>2)
        double.TryParse(args[2], out LowerLevel);

      DFS2 Data = new DFS2(dataFile);
      DFS2 Grid = new DFS2(gridFile);

      DFS2 dfsout = new DFS2(dfsoutname,Grid);
      DenseMatrix dmout = new DenseMatrix(Grid.NumberOfRows, Grid.NumberOfColumns);

      //Read the grid and fill into a matrix
      DenseMatrix dmg = Grid.GetData(0, 1);
      Dictionary<double, int> GridAreas = new Dictionary<double, int>();


      for (int i = 0; i < dmg.RowCount; i++)
        for (int j = 0; j < dmg.ColumnCount; j++)
        {
          if(!GridAreas.ContainsKey(dmg[i,j]))
            GridAreas.Add(dmg[i,j],0);
        }
      List<double> keys = new List<double>(GridAreas.Keys);


      using (StreamWriter sw = new StreamWriter(outfilename))
      {
        string Line="Time";
        //Build header
        foreach(var key in GridAreas.Keys)
        {
          Line+="\tGridCode\tNumberOfCells\tArea";
        }
        sw.WriteLine(Line);

        //Time loop
        for (int t = 0; t < Data.NumberOfTimeSteps; t++)
        {
          Line = Data.TimeSteps[t].ToString("dd-MM-yyyy HH:mm:ss");
          DenseMatrix dmd = Data.GetData(t, 1);

          for (int k = 0; k < dmd.Data.Count(); k++)
          {
            if (dmd.Data[k] > LowerLevel)
            {
              dmout.Data[k] = dmout.Data[k] + 1;
              GridAreas[dmg.Data[k]]++;
            }
          }


          //Build line
          foreach (var kvp in GridAreas)
          {
            Line += "\t" + (int)kvp.Key + "\t" + kvp.Value + "\t" + kvp.Value * Math.Pow((float)Data.GridSize, 2);
          }
          sw.WriteLine(Line);

          //Reset
          foreach(var k in keys)
            GridAreas[k] = 0;
        }

        dfsout.SetData(0, 1, dmout);
      }

      Data.Dispose();
      Grid.Dispose();
      dfsout.Dispose();
    }
  }
}
