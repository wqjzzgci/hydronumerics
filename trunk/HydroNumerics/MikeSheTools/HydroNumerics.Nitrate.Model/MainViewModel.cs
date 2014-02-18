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

    public ObservableCollection<Catchment> EndCatchments { get; private set; }
    public Dictionary<int, Catchment> AllCatchments { get; private set; }
    public ObservableCollection<Catchment> CurrentCatchments { get; private set; }
    private DataTable StateVariables;
    List<ISource> SourceModels;
    List<IReductionModel> InternalReductionModels;
    List<IReductionModel> MainStreamRecutionModels;

    XElement configuration;

    public MainViewModel()
    {
      CurrentCatchments = new ObservableCollection<Catchment>();
    }

    public MainViewModel(string xmlfilename):this()
    {
      configuration = XDocument.Load(xmlfilename).Element("Configuration");

      var startxml =configuration.Element("SimulationStart");
      var endxml = configuration.Element("SimulationEnd");
      Start = new DateTime(int.Parse(startxml.Attribute("Year").Value), int.Parse(startxml.Attribute("Month").Value), 1);
      End = new DateTime(int.Parse(endxml.Attribute("Year").Value), int.Parse(endxml.Attribute("Month").Value), 1);
      CurrentTime = Start;

      StateVariables = new DataTable();
      StateVariables.Columns.Add("ID", typeof(int));
      StateVariables.Columns.Add("Time", typeof(DateTime));
      StateVariables.Columns.Add("DownStreamOutput", typeof(double));

      
      StateVariables.PrimaryKey = new DataColumn[] { StateVariables.Columns[0], StateVariables.Columns[1] };

      //Configuration of sourcemodels
      SourceModels = new List<ISource>();
      foreach (var sourcemodelXML in configuration.Element("SourceModels").Elements())
      {
        ISource NewModel=null;
        switch (sourcemodelXML.Name.LocalName)
        {
          case "Atmospheric":
            NewModel = new AtmosphericDeposition(sourcemodelXML);
            break;
          case "GroundwaterSource":
            NewModel = new GroundWaterSource(sourcemodelXML);
            break;
          case "PointSource":
            NewModel = new PointSource(sourcemodelXML);
            break;
          case"OrganicN":
            NewModel = new OrganicN(sourcemodelXML);
            break;
        }
        if (NewModel != null)
        {
          SourceModels.Add(NewModel);
          StateVariables.Columns.Add(NewModel.Name, typeof(double));
        }
      }


      InternalReductionModels = new List<IReductionModel>();
      //Configuration of internal reduction models
      foreach (var sourcemodelXML in configuration.Element("InternalReductionModels").Elements())
      {
        IReductionModel NewModel = null;
        switch (sourcemodelXML.Name.LocalName)
        {
          case "InternalLake":
            NewModel = new InternalLakeReduction(sourcemodelXML);
            break;
          case "StreamReduction":
            NewModel = new StreamReduction(sourcemodelXML);
            break;
        }
        if (NewModel != null)
        {
          InternalReductionModels.Add(NewModel);
          StateVariables.Columns.Add(NewModel.Name, typeof(double));
        }
      }

      MainStreamRecutionModels = new List<IReductionModel>();
      //Configuration of internal reduction models
      foreach (var sourcemodelXML in configuration.Element("MainStreamRecutionModels").Elements())
      {
        IReductionModel NewModel = null;
        switch (sourcemodelXML.Name.LocalName)
        {
          case "StreamReduction":
            NewModel = new StreamReduction(sourcemodelXML);
            break;
        }
        if (NewModel != null)
        {
          MainStreamRecutionModels.Add(NewModel);
          StateVariables.Columns.Add(NewModel.Name, typeof(double));
        }
      }

      LoadCatchments(configuration.Element("ID15ShapeFile").Value);

      foreach (var c in AllCatchments.Values)
      {
        c.SourceModels = SourceModels;
        c.InternalReduction = InternalReductionModels;
        c.MainStreamReduction = MainStreamRecutionModels;
        
        c.StateVariables = StateVariables;
      }
    }


    public void Initialize()
    {
      foreach (var mshe in configuration.Elements("MikeSheFiles"))
        LoadMikeSheData(mshe.Value);

      foreach (var m in SourceModels)
      {
        if (m.Update)
          m.Initialize(Start, End, AllCatchments.Values);
      }

      foreach (var m in InternalReductionModels)
      {
        if (m.Update)
          m.Initialize(Start, End, AllCatchments.Values);
      }

      foreach (var m in MainStreamRecutionModels)
      {
        if (m.Update)
          m.Initialize(Start, End, AllCatchments.Values);
      }
    
    }

    public void Run()
    {
      Initialize();
      Run(End);
    }

    public void Print(string Filename)
    {
      StateVariables.ToCSV(Filename);
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


    public void LoadMikeSheData(string SheFile)
    {
      MikeSheTools.Core.Model m = new MikeSheTools.Core.Model(SheFile);

      var m11 = m.Results.Mike11Observations.Where(mm=>mm.Simulation!=null).ToList();
      foreach (var c in AllCatchments.Values)
      {
        var flow = m11.FirstOrDefault(mm=>mm.Name== c.ID.ToString());
        if (flow != null)
          c.M11Flow = Time2.TSTools.ChangeZoomLevel(flow.Simulation, TimeStepUnit.Month, true);
      }

      var precip = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.PrecipitationRate.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);
      foreach (var c in GetValuesFromGrid(precip, true))
        AllCatchments[c.Key].Precipitation = c.Value; 
      precip.Dispose();

      var temperature = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.AirTemperature.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);
      foreach (var t in GetValuesFromGrid(temperature, false))
        AllCatchments[t.Key].Temperature = t.Value;
      temperature.Dispose();

      m.Dispose();
    }


    private Dictionary<int, TimeStampSeries> GetValuesFromGrid(HydroNumerics.MikeSheTools.DFS.DFS2 precip, bool Accumulate)
    {
      var polygons = XYPolygon.GetPolygons(precip);

      List<Tuple<int, int, int, TimeStampSeries>> p = new List<Tuple<int, int, int, TimeStampSeries>>();
      List<Catchment> CatcmentsToTest = AllCatchments.Values.ToList();

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

      int split = p.Count / 2;

      for (int i = 0; i < precip.NumberOfTimeSteps; i++)
      {
        precipdata = precip.GetData(i, 1);
        foreach (var c in p.Take(split))
        {
          c.Item4.Items.Add(new TimeStampValue(precip.TimeSteps[i], precipdata[c.Item3, c.Item2]));
        }
      }

      Dictionary<int, TimeStampSeries> ToReturn = new Dictionary<int, TimeStampSeries>();
      foreach (var c in p.Take(split))
      {
        ToReturn.Add(c.Item1, TSTools.ChangeZoomLevel(c.Item4, TimeStepUnit.Month, true));
      }

      for (int i = 0; i < precip.NumberOfTimeSteps; i++)
      {
        precipdata = precip.GetData(i, 1);
        foreach (var c in p.Skip(split))
        {
          c.Item4.Items.Add(new TimeStampValue(precip.TimeSteps[i], precipdata[c.Item3, c.Item2]));
        }
      }

      foreach (var c in p.Skip(split))
      {
        ToReturn.Add(c.Item1, TSTools.ChangeZoomLevel(c.Item4, TimeStepUnit.Month, Accumulate));
      }

      return ToReturn;
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



        EndCatchments  = new ObservableCollection<Catchment>(AllCatchments.Values.Where(c => c.DownstreamConnection == null));
      }
    }

  }
}

