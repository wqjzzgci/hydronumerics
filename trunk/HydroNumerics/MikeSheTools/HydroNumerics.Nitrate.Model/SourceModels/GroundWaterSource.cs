using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MathNet.Numerics.LinearAlgebra.Double;


using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.Core.Time;

namespace HydroNumerics.Nitrate.Model
{
  public class GroundWaterSource : BaseModel, ISource
  {
    private Dictionary<int, float[]> GWInput = new Dictionary<int, float[]>();

    private object Lock = new object();





    private DateTime RecycleStart = DateTime.MinValue;
    private DateTime RecycleEnd = DateTime.MinValue;
    private double RecycleScale =1f;


    public GroundWaterSource()
    {
    }


#region Implementation of InitrateModel and ISource 
   
    /// <summary>
    /// Reads and parses the configuration element
    /// </summary>
    /// <param name="Configuration"></param>
    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);
      if (Update)
      {
        var Unsatelement = Configuration.Element("UnsatFiles");
        if (Unsatelement != null)
        {
          foreach (var parfile in Unsatelement.Elements("UnsatFile"))
          {
            UZAgeDefinition uzd = new UZAgeDefinition();
            uzd.FileName = new SafeFile() { FileName = parfile.SafeParseString("DFS2FileName") };
            uzd.FileName.ColumnNames.Add(parfile.SafeParseString("ItemName"));
            uzd.MinHorizontalTravelDistance =parfile.SafeParseDouble("MinHorizontalTravelDistance") ?? 0;
            uzd.MinTravelTime = parfile.SafeParseDouble("MinTravelTime") ?? 0;

            UnsatAgeFiles.Add(uzd);
          }
        }
        
        var daisyelement =Configuration.Element("DaisyFiles");

        var startyear = daisyelement.SafeParseInt("RecycleStartYear");
        int startmonth = daisyelement.SafeParseInt("RecycleStartMonth") ??1;

        if (startyear.HasValue)
          RecycleStart = new DateTime(startyear.Value, startmonth, 1);

        var endyear = daisyelement.SafeParseInt("RecycleEndYear");
        int endmonth = daisyelement.SafeParseInt("RecycleEndMonth") ?? 1;
        if (endyear.HasValue)
          RecycleEnd = new DateTime(endyear.Value, endmonth, 1);
        RecycleScale = daisyelement.SafeParseDouble("RecycleScaleFactor") ?? 1;

        foreach (var parfile in daisyelement.Elements("DaisyFile"))
        {
          DaisyFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("FileName") });
        }

        var ParticleFiles =Configuration.Element("ParticleFiles");
        UseUnsatFilter = ParticleFiles.SafeParseBool("RemoveUnsatParticles") ?? false;

        foreach (var parfile in ParticleFiles.Elements("ParticleFile"))
        {
          ParticleFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("ShapeFileName") });
        }

        SoilCodes = new SafeFile() { FileName = Configuration.Element("SoilCodes").SafeParseString("ShapeFileName") };
      }
    }


    /// <summary>
    /// Initializes the model by reading all files and building the leaching time series to and from each catchment
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="Catchments"></param>
    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      base.Initialize(Start, End, Catchments);

      this.Start = Start;
      LoadSoilCodesGrid(SoilCodes.FileName);

        foreach (var item in UnsatAgeFiles)
        {
          item.Initialize();
        }

      foreach (var parfile in DaisyFiles)
      {
        LoadDaisyData(parfile.FileName);
      }

      leachdata.BuildLeachData(Start, End, Catchments, RecycleStart, RecycleEnd, RecycleScale);

      ParticleReader pr = new ParticleReader();
      pr.Catchments = Catchments;

      List<Particle> AllParticles = new List<Particle>();

      IEnumerable<Particle> particles;

      foreach (var parfile in ParticleFiles)
      {
        if(UseUnsatFilter)  
          particles = pr.ReadParticleFile(parfile.FileName, pr.UnSatFilter);
        else
          particles = pr.ReadParticleFile(parfile.FileName, null);

        AllParticles.AddRange(particles);


        if (ExtraOutput)
          pr.Distribute(particles.Where(p => p.SinkType != SinkType.Removed_by_Well));
        else
          pr.Distribute(particles.Where(p => p.SinkType != SinkType.Removed_by_Well &p.Registration != 1));
      }

      //Now scale the leaching data with the number of particles in each grid cell.
      leachdata.ScaleWithParticles(AllParticles);

      foreach (var p in AllParticles)
      {
        //Adjust traveltime from dfs2
        foreach (var dfs in UnsatAgeFiles)
        {
          if (p.HorizontalTravelDistance >= dfs.MinHorizontalTravelDistance & p.TravelTime >= dfs.MinTravelTime)
          {
            int col = dfs.dfs.GetColumnIndex(p.X);
            int row = dfs.dfs.GetRowIndex(p.Y);

            if (col >= 0 & row >= 0)
              p.TravelTime += Math.Max(0, dfs.Data[row, col]);
          }
        }
      }

      BuildInputConcentration(Start, End, Catchments);

      if (ExtraOutput)
        pr.DebugPrint(MainModel.OutputDirectory);

      //Now clear memory
      leachdata.ClearMemory();

      foreach (var c in Catchments)
        c.EndParticles.Clear();
    }


    /// <summary>
    /// Returns the source rate to the catchment in kg/s at the current time
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      double value = 0;
      if (GWInput.ContainsKey(c.ID))
        value = GWInput[c.ID][(CurrentTime.Year - Start.Year) * 12 + CurrentTime.Month - Start.Month];

      value = value * MultiplicationPar + AdditionPar;

      if (MultiplicationFactors != null)
        if (MultiplicationFactors.ContainsKey(c.ID))
          value *= MultiplicationFactors[c.ID];

      if (AdditionFactors != null)
        if (AdditionFactors.ContainsKey(c.ID))
          value += AdditionFactors[c.ID];

      return value;
    }



#endregion

#region Load Methods

    /// <summary>
    /// Loads and parses a daisy file
    /// </summary>
    /// <param name="DaisyResultsFileName"></param>
    public void LoadDaisyData(string DaisyResultsFileName)
    {
      NewMessage("Loading daisy data from: " + DaisyResultsFileName);
      leachdata.LoadFileParallel(DaisyResultsFileName);
    }


    /// <summary>
    /// Loads the soil codes grid
    /// </summary>
    /// <param name="ShapeFileName"></param>
    public void LoadSoilCodesGrid(string ShapeFileName)
    {
      NewMessage("Loading soil grid codes from: " + ShapeFileName);
      leachdata.LoadSoilCodesGrid(ShapeFileName);
    }

    /// <summary>
    /// Gets the groundwater concentration for each catchment using the particles and the Daisy output
    /// How much get into the catchment at a particular time step
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="NumberOfParticlesPrGrid"></param>
    public void BuildInputConcentration(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      NewMessage("Getting input for each catchment from daisy file");
      int numberofmonths = (End.Year - Start.Year) * 12 + End.Month - Start.Month;
     
      Parallel.ForEach(Catchments.Where(ca => ca.EndParticles.Count(pp=>pp.Registration!=1) > 0),  c =>
        {
          float[] values;
          if (!GWInput.TryGetValue(c.ID, out values))
          {
            values = new float[numberofmonths];
            values.Initialize();
            lock(Lock)
              GWInput.Add(c.ID, values);
          }
          foreach (var p in c.EndParticles.Where(pp => pp.Registration != 1))
          {
            var newlist = leachdata.GetValues(p.XStart, p.YStart,Start.AddDays(-p.TravelTime * 365.0), End.AddDays(-p.TravelTime * 365.0));
            if(newlist !=null)
            for (int i = 0; i < numberofmonths; i++)
              values[i] += newlist[i];
          }
        });
    }

    /// <summary>
    /// Distributes the particles on the catchment
    /// </summary>
    /// <param name="Catchments"></param>
    /// <param name="Particles"></param>
    public void CombineParticlesAndCatchments(IEnumerable<Catchment> Catchments, IEnumerable<Particle> Particles)
    {
      NewMessage("Distributing particles on catchments");
      var bb = HydroNumerics.Geometry.XYGeometryTools.BoundingBox(Particles);

      var selectedCatchments = Catchments.Where(c => c.Geometry.OverLaps(bb)).ToArray();

      Parallel.ForEach(Particles, 
        (p) =>
        {
          foreach (var c in selectedCatchments)
          {
            if (c.Geometry.Contains(p.X, p.Y))
            {
              lock (Lock)
              {
                c.EndParticles.Add(p);
              }
              break;
            }
          }
        });
    }

#endregion

    #region Properties
    
    
    private DistributedLeaching _leachdata= new DistributedLeaching();
    public DistributedLeaching leachdata
    {
      get { return _leachdata; }
      set
      {
        if (_leachdata != value)
        {
          _leachdata = value;
          RaisePropertyChanged("leachdata");
        }
      }
    }
    

    private SafeFile _SoilCodes;
    public SafeFile SoilCodes
    {
      get { return _SoilCodes; }
      set
      {
        if (_SoilCodes != value)
        {
          _SoilCodes = value;
          RaisePropertyChanged("SoilCodes");
        }
      }
    }

    private List<UZAgeDefinition> _UnsatAgeFiles = new List<UZAgeDefinition>();
    public List<UZAgeDefinition> UnsatAgeFiles
    {
      get { return _UnsatAgeFiles; }
      set
      {
        if (_UnsatAgeFiles != value)
        {
          _UnsatAgeFiles = value;
          RaisePropertyChanged("UnsatAgeFiles");
        }
      }
    }
    

    private List<SafeFile> _ParticleFiles = new List<SafeFile>();
    public List<SafeFile> ParticleFiles
    {
      get { return _ParticleFiles; }
      set
      {
        if (_ParticleFiles != value)
        {
          _ParticleFiles = value;
          RaisePropertyChanged("ParticleFiles");
        }
      }
    }

    private List<SafeFile> _DaisyFiles= new List<SafeFile>();
    public List<SafeFile> DaisyFiles
    {
      get { return _DaisyFiles; }
      set
      {
        if (_DaisyFiles != value)
        {
          _DaisyFiles = value;
          RaisePropertyChanged("DaisyFiles");
        }
      }
    }

    


    private DateTime _Start;
    public DateTime Start
    {
      get { return _Start; }
      private set
      {
        if (_Start != value)
        {
          _Start = value;
          RaisePropertyChanged("Start");
        }
      }
    }

    private bool _UseUnsatFilter=false;
    public bool UseUnsatFilter
    {
      get { return _UseUnsatFilter; }
      set
      {
        if (_UseUnsatFilter != value)
        {
          _UseUnsatFilter = value;
          RaisePropertyChanged("UseUnsatFilter");
        }
      }
    }
    


    #endregion

  }
}
