using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

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
    SoilCodesGrid DaisyCodes;

    public MainViewModel()
    {
      CurrentCatchments = new ObservableCollection<Catchment>();
    }

    public MainViewModel(string xmlfilename):this()
    {
      var configuration = XDocument.Load(xmlfilename);


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

    DistributedLeaching leachdata;


    public void LoadDaisyData(string DaisyResultsFileName)
    {
      if (leachdata == null)
        leachdata = new DistributedLeaching();
      leachdata.LoadFile(DaisyResultsFileName);
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



    public void LoadSoilCodesGrid(string ShapeFileName)
    {
      DaisyCodes = new SoilCodesGrid(); 
      DaisyCodes.BuildGrid(ShapeFileName);
   }

    public List<Particle> Particles { get; set; }

    public void LoadParticles(string ShapeFileName)
    {
      Particles = new List<Particle>();
      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          double x = sr.Data.ReadDouble(i, "X-Reg");
          double y = sr.Data.ReadDouble(i, "Y-Reg");

          Particle p = new Particle();
          IXYPoint point = (IXYPoint)sr.ReadNext();
          p.XStart = point.X;
          p.YStart = point.Y;
          p.X = x;
          p.Y = y;
//          p.StartXGrid = sr.Data.ReadInt(i, "IX-Birth");
//          p.StartYGrid = sr.Data.ReadInt(i, "IY-Birth");
          p.TravelTime = sr.Data.ReadDouble(i, "TravelTime");
          Particles.Add(p);
        }
      }
    }


    /// <summary>
    /// Gets the groundwater concentration for each catchment using the particles and the Daisy output
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="NumberOfParticlesPrGrid"></param>
    public void BuildInputConcentration(DateTime Start, DateTime End, int NumberOfParticlesPrGrid)
    {
      int numberofmonths = (End.Year - Start.Year) * 12 + End.Month - Start.Month;

      Parallel.ForEach(AllCatchments.Values.Where(ca => ca.Particles.Count > 0), new ParallelOptions() { MaxDegreeOfParallelism = 7 }, c =>
        {
          List<float> values = new List<float>();
          for (int i = 0; i < numberofmonths; i++)
            values.Add(0);

          foreach (var p in c.Particles)
            {
              int gridid = DaisyCodes.GetID(p.XStart, p.YStart);
              var newlist = leachdata.Grids[gridid].GetValues(Start, End);
              for (int i = 0; i < numberofmonths; i++)
                values[i]+= newlist[i];
            }
          for (int i =0;i<numberofmonths;i++)
            c.GWInput.Items.Add(new TimeSpanValue(Start.AddMonths(i), Start.AddMonths(i), values[i] / NumberOfParticlesPrGrid));
          });
    }

    private object Lock = new object();

    public void CombineParticlesAndCatchments()
    {

      var bb = HydroNumerics.Geometry.XYGeometryTools.BoundingBox(Particles);

      var selectedCatchments = AllCatchments.Values.Where(c => bb.OverLaps(c.Geometry)).ToArray();

      Parallel.ForEach(Particles, new ParallelOptions() { MaxDegreeOfParallelism = 7 },
        (p) =>
        {
          foreach (var c in selectedCatchments)
          {
            if (c.Geometry.Contains(p.X, p.Y))
            {
              lock(Lock)
                c.Particles.Add(p);
              break;
            }
          }
        });
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

        foreach (var c in sr.GeoData)
        {
          Catchment ca = new Catchment((int)c.Data[0]);
          if (!AllCatchments.ContainsKey(ca.ID))
            AllCatchments.Add(ca.ID, ca);

          ca.Geometry = (XYPolygon) c.Geometry;
        }

        foreach (var c in sr.GeoData)
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

