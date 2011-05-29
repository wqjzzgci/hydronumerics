using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HydroNumerics.MikeSheTools.Core
{
  public class ModelsController
  {

    private List<Model> models = new List<Model>();

    int numberOfParallelsimulations = 4;

    public void AddModel(string FileName)
    {
      models.Add(new Model(FileName));
    }

    public void RunParameterSets(List<SortedList<int, double>> ParameterValues)
    {

      var t = TaskScheduler.FromCurrentSynchronizationContext();
      TaskFactory ts = new TaskFactory();
      

      var task1 = new Task(() => models.First().Run());
      task1.Start();


    }
  }
}
