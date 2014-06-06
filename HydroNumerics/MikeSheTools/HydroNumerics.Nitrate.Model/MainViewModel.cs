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
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.Time2;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class MainViewModel : BaseViewModel
  {

    private DataTable StateVariables;
    private DataTable ReductionVariables;
    private List<ISource> SourceModels;
    private List<ISink> InternalReductionModels;
    private List<ISink> MainStreamRecutionModels;
    private List<SafeFile> MsheSetups = new List<SafeFile>();
    private SafeFile M11FlowOverride;
    private List<SafeFile> CatchmentFiles = new List<SafeFile>();
    private SafeFile InitialConditionsfile;
    private SafeFile AlldataFile;
    private SafeFile LakeFile;
    private SafeFile CoastalZone;
    private List<SafeFile> MapOutputFiles = new List<SafeFile>();
    private List<SafeFile> StatisticsMap = new List<SafeFile>();
    private List<SafeFile> DetailedParameterTimeSeries = new List<SafeFile>();
    private List<SafeFile> DetailedCatchmentTimeSeries = new List<SafeFile>();
    private SafeFile Stations;
    private SafeFile StationData;
    private ReductionMap reductioncreator;

    private SafeFile ExcelTemplate;
    
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
      string dir = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(Path.GetDirectoryName(xmlfilename));

      LogThis("Reading configfile: " + xmlfilename);

      XElement configuration = XDocument.Load(xmlfilename).Element("Configuration");

      var startxml =configuration.Element("SimulationStart");
      var endxml = configuration.Element("SimulationEnd");
      Start = new DateTime(int.Parse(startxml.Attribute("Year").Value), int.Parse(startxml.Attribute("Month").Value), 1);
      End = new DateTime(int.Parse(endxml.Attribute("Year").Value), int.Parse(endxml.Attribute("Month").Value), 1);
      CurrentTime = Start;

      LakeFile = new SafeFile() { FileName = configuration.Element("Lakes").SafeParseString("ShapeFileName") };

      SoilsShape = new SafeFile() { FileName = configuration.Element("SoilTypes").SafeParseString("ShapeFileName") };
      SoilsShape.ColumnNames.Add(configuration.Element("SoilTypes").SafeParseString("SoilTypeColumn"));

      Stations = new SafeFile() { FileName = configuration.Element("Observations").SafeParseString("ShapeFileName") };
      StationData = new SafeFile() { FileName = configuration.Element("Observations").SafeParseString("TransportFileName") };

      //Read output section
      var output = configuration.Element("Output");
      var log = output.Element("Log");
      if(log!=null && (log.SafeParseBool("Include") ?? true))
        LogFileName = Path.GetFullPath( log.SafeParseString("FileName"));

      var csv = output.Element("AllData");
      if (csv != null && (csv.SafeParseBool("Include") ?? true))
        AlldataFile = new SafeFile() { CheckIfFileExists=false, InitialDelete = true, FileName = csv.SafeParseString("CSVFileName") };

      var excel = output.Element("Calibration");
      if (excel != null && (excel.SafeParseBool("Include") ?? true))
      {
        ExcelTemplate = new SafeFile() { FileName = excel.SafeParseString("ExcelTemplate") };
        ExcelTemplate.ColumnNames.Add(Path.GetFullPath(excel.SafeParseString("OutputFolder")));
      }

      var mapouts = output.Element("MapOutputs");
      if(mapouts !=null)
        foreach (var mapout in mapouts.Elements())
        {
          if (mapout.SafeParseBool("Include")??true)
          {
            SafeFile sf = new SafeFile() {CheckIfFileExists=false, FileName = mapout.SafeParseString("ShapeFileName") };
            sf.Parameters.Add(mapout.SafeParseInt("FromYear") ?? Start.Year);
            sf.Parameters.Add(mapout.SafeParseInt("ToYear") ?? End.Year);
            sf.Parameters.Add(mapout.SafeParseInt("FromMonth") ?? Start.Month);
            sf.Parameters.Add(mapout.SafeParseInt("ToMonth") ?? End.Month);
            sf.Flags.Add(mapout.SafeParseBool("AreaWeighted") ?? false);
            sf.Flags.Add(mapout.SafeParseBool("Accumulated") ?? false);
            MapOutputFiles.Add(sf);
          }
        }

      mapouts = output.Element("StatisticsMaps");
      if (mapouts != null)
      {
        foreach (var mapout in mapouts.Elements())
        {
          if (mapout.SafeParseBool("Include") ?? true)
          {
            SafeFile sf = new SafeFile() { CheckIfFileExists = false, FileName = mapout.SafeParseString("ShapeFileName") };
            sf.Parameters.Add(mapout.SafeParseInt("FromYear") ?? Start.Year);
            sf.Parameters.Add(mapout.SafeParseInt("ToYear") ?? End.Year);
            sf.Parameters.Add(mapout.SafeParseInt("FromMonth") ?? Start.Month);
            sf.Parameters.Add(mapout.SafeParseInt("ToMonth") ?? End.Month);
            sf.Flags.Add(mapout.SafeParseBool("Yearly") ?? false);
            StatisticsMap.Add(sf);
          }
        }
      }


      //Detailed parameter time series
        var detailed = output.Element("DetailedParameterTimeSeries");
        if (detailed != null)
        {
          foreach (var mapout in detailed.Elements())
          {
            if (mapout.SafeParseBool("Include") ?? true)
            {
              SafeFile sf = new SafeFile() { CheckIfFileExists = false, InitialDelete=true, FileName = mapout.SafeParseString("CSVFileName") };
              sf.ColumnNames.Add(mapout.SafeParseString("Parameter"));
              sf.Flags.Add(mapout.SafeParseBool("Accumulated") ?? false);
              DetailedParameterTimeSeries.Add(sf);
            }
          }
        }

      //Detailed catchment time series
        detailed = output.Element("DetailedCatchmentTimeSeries");
        if (detailed != null)
        {
          foreach (var mapout in detailed.Elements())
          {
            if (mapout.SafeParseBool("Include") ?? true)
            {
              SafeFile sf = new SafeFile() { CheckIfFileExists = false, InitialDelete=true, FileName = mapout.SafeParseString("CSVFileName") };
              sf.Parameters.Add(mapout.SafeParseInt("CatchmentID")??0);
              sf.Flags.Add(mapout.SafeParseBool("Accumulated") ?? false);
              DetailedCatchmentTimeSeries.Add(sf);
            }
          }
        }


      if (configuration.Element("InitialConditions").SafeParseBool("Include") ?? false)
      {
        InitialConditionsfile = new SafeFile() { FileName = configuration.Element("InitialConditions").SafeParseString("CSVFileName") };
        InitialConditionsfile.ColumnNames.Add(configuration.Element("InitialConditions").SafeParseString("DateFormat"));
      }

      foreach (var mshe in configuration.Element("MikeSheModels").Elements("MikeSheModel"))
      {
        if(mshe.SafeParseBool("Update") ?? true)
          MsheSetups.Add(new SafeFile(){ FileName = mshe.SafeParseString("SheFileName")});
      }

      var m11override = configuration.Element("M11FlowOverride");
      if (m11override != null && (m11override.SafeParseBool("Include") ?? true) && (m11override.SafeParseBool("Update") ?? true))
        M11FlowOverride = new SafeFile() { FileName = m11override.SafeParseString("DFS0FileName") };

      var coastal = configuration.Element("CoastalZone");
      if(coastal!=null && (coastal.SafeParseBool("Include")??true))
      {
        CoastalZone = new SafeFile(){FileName = coastal.SafeParseString("ShapeFileName")};
        CoastalZone.ColumnNames.Add(coastal.SafeParseString("Column") ?? "Kyst");
        foreach (var elem in coastal.Elements("KeepValue"))
          CoastalZone.ColumnNames.Add(elem.SafeParseString("AttributeValue"));

      }

      //Configuration of sourcemodels
      SourceModels = new List<ISource>();
      foreach (var sourcemodelXML in configuration.Element("SourceModels").Elements())
      {
        ISource NewModel= ModelFactory.GetSourceModel(sourcemodelXML.Name.LocalName);
        if (NewModel != null)
        {
          NewModel.ReadConfiguration(sourcemodelXML);
          if (NewModel.Include)
          {
            SourceModels.Add(NewModel);
            NewModel.MessageChanged += new NewMessageEventhandler(NewModel_MessageChanged);
          }
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
          NewModel.ReadConfiguration(sourcemodelXML);
          if (NewModel.Include)
          {
            InternalReductionModels.Add(NewModel);
            NewModel.MessageChanged += new NewMessageEventhandler(NewModel_MessageChanged);
          }
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
          NewModel.ReadConfiguration(sourcemodelXML);
          if (NewModel.Include)
          {
            MainStreamRecutionModels.Add(NewModel);
            NewModel.MessageChanged += new NewMessageEventhandler(NewModel_MessageChanged);
          }
        }
      }

      var redmap = configuration.Element("ReductionMap");
      if(redmap!=null)
      {
        reductioncreator = new ReductionMap();
        reductioncreator.ReadConfiguration(redmap);
      }
      
      LogThis("Main stream sink models created");

      CatchmentFiles.Add(new SafeFile() { FileName = configuration.Element("Catchments").SafeParseString("ShapeFileName") });

      Directory.SetCurrentDirectory(dir);
    }



    /// <summary>
    /// Initializes all models
    /// </summary>
    public void Initialize()
    {
      LogThis("Initializing");

      LogThis("Reading catchments");
      foreach (var cfile in CatchmentFiles)
      {
        LogThis("Reading catchments from: " + cfile.FileName);
        LoadCatchments(cfile.FileName);
      }
      LogThis(AllCatchments.Values.Count + " catchments read");

      LoadStationData(Stations.FileName, StationData.FileName);
      LoadCoastalZone();
      LoadLakes(); //This should be made dependent on the actual submodels

      StateVariables = new DataTable();

      if (InitialConditionsfile != null) //Work from previous simulation
      {
        LogThis("Reading initial conditions from previous simulation. FileName: " + InitialConditionsfile.FileName);
        StateVariables.FromCSV(InitialConditionsfile.FileName, InitialConditionsfile.ColumnNames.First());
        LogThis("Have read " + StateVariables.Rows.Count);

        for (int i = 0; i < StateVariables.Columns.Count; i++)
          LogThis(StateVariables.Columns[i].ColumnName);

        //Read in catchment values
        CurrentTime = Start;
        foreach (var c in AllCatchments.Values)
        {

          var catchm = StateVariables.Rows.Find(new object[] { c.ID, StateVariables.Rows[0][1] });

          if (catchm == null)
            LogThis("Could not find inital conditions for cathment with ID = " + c.ID);
          else
          {


            var precipvalues = new List<double>();
            c.Temperature = new TimeStampSeries();
            c.Leaching = new TimeStampSeries();
            var m11values = new List<double>();
            while (CurrentTime < End)
            {
              var CurrentState = StateVariables.Rows.Find(new object[] { c.ID, CurrentTime });

              if (!CurrentState.IsNull("Precipitation"))
                precipvalues.Add((double)CurrentState["Precipitation"]);
              if (!CurrentState.IsNull("Air Temperature"))
                c.Temperature.Items.Add(new Time2.TimeStampValue(CurrentTime, (double)CurrentState["Air Temperature"]));
              if (!CurrentState.IsNull("M11Flow"))
                m11values.Add((double)CurrentState["M11Flow"] / (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0));
              if (!CurrentState.IsNull("Leaching"))
                c.Leaching.Items.Add(new Time2.TimeStampValue(CurrentTime, (double)CurrentState["Leaching"] / (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0)));
              CurrentTime = CurrentTime.AddMonths(1);
            }
            if (m11values.Count > 0)
            {
              c.M11Flow = new ZoomTimeSeries();
              c.M11Flow.GetTs(TimeStepUnit.Month).AddRange(Start, m11values);
            }
            if (precipvalues.Count > 0)
            {
              c.Precipitation = new ZoomTimeSeries() { Accumulate = true };
              c.Precipitation.GetTs(TimeStepUnit.Month).AddRange(Start, precipvalues);
            }
            CurrentTime = Start;
          }
        }
        CurrentTime = Start;
      }
      else
      {
        StateVariables.Columns.Add("ID", typeof(int));
        StateVariables.Columns.Add("Time", typeof(DateTime));
        StateVariables.Columns.Add("ObservedFlow", typeof(double));
        StateVariables.Columns.Add("ObservedNitrate", typeof(double));
        StateVariables.Columns.Add("M11Flow", typeof(double));
        StateVariables.Columns.Add("NetM11Flow", typeof(double));
        StateVariables.Columns.Add("Precipitation", typeof(double));
        StateVariables.Columns.Add("Air Temperature", typeof(double));
        StateVariables.Columns.Add("DownStreamOutput", typeof(double));
        StateVariables.Columns.Add("Leaching", typeof(double));
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

      if (M11FlowOverride != null)
      {
        OverrideMike11();
      }

      
      foreach(var c in AllCatchments.Values)
      {
        var v = c.NetInflow;
      }


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
    }

    public void Run()
    {
      LogThis("Model run started");
      Run(End);
      LogThis("Model run ended");

      if(reductioncreator!= null && reductioncreator.Include)
        reductioncreator.MakeMap(AllCatchments, EndCatchments, StateVariables, SourceModels, InternalReductionModels, MainStreamRecutionModels);

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

    public void DebugPrint()
    {
      string dir = Path.GetDirectoryName(AlldataFile.FileName);

      foreach (var s in InternalReductionModels)
        s.DebugPrint(dir, AllCatchments);

      //Get the output coordinate system
      ProjNet.CoordinateSystems.ICoordinateSystem projection;
      using (System.IO.StreamReader sr = new System.IO.StreamReader(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Default.prj")))
      {
        ProjNet.CoordinateSystems.CoordinateSystemFactory cs = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
        projection = cs.CreateFromWkt(sr.ReadToEnd());
      }
      using (ShapeWriter sw = new ShapeWriter(Path.Combine(dir, "DebugMap")) { Projection = projection })
      {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID", typeof(int));
        dt.Columns.Add("LakeArea", typeof(double));
        dt.Columns.Add("M11Input", typeof(double));

        foreach (var v in AllCatchments.Values)
        {
          GeoRefData gd = new GeoRefData() { Geometry = v.Geometry, Data = dt.NewRow() };
          gd.Data[0] = v.ID;
          double lakearea = v.Lakes.Sum(l => l.Geometry.GetArea());
          if (v.BigLake != null) //Add the big lake
            lakearea += v.BigLake.Geometry.GetArea();

          gd.Data[1] = lakearea;
          if (v.NetInflow != null)
            gd.Data[2] = v.NetInflow.GetTs(TimeStepUnit.Month).Average / v.Geometry.GetArea() * 100 * 86400;
          sw.Write(gd);
        }
      }
    }


    public void Print()
    {
      //Get the output coordinate system
      ProjNet.CoordinateSystems.ICoordinateSystem projection;
      using (System.IO.StreamReader sr = new System.IO.StreamReader(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Default.prj")))
      {
        ProjNet.CoordinateSystems.CoordinateSystemFactory cs = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
        projection = cs.CreateFromWkt(sr.ReadToEnd());
      }

      LogThis("Writing output");
      if (AlldataFile != null)
        StateVariables.ToCSV(AlldataFile.FileName);


      if (ExcelTemplate != null)
      {
        Accumulated.ToExcelTemplate(ExcelTemplate.FileName, ExcelTemplate.ColumnNames[0]);
      }


      foreach (var detailed in DetailedParameterTimeSeries)
      {
        if (detailed.Flags.First())
          Accumulated.ToCSV(detailed.ColumnNames.First(), detailed.FileName);
        else
          StateVariables.ToCSV(detailed.ColumnNames.First(), detailed.FileName);
      }

      foreach (var detailed in DetailedCatchmentTimeSeries)
      {
        if (detailed.Flags.First())
          Accumulated.ToCSV((int)detailed.Parameters.First(), detailed.FileName);
        else
          StateVariables.ToCSV((int)detailed.Parameters.First(), detailed.FileName);
      }

      foreach (var statmap in StatisticsMap)
      {
        var Ctime = new DateTime((int)statmap.Parameters[0], (int)statmap.Parameters[2], 1);
        var sumend = new DateTime((int)statmap.Parameters[1], (int)statmap.Parameters[3], 1);

        var sim = StateVariables.ExtractTimeSeries("DownStreamOutput");
        var obs = StateVariables.ExtractTimeSeries("ObservedNitrate");

        using (ShapeWriter sw = new ShapeWriter(statmap.FileName) { Projection = projection })
        {
          DataTable data = new DataTable();

          data.Columns.Add("ID15", typeof(int));
          data.Columns.Add("ME", typeof(double));
          data.Columns.Add("MAE", typeof(double));
          data.Columns.Add("RMSE", typeof(double));
          data.Columns.Add("FBAL", typeof(double));
          data.Columns.Add("R2", typeof(double));
          data.Columns.Add("bR2", typeof(double));
          data.Columns.Add("NValues", typeof(int));

          foreach (var kvp in obs)
          {
            ZoomTimeSeries obsr = new ZoomTimeSeries() { Accumulate = true };
            obsr.GetTs(TimeStepUnit.Month).AddRange(Ctime, kvp.Value.GetValues(Ctime, sumend));

            ZoomTimeSeries simr = new ZoomTimeSeries() { Accumulate = true };
            simr.GetTs(TimeStepUnit.Month).AddRange(Ctime, sim[kvp.Key].GetValues(Ctime, sumend));

            FixedTimeStepSeries obsreduced;
            FixedTimeStepSeries simreduced;
            if (statmap.Flags[0])
            {
              obsreduced = obsr.GetTs(TimeStepUnit.Year);
              simreduced = simr.GetTs(TimeStepUnit.Year);
            }
            else
            {
              obsreduced = obsr.GetTs(TimeStepUnit.Month);
              simreduced = simr.GetTs(TimeStepUnit.Month);
            }

            double? me = obsreduced.ME(simreduced);
            if (me.HasValue)
            {
              GeoRefData gd = new GeoRefData() { Geometry = AllCatchments[kvp.Key].Geometry };
              gd.Data = data.NewRow();
              gd.Data[0] = kvp.Key;
              gd.Data[1] = me;
              gd.Data[2] = obsreduced.MAE(simreduced);
              gd.Data[3] = obsreduced.RMSE(simreduced);
              gd.Data[4] = obsreduced.FBAL(simreduced);
              gd.Data[5] = obsreduced.R2(simreduced);
              var bR2 =obsreduced.bR2(simreduced);
              if (bR2.HasValue)
                gd.Data[6] = bR2;
              else
                gd.Data[6] = DBNull.Value;
              gd.Data[7] = obsreduced.CommonCount(simreduced);
              sw.Write(gd);
            }
          }
        }

      }



      foreach (var mapout in MapOutputFiles)
      {
        using (ShapeWriter sw = new ShapeWriter(mapout.FileName) { Projection = projection })
        {
          DataTable data;
          if (mapout.Flags[1])
            data = Accumulated;
          else
            data = StateVariables;

          foreach (var c in AllCatchments.Values)
          {
            GeoRefData gd = new GeoRefData() { Geometry = c.Geometry };
            gd.Data = data.NewRow();

            gd.Data[0] = c.ID;

            var Ctime = new DateTime((int)mapout.Parameters[0], (int)mapout.Parameters[2], 1);
            var sumend = new DateTime((int)mapout.Parameters[1], (int)mapout.Parameters[3], 1);
            for (int k = 4; k < data.Columns.Count; k++)
              gd.Data[k] = 0;

            while (Ctime < sumend)
            {
              var row = data.Rows.Find(new object[] { c.ID, Ctime });
              for (int k = 4; k < data.Columns.Count; k++)
                if (!row.IsNull(k) & data.Columns[k].DataType == typeof(double))
                  gd.Data[k] = (double)gd.Data[k] + (double)row[k];
              Ctime = Ctime.AddMonths(1);
            }
            if (mapout.Flags[0])
            {
              for (int k = 6; k < data.Columns.Count; k++)
                if (!gd.Data.IsNull(k))
                  gd.Data[k] = (double)gd.Data[k] / c.Geometry.GetArea();
            }
            sw.Write(gd);
          }
        }
      }
    }


    


    private object Lock= new object();
    private static object staticLock= new object();

   #region Private methods

    private void OverrideMike11()
    {
      using (HydroNumerics.MikeSheTools.DFS.DFS0 mflow = new MikeSheTools.DFS.DFS0(M11FlowOverride.FileName))
      {
        foreach (var i in mflow.Items)
        {
          Catchment ca;
          if (AllCatchments.TryGetValue(int.Parse(i.Name), out ca))
          {
            var ts = mflow.GetTimeSpanSeries(i.ItemNumber);
            if (ts.Items.Count != 7680)
            {
              int k = 0;
            }
            ca.M11Flow = new ZoomTimeSeries();
            ca.M11Flow.GetTs(TimeStepUnit.Day).AddRange(ts.StartTime, ts.Items.Select(v => v.Value));
          }
        }
      }

    }


    private DataTable Accumulate()
    {
      var accumulated = StateVariables.Copy();
      accumulated.Columns.Add("IsAccumulated", typeof(bool));
      for (int i = 0; i < accumulated.Rows.Count;i++ )
        accumulated.Rows[i]["IsAccumulated"] = false;


      CurrentTime = Start;
      while (CurrentTime < End)
      {
        foreach (var c in EndCatchments)
        {
          c.Accumulate(accumulated, CurrentTime);
        }
        CurrentTime = CurrentTime.AddMonths(1);
      }

      var v = accumulated.Rows.Find(new object[] { 16100009, Start });
      return accumulated;
    }


    private DataTable _accumulated;
    private DataTable Accumulated
    {
      get
      {
        if (_accumulated == null)
          _accumulated = Accumulate();
        return _accumulated;

      }
    }


    public void LoadCoastalZone()
    {
      if (CoastalZone == null)
        return;

      LogThis("Reading shape coastal zone");
      List<IXYPolygon> CutPolygons = new List<IXYPolygon>(); 
      using (ShapeReader sr = new ShapeReader(CoastalZone.FileName))
      {
        foreach (var pol in sr.GeoData)
        {
          if(CoastalZone.ColumnNames.Skip(1).Contains(pol.Data[CoastalZone.ColumnNames.First()].ToString().ToLower().Trim()))
            CutPolygons.Add((IXYPolygon)pol.Geometry);
        }
     }
      LogThis("Distributing coastal zone");
      Parallel.ForEach(EndCatchments, l =>
        {
          foreach (var c in CutPolygons)
            if (l.Geometry.OverLaps(c))
            {
              lock (Lock)
                l.CoastalZones.Add(c);
            }
        });

    }


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
            l.BigLakeID = (int) ldata.Data["Rec"];
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
            if (c.Geometry.Contains(l.Geometry.GetX(0),l.Geometry.GetY(0)))
            {
              lock (Lock)
                c.Lakes.Add(l);
              break;
            }
        });

 
      Parallel.ForEach(lakes.Where(la => la.HasDischarge & !la.IsSmallLake), l =>
      {
        foreach (var c in AllCatchments.Values)
          if (c.Geometry.Contains(l.Geometry.Points.Average(p=>p.X),l.Geometry.Points.Average(p=>p.Y))) //Centroid
          {
            lock (Lock)
            {
              if (c.BigLake==null || c.BigLake.Geometry.GetArea()<l.Geometry.GetArea()) //Add the lake if it is bigger
                c.BigLake = l;
            }
            break;
          }
      });


      LogThis(lakes.Count(la => la.HasDischarge & la.IsSmallLake) + " lakes distributed on " + AllCatchments.Values.Count(c => c.Lakes.Count > 0) + " catchments");
      LogThis(AllCatchments.Values.Count(c => c.BigLake != null).ToString() + " catchments have large lakes");
    }


    public void LoadStationData(string ShapeFileName, string StationData)
    {

      Dictionary<int, DMUStation> locatedStations = new Dictionary<int,DMUStation>();
      List<DMUStation> stations = new List<DMUStation>();
      LogThis("Reading stations from " + ShapeFileName);
      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          DMUStation dm = new DMUStation();
          dm.Location = sr.ReadNext() as XYPoint;
          dm.ID = sr.Data.ReadInt(i, "Dmunr");
          dm.ODANummer = sr.Data.ReadInt(i, "ODA_nr");
          stations.Add(dm);
          if(dm.ODANummer!=0)
            locatedStations.Add(dm.ODANummer, dm);
          int id =sr.Data.ReadInt(i, "ID15");
          if (id != 0 & AllCatchments.ContainsKey(id))
          {
            if (AllCatchments[id].Measurements != null)
            {
              int m = 0;
            }
            AllCatchments[id].Measurements = dm;
          }
        }
      }
      LogThis(stations.Count + " stations read. " + locatedStations.Count + " within catchments distributed on " + AllCatchments.Values.Count(ca => ca.Measurements != null) +" catchments.");

      using (StreamReader sr = new StreamReader(StationData))
      {
        sr.ReadLine();//HeadLine
        while (!sr.EndOfStream)
        {
          var data = sr.ReadLine().Split(';');
          DMUStation  station;

          if (locatedStations.TryGetValue(int.Parse(data[0]), out station))
          {
            var time = new DateTime(int.Parse(data[2]), int.Parse(data[3]), 1);
            station.Nitrate.Items.Add(new TimeStampValue(time, double.Parse(data[4])));
            station.Flow.Items.Add(new TimeStampValue(time, double.Parse(data[5])*1000));
          }
        }
      }
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
        //We find all the detailed timeseries that starts with the catchment ID.
        //If there are more than one they ar summed
        var flows = m11.Where(mm=>mm.Name.Substring(0, Math.Min(mm.Name.Length, c.ID.ToString().Length))== c.ID.ToString());
        List<double> flowdata=null;
        foreach(var flow in flows)
        {
          if (flowdata == null)
            flowdata = new List<double>(flow.Simulation.Items.Select(v => Math.Abs(v.Value))); //Careful we are now removing negative flows
          else
            for (int i = 0; i < flowdata.Count; i++)
              flowdata[i] += flow.Simulation.Items[i].Value;
        }
        if (flowdata != null)
        {
          c.M11Flow = new ZoomTimeSeries();
          c.M11Flow.GetTs(TimeStepUnit.Day).AddRange(flows.First().Simulation.StartTime, flowdata);
        }
      }
      LogThis(AllCatchments.Values.Count(c=>c.M11Flow!=null) + " catchments now have m11 flow");

      LogThis("Distributing precipitation");
      var precip = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.PrecipitationRate.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);
      foreach (var c in GetValuesFromGrid(precip, AllCatchments.Values.Where(c => c.Precipitation == null).ToList(), true, Start, End))
      {
        AllCatchments[c.Key].Precipitation = new ZoomTimeSeries() { Accumulate = true };
        AllCatchments[c.Key].Precipitation.GetTs(TimeStepUnit.Month).AddRange(c.Value.StartTime, c.Value.Items.Select(t => t.Value));
      }
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

    #region Static properties

    private static SafeFile SoilsShape;

    private static List<GeoRefData> _soilTypes;
    public static List<GeoRefData> SoilTypes
    {
      get
      {
        lock (staticLock)
        {
          if (_soilTypes == null)
          {
            if (SoilsShape != null)
            {
              using (ShapeReader sr = new ShapeReader(SoilsShape.FileName))
              {
                _soilTypes = new List<GeoRefData>(sr.GeoData);
              }
            }
          }
          for (int i = _soilTypes.First().Data.Table.Columns.Count-1; i > 0; i--)
          {
            if (_soilTypes.First().Data.Table.Columns[i].ColumnName != SoilsShape.ColumnNames.First())
              _soilTypes.First().Data.Table.Columns.RemoveAt(i);
          }
        }
        return _soilTypes;
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
        using (StreamWriter sw = new StreamWriter(LogFileName, true, Encoding.Default))
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

      Dictionary<int, TimeStampSeries> ToReturn = new Dictionary<int, TimeStampSeries>();

      int firsttimestep = precip.GetTimeStep(Start);
      int lasttimestep = precip.GetTimeStep(End);

        for (int i = firsttimestep; i <= lasttimestep; i++)
        {
          precipdata = precip.GetData(i, 1);
          for (int m = 0; m < p.Count; m++)
          {
            p[m].Item4.Items.Add(new TimeStampValue(precip.TimeSteps[i], precipdata[p[m].Item3, p[m].Item2]));
          }
        }

        for (int m = 0; m < p.Count; m++)
        {
          ToReturn.Add(p[m].Item1, TSTools.ChangeZoomLevel(p[m].Item4, TimeStepUnit.Month, Accumulate));
          p[m] = null;
        }

      return ToReturn;
    }


    #endregion


  
  
  }
}

