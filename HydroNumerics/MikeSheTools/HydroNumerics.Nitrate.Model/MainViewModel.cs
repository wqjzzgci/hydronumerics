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
    private SafeFile LakeFile;
    private List<SafeFile> MapOutputFiles = new List<SafeFile>();

    
    public MainViewModel()
    {
      CurrentCatchments = new ObservableCollection<Catchment>();
    }

    /// <summary>
    /// Reads and parses the configuration file
    /// </summary>
    /// <param name="xmlfilename"></param>
    public void ReadConfiguration(string xmlfilename)
    {
      LogThis("Reading configfile: " + xmlfilename);

      XElement configuration = XDocument.Load(xmlfilename).Element("Configuration");

      var startxml =configuration.Element("SimulationStart");
      var endxml = configuration.Element("SimulationEnd");
      Start = new DateTime(int.Parse(startxml.Attribute("Year").Value), int.Parse(startxml.Attribute("Month").Value), 1);
      End = new DateTime(int.Parse(endxml.Attribute("Year").Value), int.Parse(endxml.Attribute("Month").Value), 1);
      CurrentTime = Start;

      LakeFile = new SafeFile() { FileName = configuration.Element("Lakes").SafeParseString("ShapeFileName") };

      //Read output section
      var output = configuration.Element("Output");
      var log = output.Element("Log");
      if(log!=null)
        LogFileName = log.SafeParseString("FileName");

      var csv = output.Element("AllData");
      if (csv != null)
        CSVOutputfile = csv.SafeParseString("CSVFileName");

      var mapouts = output.Element("MapOutputs");
      if(mapouts !=null)
        foreach (var mapout in mapouts.Elements())
        {
          if (mapout.SafeParseBool("Update")??true)
          {
            SafeFile sf = new SafeFile() {CheckIfFileExists=false, FileName = mapout.SafeParseString("ShapeFileName") };
            sf.Parameters.Add(mapout.SafeParseInt("FromYear") ?? Start.Year);
            sf.Parameters.Add(mapout.SafeParseInt("ToYear") ?? End.Year);
            sf.Parameters.Add(mapout.SafeParseInt("FromMonth") ?? Start.Month);
            sf.Parameters.Add(mapout.SafeParseInt("ToMonth") ?? End.Month);
            MapOutputFiles.Add(sf);
          }
        }

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
      foreach (var sourcemodelXML in configuration.Element("InternalSinks").Elements())
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
      foreach (var sourcemodelXML in configuration.Element("MainStreamSinks").Elements())
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


    /// <summary>
    /// Initializes all models
    /// </summary>
    public void Initialize()
    {
      LogThis("Initializing");

      //Clear old output file
      if (!string.IsNullOrEmpty(CSVOutputfile))
        File.Delete(CSVOutputfile);

      LogThis("Reading catchments");
      foreach(var cfile in CatchmentFiles)
        LoadCatchments(cfile.FileName);
      LogThis(AllCatchments.Values.Count + " catchments read");

//      LoadLakes(); //This should be made dependent on the actual submodels

      StateVariables = new DataTable();

      if (InitialConditionsfile != null) //Work from previous simulation
      {
        LogThis("Reading initial conditions from previous simulation. FileName: " + InitialConditionsfile.FileName);
        StateVariables.FromCSV(InitialConditionsfile.FileName);
        StateVariables.PrimaryKey = new DataColumn[] { StateVariables.Columns[0], StateVariables.Columns[1] };

        //Read in catchment values
        CurrentTime = Start;
        foreach (var c in AllCatchments.Values)
        {
          c.Precipitation = new TimeStampSeries();
          c.Temperature = new TimeStampSeries();
          c.M11Flow = new TimeStampSeries();
          c.Leaching = new TimeStampSeries();
          while (CurrentTime < End)
          {
            var CurrentState = StateVariables.Rows.Find(new object[] { c.ID, CurrentTime });
            if(!CurrentState.IsNull("Precipitation"))
            c.Precipitation.Items.Add(new Time2.TimeStampValue( CurrentTime, (double)CurrentState["Precipitation"]));

            if (!CurrentState.IsNull("Air Temperature"))
              c.Temperature.Items.Add(new Time2.TimeStampValue(CurrentTime, (double)CurrentState["Air Temperature"]));
            if (!CurrentState.IsNull("M11Flow"))
              c.M11Flow.Items.Add(new Time2.TimeStampValue(CurrentTime, (double)CurrentState["M11Flow"]));
            if (!CurrentState.IsNull("Leaching"))
              c.Leaching.Items.Add(new Time2.TimeStampValue(CurrentTime, (double)CurrentState["Leaching"] / (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0)));
            CurrentTime = CurrentTime.AddMonths(1);
          }
          CurrentTime = Start;
        }
        CurrentTime = Start;
      }
      else
      {
        StateVariables.Columns.Add("ID", typeof(int));
        StateVariables.Columns.Add("Time", typeof(DateTime));
        StateVariables.Columns.Add("M11Flow", typeof(double));
        StateVariables.Columns.Add("Precipitation", typeof(double));
        StateVariables.Columns.Add("Air Temperature", typeof(double));
        StateVariables.Columns.Add("Leaching", typeof(double));
        StateVariables.Columns.Add("DownStreamOutput", typeof(double));
        StateVariables.PrimaryKey = new DataColumn[] { StateVariables.Columns[0], StateVariables.Columns[1] };

      }


      foreach (var c in AllCatchments.Values)
      {
        c.SourceModels = SourceModels;
        c.InternalReduction = InternalReductionModels;
        c.MainStreamReduction = MainStreamRecutionModels;
        c.StateVariables = StateVariables;
      }

      foreach (var mshe in MsheSetups)
        LoadMikeSheData(mshe.FileName, Start, End);

      LogThis("Initializing source models");
      foreach (var m in SourceModels)
      {
        if (!StateVariables.Columns.Contains(m.Name))
          StateVariables.Columns.Add(m.Name, typeof(double));

        if (m.Update)
          m.Initialize(Start, End, AllCatchments.Values);
      }

      //Do we have an updated groundwater model?
      GroundWaterSource gwmodel = SourceModels.FirstOrDefault(s => s.Update & s.GetType() == typeof(GroundWaterSource)) as GroundWaterSource;

      if (gwmodel != null)
      {
        foreach (var c in AllCatchments.Values)
        {
          CurrentTime = Start;
          c.Leaching = new TimeStampSeries();
          while (CurrentTime < End)
          {
            c.Leaching.Items.Add(new Time2.TimeStampValue(CurrentTime, gwmodel.leachdata.GetValue(c, CurrentTime)));
            CurrentTime = CurrentTime.AddMonths(1);
          }
        }
        CurrentTime = Start;

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


      //Fill in data in the 
    
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


      foreach (var mapout in MapOutputFiles)
      {
        using (ShapeWriter sw = new ShapeWriter(mapout.FileName))
        {
          foreach (var c in AllCatchments.Values)
          {
            GeoRefData gd = new GeoRefData() { Geometry = c.Geometry };
            gd.Data = StateVariables.NewRow();

            gd.Data[0] = c.ID;

             var Ctime = new DateTime((int)mapout.Parameters[0], (int)mapout.Parameters[2], 1);
            var sumend  = new DateTime((int)mapout.Parameters[1], (int)mapout.Parameters[3], 1);
            for (int k = 4; k < StateVariables.Columns.Count; k++)
              gd.Data[k] = 0;

            while (Ctime < sumend)
              {
                var row = StateVariables.Rows.Find(new object[] { c.ID, Ctime });
                for(int k=4;k<StateVariables.Columns.Count;k++)
                  if(!row.IsNull(k))
                    gd.Data[k] = (double)gd.Data[k] + (double) row[k];
                Ctime= Ctime.AddMonths(1);
              }
            sw.Write(gd);
          }
        }
      }
    }


    private object Lock= new object();

    #region Public methods
    public void LoadLakes()
    {
      List<Lake> lakes = new List<Lake>();
      LogThis("Reading lakes from: " + LakeFile.FileName);
      
      using (ShapeReader sr = new ShapeReader(LakeFile.FileName))
      {
        foreach (var ldata in sr.GeoData)
        {
          Lake l = new Lake() { 
            Geometry = ldata.Geometry as XYPolygon,
            ID = int.Parse(ldata.Data["OBJECTID_1"].ToString()),
          };

          if (l.Geometry == null)
            l.Geometry = ((MultiPartPolygon)ldata.Geometry).Polygons.OrderBy(p => p.GetArea()).Last(); //Just take the largest 

          l.HasDischarge = ldata.Data["Aflob"].ToString().ToLower().Trim() == "aflob";

          if (ldata.Data["Type"].ToString().ToLower().Trim() == "stor soe")
          {
            l.IsSmallLake = false;
            l.Name = ldata.Data["Navn"].ToString();
          }
          else //Only small lakes have cultivation class and soil types
          {
            string cultclass = ldata.Data["Dyrk_klass"].ToString().ToLower().Trim();
            if (cultclass == "> = 60 %")
              l.DegreeOfCultivation = CultivationClass.High;
            else if (cultclass == "< 30 %")
              l.DegreeOfCultivation = CultivationClass.Low;
            else if (cultclass == ">= 30 < 6*")
              l.DegreeOfCultivation = CultivationClass.Intermediate;
            l.SoilType = ldata.Data["Jord_type"].ToString();
          }
          lakes.Add(l);
        }
      }

      LogThis(lakes.Count + " lakes read");


      Parallel.ForEach(lakes.Where(la=>la.HasDischarge & la.IsSmallLake) , l =>
        {
          foreach (var c in AllCatchments.Values)
            if (c.Geometry.OverLaps(l.Geometry))
            {
              lock (Lock)
                c.Lakes.Add(l);
              break;
            }
        });

      LogThis(lakes.Count(la => la.HasDischarge & la.IsSmallLake) + " lakes distributed on " + AllCatchments.Values.Count(c => c.Lakes.Count > 0) + " catchments");
    }


    /// <summary>
    /// Loads a MikeShe setup
    /// </summary>
    /// <param name="SheFile"></param>
    public void LoadMikeSheData(string SheFile)
    {
      LoadMikeSheData(SheFile, DateTime.MinValue, DateTime.MaxValue);
    }

    /// <summary>
    /// Loads a MikeShe setup
    /// </summary>
    /// <param name="SheFile"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public void LoadMikeSheData(string SheFile, DateTime Start, DateTime End)
    {
      LogThis("Loading Mike she model. FileName: " + SheFile);
      MikeSheTools.Core.Model m = new MikeSheTools.Core.Model(SheFile);

      LogThis("Distributing m11 detailed time series on catchments");
      var m11 = m.Results.Mike11Observations.Where(mm => mm.Simulation != null).ToList();
      foreach (var c in AllCatchments.Values)
      {
        var flow = m11.FirstOrDefault(mm=>mm.Name== c.ID.ToString());
        if (flow != null)
        {
          c.M11Flow = Time2.TSTools.ChangeZoomLevel(flow.Simulation, TimeStepUnit.Month, false);
          Time2.TSTools.LimitTimeSeries(c.M11Flow, Start, End);
        }
      }
      LogThis(AllCatchments.Values.Count(c=>c.M11Flow!=null) + " catchments now have m11 flow");

      LogThis("Distributing precipitation");
      var precip = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.PrecipitationRate.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);
      foreach (var c in GetValuesFromGrid(precip, AllCatchments.Values.Where(c=>c.Precipitation==null).ToList(), true, Start, End))
        AllCatchments[c.Key].Precipitation = c.Value; 
      precip.Dispose();

      LogThis(AllCatchments.Values.Count(c => c.Precipitation != null) + " catchments now have precipitation");

      LogThis("Distributing temperature");
      var temperature = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.AirTemperature.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);
      foreach (var t in GetValuesFromGrid(temperature, AllCatchments.Values.Where(c => c.Temperature == null).ToList(), false, Start, End))
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

    private Dictionary<int, TimeStampSeries> GetValuesFromGrid(HydroNumerics.MikeSheTools.DFS.DFS2 precip, List<Catchment> CatcmentsToTest, bool Accumulate, DateTime Start, DateTime End)
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

      int firsttimestep = precip.GetTimeStep(Start);
      int lasttimestep = precip.GetTimeStep(End);

      for (int k = 0; k < 3; k++)
      {
        for (int i = firsttimestep; i <= lasttimestep; i++)
        {
          precipdata = precip.GetData(i, 1);
          for (int m = localcount; m < localcount + split; m++)
          {
            p[m].Item4.Items.Add(new TimeStampValue(precip.TimeSteps[i], precipdata[p[m].Item3, p[m].Item2]));
          }
        }

        for (int m = localcount; m < localcount + split; m++)
        {
          ToReturn.Add(p[m].Item1, TSTools.ChangeZoomLevel(p[m].Item4, TimeStepUnit.Month, Accumulate));
          p[m] = null;
        }
        localcount += split;
      }

      return ToReturn;
    }


    #endregion


  
  
  }
}

