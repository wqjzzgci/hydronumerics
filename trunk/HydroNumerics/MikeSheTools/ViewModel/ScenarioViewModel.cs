using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

using NDepend.Helpers.FileDirectoryPath;

using HydroNumerics.Core;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScenarioViewModel : NotifyPropertyChangedBase
  {
    private ObservableCollection<IScenarioModel> models;

    private string outputDirectory;
    public string OutputDirectory
    {
      get
      {
        return outputDirectory;
      }
      set
      {
        if (outputDirectory != value)
        {
          outputDirectory = value;
          NotifyPropertyChanged("OutputDirectory");
        }
      }
    }

    private string prefix = "Run";
    public string Prefix
    {
      get
      {
        return prefix;
      }
      set
      {
        if (prefix != value)
        {
          prefix = value;
          NotifyPropertyChanged("Prefix");
        }
      }
    }

    private FilePathRelative mikeSheFileName;
    public string MikeSheFileName
    {
      get
      {
        if (mikeSheFileName == null)
          return "";
        return mikeSheFileName.Path;
      }
    }

    public ObservableCollection<string> FileNamesToCopy { get; private set; }

    public ObservableCollection<CalibrationParameter> Params { get; private set; }
    private ConcurrentStack<ScenarioRun> ScenariosToRun;
    private SimlabFile slf;

    public ObservableCollection<IScenarioModel> Models
    {
      get
      {
        return models;
      }
    }

    public ScenarioViewModel()
    {
      models = new ObservableCollection<IScenarioModel>();
      FileNamesToCopy = new ObservableCollection<string>();
      NumberOfScenarios = 10;
      SeedValue = 123456;
    }

    public List<ScenarioRun> Runs { get; set; }

    public List<ScenarioResultViewModel> Results { get; private set; }


    public void GenerateParameterSets()
    {
      Random R = new Random(SeedValue);

      Runs = new List<ScenarioRun>();

      for (int i = 0; i < NumberOfScenarios; i++)
      {
        ScenarioRun sc = new ScenarioRun();
        sc.Number = i + 1;
        sc.ParamValues = new SortedList<CalibrationParameter, double?>();

        foreach (var v in Params.Where(var => var.ParType != ParameterType.Fixed & var.ParType != ParameterType.tied))
          sc.ParamValues.Add(v, null);
        Runs.Add(sc);
      }


      foreach (var v in Params.Where(var => var.ParType != ParameterType.Fixed & var.ParType != ParameterType.tied))
      {
        double stepsize = (v.CurrentValue*1.1 - v.CurrentValue*0.9) / NumberOfScenarios;

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
          addModelCommand = new RelayCommand(param => this.LoadModel(), param => true);
        }
        return addModelCommand;
      }
    }


    private void LoadModel()
    {

      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.she; *.pst)|*.she; *.pst";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select a Mike She input file or pst file";

      if (openFileDialog2.ShowDialog().Value)
      {
        LoadModel(openFileDialog2.FileName);
      }
    }
    private void LoadModel(string filename)
    {
      IScenarioModel m;
      if (Path.GetExtension(filename).Contains("she"))
      {
        m = new MikeSheScenarioModel(filename);
      }
      else
      {
        m = new PestModel(filename);
      }
      if (models.Count == 0)
      {
        AsyncWithWait(() => Params = new ObservableCollection<CalibrationParameter>(m.Parameters))
        .ContinueWith((tt) => NotifyPropertyChanged("Params")).Wait();
        OutputDirectory = Directory.GetParent(Path.GetDirectoryName(filename)).FullName;
      }
      models.Add(m);

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
          generateSamplesCommand = new RelayCommand(param => GenerateParameterSets(), param => Params != null);
        }
        return generateSamplesCommand;
      }
    }
    #endregion

    #region LoadSimLabCommand
    RelayCommand loadSimLabCommand;
    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand LoadSimLabCommand
    {
      get
      {
        if (loadSimLabCommand == null)
        {
          loadSimLabCommand = new RelayCommand(param => loadSimLab(), param => true);
        }
        return loadSimLabCommand;
      }
    }

    private void loadSimLab()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.sam)|*.sam";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select a Sim lab output file";

      if (openFileDialog2.ShowDialog().Value)
      {
        loadSimLab(openFileDialog2.FileName);
      }
    }
    private void loadSimLab(string filename)
    {

      Runs = new List<ScenarioRun>();
      slf = new SimlabFile();
      slf.Load(filename);

      for (int i = 0; i < slf.Samples.Values.First().Count(); i++)
      {
        ScenarioRun sc = new ScenarioRun();
        sc.Number = i + 1;
        sc.ParamValues = new SortedList<CalibrationParameter, double?>();

        foreach (var v in slf.Samples.Keys)
        {
          sc.ParamValues.Add(v, slf.Samples[v][i]);
        }
        Runs.Add(sc);
      }
      NotifyPropertyChanged("Runs");

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
          runCommand = new RelayCommand(param => this.Run(), param => models.Count > 0 & Runs != null && Runs.Count > 0);
        }
        return runCommand;
      }
    }

    private void Run()
    {
      DirectoryPathAbsolute dp = new DirectoryPathAbsolute(Path.GetDirectoryName(models.First().DisplayName));

      ScenariosToRun = new ConcurrentStack<ScenarioRun>(Runs.Where(var=>var.RunThis));
      foreach (var v in models)
      {
        v.ResultFileNames.Clear();
        DirectoryPathAbsolute dp2 = new DirectoryPathAbsolute(Path.GetDirectoryName(v.DisplayName));

        foreach (var file in FileNamesToCopy)
        {
          FilePathAbsolute fp = new FilePathAbsolute(file);
          var rp = fp.GetPathRelativeFrom(dp);
          v.ResultFileNames.Add(rp.GetAbsolutePathFrom(dp2).Path);
        }

        if (v is PestModel)
        {
          ((PestModel)v).MsheFileName = mikeSheFileName.GetAbsolutePathFrom(dp2).Path;
        }

        RunNext(v);
      }
    }

    private void RunNext(IScenarioModel mshe)
    {
      ScenarioRun sc;
      if (ScenariosToRun.TryPop(out sc))
      {
        sc.OutputDirectory = Path.Combine(OutputDirectory, Prefix);
        sc.ScenarioFinished += new EventHandler(sc_ScenarioFinished);
        sc.Run(mshe);
      }
    }

    void sc_ScenarioFinished(object sender, EventArgs e)
    {

      //Allow IScenarioModel to close all files
      Thread.Sleep(10);
      RunNext(sender as IScenarioModel);
    }

    #endregion

    #region SaveSetupCommand
    RelayCommand saveSetupCommand;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand SaveSetupCommand
    {
      get
      {
        if (saveSetupCommand == null)
        {
          saveSetupCommand = new RelayCommand(param => this.SaveSetup(), param => true);
        }
        return saveSetupCommand;
      }
    }

    private void SaveSetup()
    {
      Microsoft.Win32.SaveFileDialog SaveFileDialog = new Microsoft.Win32.SaveFileDialog();
      SaveFileDialog.Filter = "Known file types (*.xml)|*.xml";
      SaveFileDialog.Title = "Save scenario info in xml-file";

      if (SaveFileDialog.ShowDialog().HasValue)
      {
        using (StreamWriter sw = new StreamWriter(SaveFileDialog.FileName))
        {
          sw.Write(ToXml().ToString());
        }
      }
    }


    private XElement ToXml()
    {
      XElement x = new XElement("ScenarioRuns");

      if (models != null && models.Count > 0)
      {
        var Elfile = new XElement("ModelFiles");

        foreach (var v in models)
        {
          Elfile.Add(new XElement("FileName", v.DisplayName));
        }
        x.Add(Elfile);
      }

      x.Add(new XElement("OutputDirectory", OutputDirectory));

      x.Add(new XElement("Prefix", Prefix));
      
      if (mikeSheFileName!=null)
        x.Add(new XElement("MikeSheFileName", MikeSheFileName));


      if (slf != null)
      {
        x.Add(new XElement("SimlabFileName", slf.FileName));
      }


      if (Runs != null)
      {
        StringBuilder s = new StringBuilder();
        foreach (var r in Runs.Where(var => var.RunThis))
        {
          s.Append(", " + r.Number);
        }
        x.Add(new XElement("ScenariosToRun", s.ToString()));
      }

      var file = new XElement("FilesToCopy");

      foreach (var v in this.FileNamesToCopy)
      {
        file.Add(new XElement("FileName", v));
      }
      x.Add(file);

      return x;
    }

    #endregion

        #region GetMsheCommand
    RelayCommand getMsheCommand;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand GetMsheCommand
    {
      get
      {
        if (getMsheCommand == null)
        {
          getMsheCommand = new RelayCommand(param => this.GetMshe(), param => ShouldGetMshe);
        }
        return getMsheCommand;
      }
    }

    private void GetMshe()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.she)|*.she";
      openFileDialog2.Title = "Select the Mike She file corresponding to the first .pst-file";

      if (openFileDialog2.ShowDialog().Value)
      {
        DirectoryPathAbsolute dp = new DirectoryPathAbsolute(Path.GetDirectoryName(models.First().DisplayName));
        FilePathAbsolute fp = new FilePathAbsolute(openFileDialog2.FileName);
        mikeSheFileName = fp.GetPathRelativeFrom(dp);
        NotifyPropertyChanged("MikeSheFileName");

      }
    }

    private bool ShouldGetMshe
    {
      get
      {
        if (models.Count > 0)
          if (models[0] is PestModel)
            return true;
        return false;
      }
    }

        #endregion

    #region LoadSetupCommand
    RelayCommand loadSetupCommand;

    public ICommand LoadSetupCommand
    {
      get
      {
        if (loadSetupCommand == null)
        {
          loadSetupCommand = new RelayCommand(param => this.LoadSetup(), param => true);
        }
        return loadSetupCommand;
      }
    }

    private void LoadSetup()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.xml)|*.xml";
      openFileDialog2.Title = "Select an xml file with scenario info";

      if (openFileDialog2.ShowDialog().Value)
      {
        FromXML(XDocument.Load(openFileDialog2.FileName));
      }
    }

    private void FromXML(XDocument XDoc)
    {

      var Elem = XDoc.Element("ScenarioRuns");

      var m = Elem.Element("ModelFiles");
      if (m != null)
      {
        foreach (var f in m.Elements("FileName"))
        {
          this.LoadModel(f.Value);
        }
      }

      OutputDirectory = Elem.Element("OutputDirectory").Value;
      Prefix = Elem.Element("Prefix").Value;

      m = Elem.Element("MikeSheFileName");
      if (m != null)
      {
        mikeSheFileName = new FilePathRelative(m.Value);
        NotifyPropertyChanged("MikeSheFileName");
      }

      var fs = Elem.Element("FilesToCopy");
      if (fs != null)
      {
        foreach (var f in fs.Elements("FileName"))
        {
          this.FileNamesToCopy.Add(f.Value);
        }
      }


      var slf = Elem.Element("SimlabFileName");
      if (slf != null)
      {
        loadSimLab(slf.Value);
      }

      var sctorun = Elem.Element("ScenariosToRun");

      if (sctorun != null & Runs != null)
      {
        var splits = sctorun.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var r in Runs)
          r.RunThis = false;

        foreach (var s in splits)
        {          
          int index;
          if (int.TryParse(s, out index))
          {
            if (index < Runs.Count)
            {
              var r = Runs.SingleOrDefault(var => var.Number == index);
              if (r != null)
                r.RunThis = true;
            }
          }
        }
      }
    }

    #endregion

    #region LoadResultsCommand
    RelayCommand loadResultsCommand;

    public ICommand LoadResultsCommand
    {
      get
      {
        if (loadResultsCommand == null)
        {
          loadResultsCommand = new RelayCommand(param => this.LoadResults(), param => OutputDirectory!=string.Empty);
        }
        return loadResultsCommand;
      }
    }

    private void LoadResults()
    {
      Results = new List<ScenarioResultViewModel>();

      foreach (var v in Directory.GetDirectories(OutputDirectory, Prefix + "*"))
        Results.Add(new ScenarioResultViewModel(new DirectoryInfo(v)));

      NotifyPropertyChanged("Results");
    }
    #endregion


  }

}
