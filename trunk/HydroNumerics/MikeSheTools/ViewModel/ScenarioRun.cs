using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.MikeSheTools.Core;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScenarioRun : NotifyPropertyChangedBase
  {
    public SortedList<CalibrationParameter, double?> ParamValues { get; set; }

    private bool isRunning = false;
    public bool IsRunning
    {
      get
      {
        return isRunning;
      }
      set
      {
        isRunning = value;
        NotifyPropertyChanged("IsRunning");
      }
    }

    private bool runThis = true;
    public bool RunThis
    {
      get
      {
        return runThis;
      }
      set
      {
        runThis = value;
        NotifyPropertyChanged("RunThis");
      }
    }


    public int Number { get; set; }
    public string OutputDirectory;
    public event EventHandler ScenarioFinished;

    public void Run(IScenarioModel model)
    {
      foreach (var v in ParamValues)
        model.Parameters.Single(var => var.ShortName == v.Key.ShortName).CurrentValue = v.Value.Value;

      model.SimulationFinished += new EventHandler(model_SimulationFinished);

      IsRunning = true;
      model.BeginRun();
    }

    void model_SimulationFinished(object sender, EventArgs e)
    {
      var d = Directory.CreateDirectory(OutputDirectory +"_"+ Number.ToString());

      using (StreamWriter sw = new StreamWriter(Path.Combine(d.FullName, "logfile.txt")))
      {
        sw.Write((sender as IScenarioModel).Status);
      }

      foreach (var file in (sender as IScenarioModel).ResultFileNames)
      {
        try
        {
          File.Copy(file, Path.Combine(d.FullName, Path.GetFileName(file)), true);
        }
        catch (Exception E) //Nothing yet
        { }
      }

      IsRunning = false;
      (sender as IScenarioModel).SimulationFinished -= (model_SimulationFinished);

      if (ScenarioFinished != null)
        ScenarioFinished(sender, null);
    }
  }
}
