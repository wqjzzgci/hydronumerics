using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using HydroNumerics.Core;
using HydroNumerics.Time2;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class MainViewModel : BaseViewModel
  {

    private DataTable StateVariables;
    private List<ISource> SourceModels;
    private List<ISink> InternalReductionModels;
    private List<ISink> MainStreamRecutionModels;
    private List<SafeFile> MsheSetups = new List<SafeFile>();
    private List<SafeFile> CatchmentFiles = new List<SafeFile>();
    private SafeFile InitialConditionsfile;

    
    public MainViewModel()
    {
      CurrentCatchments = new ObservableCollection<Catchment>();
    }

    public void ReadConfiguration(string xmlfilename)
    {
      LogThis("Reading configfile: " + xmlfilename);

      XElement configuration = XDocument.Load(xmlfilename).Element("Configuration");

      var startxml =configuration.Element("SimulationStart");
      var endxml = configuration.Element("SimulationEnd");
      Start = new DateTime(int.Parse(startxml.Attribute("Year").Value), int.Parse(startxml.Attribute("Month").Value), 1);
      End = new DateTime(int.Parse(endxml.Attribute("Year").Value), int.Parse(endxml.Attribute("Month").Value), 1);
      CurrentTime = Start;


      //Read output section
      var output = configuration.Element("Output");
      var log = output.Element("Log");
      if(log!=null)
        LogFileName = log.SafeParseString("FileName");

      var csv = output.Element("AllData");
      if (csv != null)
        CSVOutputfile = csv.SafeParseString("CSVFileName");


      if (configuration.Element("InitialConditions").SafeParseBool("Use") ?? false)
      {
        InitialConditionsfile = new SafeFile() { FileName = configuration.Element("InitialConditions").SafeParseString("CSVFileName") };
      }

      foreach (var mshe in configuration.Element("MikeSheModels").Elements("MikeSheModel"))
      {
        if(mshe.SafeParseBool("Update") ?? true)
          MsheSetups.Add(new SafeFile(){ FileName = mshe.SafeParseString("SheFileName")});
      }


      //Configuration of sourcemodels
      SourceModels = new List<ISource>();
      foreach (var sourcemodelXML in configuration.Element("SourceModels").Elements())
      {
        ISource NewModel= ModelFactory.GetSourceModel(sourcemodelXML.Name.LocalName);
        if (NewModel != null)
        {
          SourceModels.Add(NewModel);
          NewModel.MessageChanged += new NewMessageEventhandler(NewModel_MessageChanged);
          NewModel.ReadConfiguration(sourcemodelXML);
        }
      }

      LogThis("Source models created");


      InternalReductionModels = new List<ISink>();
      //Configuration of internal reduction models
      foreach (var sourcemodelXML in configuration.Element("InternalReductionModels").Elements())
      {
        ISink NewModel = ModelFactory.GetSinkModel(sourcemodelXML.Name.LocalName);
        if (NewModel != null)
        {
          InternalReductionModels.Add(NewModel);
          NewModel.MessageChanged+=new NewMessageEventhandler(NewModel_MessageChanged);
          NewModel.ReadConfiguration(sourcemodelXML);
        }
      }

      LogThis("Internal sink models created");

      MainStreamRecutionModels = new List<ISink>();
      //Configuration of internal reduction models
      foreach (var sourcemodelXML in configuration.Element("MainStreamRecutionModels").Elements())
      {
        ISink NewModel = ModelFactory.GetSinkModel(sourcemodelXML.Name.LocalName);
        if (NewModel != null)
        {
          MainStreamRecutionModels.Add(NewModel);
          NewModel.MessageChanged += new NewMessageEventhandler(NewModel_MessageChanged);
          NewModel.ReadConfiguration(sourcemodelXML);
        }
      }

      LogThis("Main stream sink models created");

      CatchmentFiles.Add(new SafeFile() { FileName = configuration.Element("Catchments").SafeParseString("ShapeFileName") });

    }



    public void Initialize()
    {
      LogThis("Initializing");

      LogThis("Reading catchments");
      foreach(var cfile in CatchmentFiles)
        LoadCatchments(cfile.FileName);

      LogThis(AllCatchments.Values.Count + " catchments read");

      StateVariables = new DataTable();

      if (InitialConditionsfile != null) //Work from previous simulation
      {
        LogThis("Reading initial conditions from previous simulation. FileName: " + InitialConditionsfile.FileName);
        StateVariables.FromCSV(InitialConditionsfile.FileName);
      }
      else
      {
        StateVariables.Columns.Add("ID", typeof(int));
        StateVariables.Columns.Add("Time", typeof(DateTime));
        StateVariables.Columns.Add("M11Flow", typeof(double));
        StateVariables.Columns.Add("DownStreamOutput", typeof(double));
      }

      StateVariables.PrimaryKey = new DataColumn[] { StateVariables.Columns[0], StateVariables.Columns[1] };

      foreach (var c in AllCatchments.Values)
      {
        c.SourceModels = SourceModels;
        c.InternalReduction = InternalReductionModels;
        c.MainStreamReduction = MainStreamRecutionModels;
        c.StateVariables = StateVariables;
      }

      foreach (var mshe in MsheSetups)
        LoadMikeSheData(mshe.FileName);

      LogThis("Removing catchments without precipitation");
      var islands = AllCatchments.Where(c => c.Value.Precipitation == null).Select(c => c.Key).ToList();
      foreach (var island in islands)
        AllCatchments.Remove(island);

      LogThis("Initializing source models");
      foreach (var m in SourceModels)
      {
        if (!StateVariables.Columns.Contains(m.Name))
          StateVariables.Columns.Add(m.Name, typeof(double));

        if (m.Update)
          m.Initialize(Start, End, AllCatchments.Values);
      }

      LogThis("Initializing internal sink models");
      foreach (var m in InternalReductionModels)
      {
        if (!StateVariables.Columns.Contains(m.Name))
          StateVariables.Columns.Add(m.Name, typeof(double));

        if (m.Update)
          m.Initialize(Start, End, AllCatchments.Values);
      }

      LogThis("Initializing main stream models");
      foreach (var m in MainStreamRecutionModels)
      {
        if (!StateVariables.Columns.Contains(m.Name))
          StateVariables.Columns.Add(m.Name, typeof(double));

        if (m.Update)
          m.Initialize(Start, End, AllCatchments.Values);
      }
    
    }

    public void Run()
    {
      LogThis("Model run started");
      Run(End);
      LogThis("Model run ended");
    }



    public void Run(DateTime End)
    {
      while (CurrentTime < End)
      {
        foreach (var c in EndCatchments)
        {
          c.MoveInTime(CurrentTime);
        }
        CurrentTime = CurrentTime.AddMonths(1);
      }
    }


    public void Print()
    {
      LogThis("Writing output");
      if (!string.IsNullOrEmpty(CSVOutputfile))
        StateVariables.ToCSV(CSVOutputfile);
    }




    #region Public methods
    public void LoadMikeSheData(string SheFile)
    {
      LogThis("Loading Mike she model. FileName: " + SheFile);
      MikeSheTools.Core.Model m = new MikeSheTools.Core.Model(SheFile);

      LogThis("Distributing m11 detailed time series on catchments");
      var m11 = m.Results.Mike11Observations.Where(mm => mm.Simulation != null).ToList();
      foreach (var c in AllCatchments.Values)
      {
        var flow = m11.FirstOrDefault(mm=>mm.Name== c.ID.ToString());
        if (flow != null)
          c.M11Flow = Time2.TSTools.ChangeZoomLevel(flow.Simulation, TimeStepUnit.Month, true);
      }
      LogThis(AllCatchments.Values.Count(c=>c.M11Flow!=null) + " catchments now have m11 flow");

      LogThis("Distributing precipitation");
      var precip = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.PrecipitationRate.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);
      foreach (var c in GetValuesFromGrid(precip, AllCatchments.Values.Where(c=>c.Precipitation==null).ToList(), true))
        AllCatchments[c.Key].Precipitation = c.Value; 
      precip.Dispose();
      LogThis(AllCatchments.Values.Count(c => c.Precipitation != null) + " catchments now have precipitation");

      LogThis("Distributing temperature");
      var temperature = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.AirTemperature.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);
      foreach (var t in GetValuesFromGrid(temperature, AllCatchments.Values.Where(c=>c.Temperature==null).ToList(), false))
        AllCatchments[t.Key].Temperature = t.Value;
      temperature.Dispose();
      LogThis(AllCatchments.Values.Count(c => c.Temperature != null) + " catchments now have Temperature");


      m.Dispose();

      LogThis("Mike she model loaded");
    }



    /// <summary>
    /// Loads the catchments and connects them
    /// </summary>
    /// <param name="ShapeFileName"></param>
    public void LoadCatchments(string ShapeFileName)
    {
      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        if (AllCatchments == null)
          AllCatchments = new Dictionary<int, Catchment>();

        var data = sr.GeoData.ToList();
        foreach (var c in data)
        {
          Catchment ca = new Catchment((int)c.Data[0]);
          if (!AllCatchments.ContainsKey(ca.ID))
            AllCatchments.Add(ca.ID, ca);

          ca.Geometry = (IXYPolygon) c.Geometry;
        }



        foreach (var c in data)
        {
          int catcid = ((int)c.Data[0]);
          int downid = ((int)c.Data[1]);
          Catchment DownStreamCatchment;
          if (AllCatchments.TryGetValue(downid, out DownStreamCatchment))
          {
            if (DownStreamCatchment != AllCatchments[catcid]) //Do not allow reference to self
            {
              AllCatchments[catcid].DownstreamConnection = DownStreamCatchment;
              DownStreamCatchment.UpstreamConnections.Add(AllCatchments[catcid]);
            }
          }
        }
      }
    }

    #endregion

    #region Properties

    private DateTime _CurrentTime;
    public DateTime CurrentTime
    {
      get { return _CurrentTime; }
      set
      {
        if (_CurrentTime != value)
        {
          _CurrentTime = value;
          NotifyPropertyChanged("CurrentTime");
        }
      }
    }


    private DateTime _End;
    public DateTime End
    {
      get { return _End; }
      set
      {
        if (_End != value)
        {
          _End = value;
          NotifyPropertyChanged("End");
        }
      }
    }


    private DateTime _Start;
    public DateTime Start
    {
      get { return _Start; }
      set
      {
        if (_Start != value)
        {
          _Start = value;
          NotifyPropertyChanged("Start");
        }
      }
    }


    private Catchment currentCatchment;

    public Catchment CurrentCatchment
    {
      get
      { return currentCatchment; }
      set
      {
        if (value != currentCatchment)
        {
          currentCatchment = value;
          NotifyPropertyChanged("CurrentCatchment");

          if (!CurrentCatchments.Contains(currentCatchment))
          {
            CurrentCatchments.Clear();
            RecursiveUpstreamAdd(GetNextDownstream(currentCatchment));
          }
        }
      }
    }

    private string _LogFileName;
    public string LogFileName
    {
      get { return _LogFileName; }
      set
      {
        if (_LogFileName != value)
        {
          _LogFileName = value;
          if (File.Exists(_LogFileName))
            File.Delete(_LogFileName);
          NotifyPropertyChanged("LogFileName");
        }
      }
    }

    private string _CSVOutputfile;
    public string CSVOutputfile
    {
      get { return _CSVOutputfile; }
      set
      {
        if (_CSVOutputfile != value)
        {
          _CSVOutputfile = value;
          NotifyPropertyChanged("CSVOutputfile");
        }
      }
    }

    private ObservableCollection<Catchment> _EndCatchments;
    public ObservableCollection<Catchment> EndCatchments
    {
      get
      {
        if (_EndCatchments == null)
          _EndCatchments = new ObservableCollection<Catchment>(AllCatchments.Values.Where(c => c.DownstreamConnection == null));

        return _EndCatchments;
      }
      set
      {
        if (_EndCatchments != value)
        {
          _EndCatchments = value;
          NotifyPropertyChanged("EndCatchments");
        }
      }
    }


    public Dictionary<int, Catchment> AllCatchments { get; private set; }


    public ObservableCollection<Catchment> CurrentCatchments { get; private set; }


    #endregion


    #region Private Methods

    private void NewModel_MessageChanged(INitrateModel sender, string Message)
    {
      LogThis(sender.Name + ": " + Message);
    }

    private void LogThis(string Message)
    {
      string line = DateTime.Now.ToString("HH:mm:ss") + ". " + Message;
      if (!string.IsNullOrEmpty(LogFileName))
        using (StreamWriter sw = new StreamWriter(LogFileName, true))
        {
          sw.WriteLine(line);
        }
      Console.WriteLine(line);
    }


    private void RecursiveUpstreamAdd(Catchment c)
    {
      CurrentCatchments.Add(c);
      foreach (var ups in c.UpstreamConnections)
        RecursiveUpstreamAdd(ups);
    }

    private Catchment GetNextDownstream(Catchment c)
    {
      if (c.DownstreamConnection == null)
        return c;
      return GetNextDownstream(c.DownstreamConnection);
    }

    private Dictionary<int, TimeStampSeries> GetValuesFromGrid(HydroNumerics.MikeSheTools.DFS.DFS2 precip, List<Catchment> CatcmentsToTest, bool Accumulate)
    {
      var polygons = XYPolygon.GetPolygons(precip);

      List<Tuple<int, int, int, TimeStampSeries>> p = new List<Tuple<int, int, int, TimeStampSeries>>();

      var precipdata = precip.GetData(0, 1);
      HydroNumerics.MikeSheTools.DFS.DFS2.MaxEntriesInBuffer = 1;


      for (int i = 0; i < precip.NumberOfColumns; i++)
        for (int j = 0; j < precip.NumberOfRows; j++)
        {
          for (int k = CatcmentsToTest.Count - 1; k >= 0; k--)
          {
            if (CatcmentsToTest[k].Geometry.OverLaps(polygons[i, j]) & precipdata[j, i] != precip.DeleteValue)
            {
              p.Add(new Tuple<int, int, int, TimeStampSeries>(CatcmentsToTest[k].ID, i, j, new TimeStampSeries()));
              CatcmentsToTest.Remove(CatcmentsToTest[k]);
            }
          }
        }

      int split = p.Count / 3;
      int localcount = 0;
      Dictionary<int, TimeStampSeries> ToReturn = new Dictionary<int, TimeStampSeries>();

      for (int k = 0; k < 3; k++)
      {
        for (int i = 0; i < precip.NumberOfTimeSteps; i++)
        {
          precipdata = precip.GetData(i, 1);
          for (int m = localcount; m < localcount + split; m++)
          {
            p[m].Item4.Items.Add(new TimeStampValue(precip.TimeSteps[i], precipdata[p[m].Item3, p[m].Item2]));
          }
        }

        for (int m = localcount; m < localcount + split; m++)
        {
          ToReturn.Add(p[m].Item1, TSTools.ChangeZoomLevel(p[m].Item4, TimeStepUnit.Month, true));
          p[m] = null;
        }
        localcount += split;
      }

      return ToReturn;
    }


    #endregion


  
  
  }
}

