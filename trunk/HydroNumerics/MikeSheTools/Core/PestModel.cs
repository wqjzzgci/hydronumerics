using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.MikeSheTools.Core
{
  public class PestModel : IScenarioModel
  {
    Process Runner;


    PSTFile file;

    public string MsheFileName{get;set;}

    public string Status { get; private set; }

    public PestModel(string fileName)
    {
      //hardcode
      Executable = "pest.exe";

      ResultFileNames = new ObservableCollection<string>();
      file = new PSTFile(fileName);
    }


    public string Executable { get; set; }
    public string PostProcessBatFile { get; set; }


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
      Runner.Dispose();
      if (SimulationFinished != null)
        SimulationFinished(this, e);
    }

    public void PostProcess()
    {
      Runner = new Process();
      Runner.StartInfo.FileName = PostProcessBatFile;
      Runner.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(file.FileName);
      Runner.WaitForExit();
      Runner.Start();

      
      Model mshe = new Model(MsheFileName);
      Status = OutputGenerator.KSTResults(mshe);
      mshe.Dispose();

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
