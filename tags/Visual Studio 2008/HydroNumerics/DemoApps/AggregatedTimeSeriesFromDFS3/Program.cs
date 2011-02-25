using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DHI.TimeSeries;
using MathNet.Numerics.LinearAlgebra;

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;

namespace AggregatedTimeSeriesFromDFS3
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      string DFS3FileName = args.FirstOrDefault(var => Path.GetExtension(var).Equals(".dfs3"));
      var SheFiles = args.Where(var => Path.GetExtension(var).Equals(".she"));

      if (DFS3FileName == null)
      {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Known file types (*.dfs3)|*.dfs3"; //Only open .dfs2-files
        ofd.Title = "Select a .dfs2-file with grid codes marking the areas where timeseries should be aggregated";
        if (ofd.ShowDialog() == DialogResult.OK)
          DFS3FileName = ofd.FileName;
        else
          return;
      }
      if (SheFiles.Count() == 0)
      {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Known file types (*.she)|*.she"; //Only open .asc-files
        ofd.Title = "Select the she-files with the scenarios";
        ofd.Multiselect = true;
        if (ofd.ShowDialog() == DialogResult.OK)
          SheFiles = ofd.FileNames;
        else
          return;
      }

      DFS3 GridFile = new DFS3(DFS3FileName);

      foreach (var file in SheFiles)
      {
        Model M = new Model(file);
        ITSObject timeseries = new TSObjectClass();
        timeseries.Time.AddTimeSteps(M.Results.PhreaticHead.TimeSteps.Count());

        
        for (int t = 0; t < M.Results.PhreaticHead.TimeSteps.Count(); t++)
        {
              for (int k=0;k<GridFile.NumberOfLayers;k++)
              {
        Dictionary<int,List<double>> AggregatedValues = new Dictionary<int,List<double>>();
          for (int i =0;i< GridFile.NumberOfRows;i++)
          {
            for (int j=0;j<GridFile.NumberOfColumns;j++)
            {
              int gridcode = (int)GridFile.GetData(0,1)[i,j,k];
              List<double> Values;
              if(!AggregatedValues.TryGetValue(gridcode, out Values))
              {
                Values = new List<double>();
                AggregatedValues.Add(gridcode,Values);
              }

              Values.Add(M.Results.PhreaticHead.TimeData(t)[i,j,k]);
              }
            }
                foreach(var kvp in AggregatedValues)
                {

                }
          }



          timeseries.Time.SetTimeForTimeStepNr(t,M.Results.PhreaticHead.TimeSteps[t]);
          timeseries.Item(0).SetDataForTimeStepNr(t,

        }
        



      }
    }
  }
}
