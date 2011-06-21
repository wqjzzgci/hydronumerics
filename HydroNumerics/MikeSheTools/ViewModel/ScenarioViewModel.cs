using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Windows.Input;


using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScenarioViewModel:BaseViewModel
  {
    private ObservableCollection<Model> models;

    public ObservableCollection<CalibrationParameterViewModel> Params { get; private set; }
    private ConcurrentStack<ScenarioRun> ScenariosToRun;

    public ObservableCollection<Model> Models
    {
      get
      {
        return models;
      }
    }

    public ScenarioViewModel()
    {
      models = new ObservableCollection<Model>();
      NumberOfScenarios = 10;
      SeedValue = 123456;
    }

    public List<ScenarioRun> Runs { get; set; }

    
    public void GenerateParameterSets()
    {
      Random R = new Random(SeedValue);

      Runs = new List<ScenarioRun>();

      for (int i = 0; i < NumberOfScenarios; i++)
      {
        ScenarioRun sc = new ScenarioRun();
        sc.Number = i + 1;
        sc.ParamValues = new SortedList<CalibrationParameterViewModel, double?>();
        foreach (var v in Params.Where(var => var.IsUsedInCalibration))
          sc.ParamValues.Add(v, null);
        Runs.Add(sc);
      }


      foreach (var v in Params.Where(var => var.IsUsedInCalibration))
      {
        double stepsize = (v.MaxValue - v.MinValue) / NumberOfScenarios;


        for (int i = 0; i < NumberOfScenarios; i++)
        {
          int k;
          while (Runs[k = R.Next(0, NumberOfScenarios)].ParamValues[v].HasValue) ;

          Runs[k].ParamValues[v] = R.NextDouble() * stepsize + v.MinValue + i * stepsize;
        }
      }
      NotifyPropertyChanged("Runs");
    }

    public int SeedValue { get; set; }
    public int NumberOfScenarios { get; set; }

    #region AddModelCommand
    RelayCommand addModelCommand;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand AddModelCommand
    {
      get
      {
        if (addModelCommand == null)
        {
          addModelCommand = new RelayCommand(param => this.LoadMshe(), param =>true);
        }
        return addModelCommand;
      }
    }


    private void LoadMshe()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.she)|*.she";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select a Mike She input file";

      if (openFileDialog2.ShowDialog().Value)
      {
        Model m = new Model(openFileDialog2.FileName);

        if (models.Count == 0)
        {
          AsyncWithWait(() => Params = new ObservableCollection<CalibrationParameterViewModel>(m.Parameters.Select(var=>new CalibrationParameterViewModel(var))))
          .ContinueWith((tt)=>NotifyPropertyChanged("Params"));
        }
        models.Add(m);
      }
    }


    #endregion

    #region GenerateSamplesCommand
    RelayCommand generateSamplesCommand;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand GenerateSamplesCommand
    {
      get
      {
        if (generateSamplesCommand == null)
        {
          generateSamplesCommand = new RelayCommand(param => GenerateParameterSets(), param => Params!=null && Params.Where(var => var.IsUsedInCalibration).Count()>0);
        }
        return generateSamplesCommand;
      }
    }
    #endregion


    #region RunCommand
    RelayCommand runCommand;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand RunCommand
    {
      get
      {
        if (runCommand == null)
        {
          runCommand = new RelayCommand(param => this.Run(), param => models.Count>0);
        }
        return runCommand;
      }
    }

    private void Run()
    {
      ScenariosToRun = new ConcurrentStack<ScenarioRun>(Runs);
 
      foreach (var v in models)
      {
        v.SimulationFinished += new EventHandler(v_SimulationFinished);
        RunNext(v);
      }
    }

    private void RunNext(Model mshe)
    {
      ScenarioRun sc;
      if (ScenariosToRun.TryPop(out sc))
      {
        foreach (var v in sc.ParamValues)
          mshe.Parameters.Single(var => var.DisplayName == v.Key.DisplayName).CurrentValue = v.Value.Value;

        sc.IsRunning = true;
        mshe.Run(true, true);
      }
    }

    void v_SimulationFinished(object sender, EventArgs e)
    {
      Model mshe = sender as Model;
      var dfs = DfsFileFactory.OpenFile(mshe.Files.SZ3DFileName);
      double[] percentiles = new double[] { 0.1 };
      string filename = Path.Combine(mshe.Files.ResultsDirectory, "SZ3D_percentiles.dfs3");
      var dfsout = DfsFileFactory.CreateFile(filename, percentiles.Count());
      dfsout.CopyFromTemplate(dfs);
      dfs.Percentile(1, dfsout, percentiles, 80000000);
      dfsout.Dispose();
      DFS3 dfsout2 = new DFS3(filename);
      Console.WriteLine(dfsout2.GetData(0, 1)[10, 10, 0]);
      dfsout2.Dispose();
     
      //Allow MikeShe to close all files
      Thread.Sleep(10);

      RunNext(mshe);

    }


    #endregion



  }
  public class ScenarioRun
  {
    public SortedList<CalibrationParameterViewModel, double?> ParamValues { get; set; }
    public bool IsRunning { get; set; }
    public int Number { get; set; }
  }
}
