using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Core
{
  public class PestModel : IScenarioModel
  {
    Process Runner;


    PSTFile file;

    public string MsheFileName{get;set;}

    public PestModel()
    {
      //hardcode
      Executable = "pest.exe";

      ResultFileNames = new ObservableCollection<string>();
    }

    public void Load(string fileName)
    {
      file = new PSTFile(fileName);
      file.Load();
      MsheFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file.FileName), "kft-sj_inv.she");
    }

    public string Executable { get; set; }

    public string DisplayName
    {
      get
      {
        return file.FileName;
      }
    }

    #region IScenarioModel Members

    public void BeginRun()
    {
      file.Save();
      Console.WriteLine("Pst-file saved");
      Runner = new Process();
      Runner.StartInfo.FileName = Executable;
      Runner.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(file.FileName);
      Runner.StartInfo.Arguments = file.FileName;
      Runner.EnableRaisingEvents = true;
      Runner.Exited += new EventHandler(Runner_Exited);
      Runner.Start();
      Console.WriteLine("Pest started");
    }

    void Runner_Exited(object sender, EventArgs e)
    {
      Console.WriteLine("Runner exited");
      Runner.Dispose();

      Model mshe = new Model(MsheFileName);
      OutputGenerator.KSTResults(mshe);
      Console.WriteLine("Output generated");
      mshe.Dispose();
      Console.WriteLine("Press any key to finish model");
      Console.ReadLine();
      if (SimulationFinished != null)
        SimulationFinished(this, e);
    }

    public event EventHandler SimulationFinished;

    public List<CalibrationParameter> Parameters
    {
      get 
      {
        if (file != null)
          return file.Parameters;
        else return null;
      }
    }

    public ObservableCollection<string> ResultFileNames { get; private set; }


    #endregion
  }
}
