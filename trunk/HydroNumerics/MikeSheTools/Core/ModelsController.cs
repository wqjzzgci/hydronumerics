using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.Core
{
  public class ModelsController
  {

    public List<Model> models = new List<Model>();
    int[] TimeSteps;

    public void AddModel(string FileName)
    {
      models.Add(new Model(FileName));
    }

    public ModelsController()
    {
      ParameterSets.Push(2);
      ParameterSets.Push(2);
      ParameterSets.Push(2);
      ParameterSets.Push(2);
    }

    Stack<int> ParameterSets = new Stack<int>();

    private void RunNext(Model mshe)
    {
      if (ParameterSets.Count > 0)
      {
        ParameterSets.Pop();
        mshe.Run(true, true);
      }
    }

    public void RunParameterSets(List<SortedList<int, double>> ParameterValues)
    {
      foreach (var v in models)
      {
        v.SimulationFinished += new EventHandler(v_SimulationFinished);
        RunNext(v);
      }
      int k = 0;
    }

    void v_SimulationFinished(object sender, EventArgs e)
    {
      Model mshe = sender as Model;
      var dfs = DfsFileFactory.OpenFile(mshe.Files.SZ3DFileName);
      double[] percentiles = new double[]{0.1};
      string filename =Path.Combine(mshe.Files.ResultsDirectory, "SZ3D_percentiles.dfs3");
      var dfsout = DfsFileFactory.CreateFile(filename, percentiles.Count());
      dfsout.CopyFromTemplate(dfs);
      dfs.Percentile(1, dfsout, percentiles, 80000000);
      dfsout.Dispose();
      DFS3 dfsout2 = new DFS3(filename);
      Console.WriteLine(dfsout2.GetData(0, 1)[10, 10, 0]);
      dfsout2.Dispose();

      RunNext(mshe);

    }
  }
}
