using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    Stack<int> ParameterSets = new Stack<int>();

    public ObservableCollection<CalibrationParameter> Params { get; private set; }

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
    }




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
        models.Add(new Model(openFileDialog2.FileName));

        if (models.Count == 1)
        {
          Params = new ObservableCollection<CalibrationParameter>(models[0].Parameters);
          NotifyPropertyChanged("Params");
        }
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
      ParameterSets.Push(2);
      ParameterSets.Push(2);
      ParameterSets.Push(2);
      ParameterSets.Push(2);

      foreach (var v in models)
      {
        v.SimulationFinished += new EventHandler(v_SimulationFinished);
        RunNext(v);
      }
    }

    private void RunNext(Model mshe)
    {
      if (ParameterSets.Count > 0)
      {
        ParameterSets.Pop();
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
      Thread.Sleep(10);

      RunNext(mshe);

    }


    #endregion


  }
}
