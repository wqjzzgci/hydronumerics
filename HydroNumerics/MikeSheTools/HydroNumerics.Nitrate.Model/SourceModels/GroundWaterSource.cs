using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.Time2;

namespace HydroNumerics.Nitrate.Model
{
  public class GroundWaterSource : BaseModel, ISource
  {
    private Dictionary<int, float[]> GWInput = new Dictionary<int, float[]>();

    private object Lock = new object();


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
        foreach (var parfile in Configuration.Element("DaisyFiles").Elements("DaisyFile"))
        {
          DaisyFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("FileName") });
        }
        foreach (var parfile in Configuration.Element("ParticleFiles").Elements("ParticleFile"))
        {
          ParticleFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("ShapeFileName") });
          ParticleFiles.Last().Parameters.Add(parfile.SafeParseInt("NumberOfParticlesInGridBlock")??100);
          ParticleFiles.Last().Flags.Add(parfile.SafeParseBool("RemoveRedox") ?? true);
        }
        WriteBreakthroughCurves = Configuration.SafeParseBool("WriteBreakthroughCurves")??false;
        SoilCodes = new SafeFile() { FileName = Configuration.Element("SoilCodes").SafeParseString("ShapeFileName") };
        BTCMap = new SafeFile() { CheckIfFileExists = false, FileName = Configuration.SafeParseString("BreakThroughMap") };
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
      this.Start = Start;
      LoadSoilCodesGrid(SoilCodes.FileName);

      foreach (var parfile in DaisyFiles)
      {
        LoadDaisyData(parfile.FileName);
      }

      leachdata.BuildLeachData(Start, End, Catchments);

      foreach (var parfile in ParticleFiles)
      {
        var particles = LoadParticles(parfile.FileName, parfile.Flags.First());
        CombineParticlesAndCatchments(Catchments, particles);
        int NumberOfParticles = (int)parfile.Parameters.First();

        if (WriteBreakthroughCurves)
        {
          NewMessage("Writing breakthrough curves");
          using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(Path.GetDirectoryName(parfile.FileName), "BC.csv")))
          {
            double np = 20.0;

            StringBuilder headline = new StringBuilder();
            headline.Append("ID\tNumber of Particles");

            for (int i = 1; i < np; i++)
            {
              headline.Append("\t + " + (i / np * 100.0));
            }
            sw.WriteLine(headline);


            foreach (var c in Catchments.Where(ca => ca.Particles.Count >= 20))
            {
              c.ParticleBreakthroughCurves = new List<Tuple<double, double>>();
              MathNet.Numerics.Statistics.Percentile p = new MathNet.Numerics.Statistics.Percentile(c.Particles.Select(pa => pa.TravelTime));
              StringBuilder line = new StringBuilder();
              line.Append(c.ID + "\t" + c.Particles.Count);
              for (int i = 1; i < np; i++)
              {
                line.Append("\t" + p.Compute(i / np));
                c.ParticleBreakthroughCurves.Add(new Tuple<double, double>(i / np * 100.0, p.Compute(i / np)));
              }
              sw.WriteLine(line);
            }
          }
        }

        BuildInputConcentration(Start, End, Catchments, NumberOfParticles);
      }

      leachdata.ClearMemory();


      if (BTCMap != null)
      {
        using (ShapeWriter sw = new ShapeWriter(BTCMap.FileName))
        {

          System.Data.DataTable dt = new System.Data.DataTable();

          foreach (var bc in Catchments.First(ca => ca.ParticleBreakthroughCurves != null).ParticleBreakthroughCurves)
          {
            dt.Columns.Add(bc.Item1.ToString(), typeof(double));
          }

          foreach (var c in Catchments.Where(ca => ca.ParticleBreakthroughCurves != null))
          {
            GeoRefData gd = new GeoRefData() { Geometry = c.Geometry };
            var row = dt.NewRow();

            foreach (var bc in c.ParticleBreakthroughCurves)
              row[bc.Item1.ToString()] = bc.Item2;

            gd.Data = row;
            sw.Write(gd);
          }
        }
      }
    }


    /// <summary>
    /// Returns the source rate to the catchment in kg/s at the current time
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      double red = 0;
      if (GWInput.ContainsKey(c.ID))
        red= GWInput[c.ID][(CurrentTime.Year - Start.Year) * 12 + CurrentTime.Month - Start.Month];
      return red*MultiplicationPar + AdditionPar;
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
    /// Reads the particles
    /// </summary>
    /// <param name="ShapeFileName"></param>
    /// <returns></returns>
    public IEnumerable<Particle> LoadParticles(string ShapeFileName, bool RemoveRedox)
    {
      NewMessage("Reading particles from: " + ShapeFileName);
      List<int> RedoxedParticles = new List<int>();
      Dictionary<int, Particle> NonRedoxedParticles = new Dictionary<int, Particle>();

      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          int id = sr.Data.ReadInt(i, "ID");
          double x = sr.Data.ReadDouble(i, "X-Reg");
          double y = sr.Data.ReadDouble(i, "Y-Reg");

          Particle p = new Particle();
          IXYPoint point = (IXYPoint)sr.ReadNext();
          p.XStart = point.X;
          p.YStart = point.Y;
          p.X = x;
          p.Y = y;
          p.TravelTime = sr.Data.ReadDouble(i, "TravelTime");


          int reg = sr.Data.ReadInt(i, "Registrati");
          if (reg == 1)
            RedoxedParticles.Add(id);
          else
            NonRedoxedParticles.Add(id, p);
        }

        if(RemoveRedox)
          foreach (var pid in RedoxedParticles)
            NonRedoxedParticles.Remove(pid);

        NewMessage(NonRedoxedParticles.Values.Count + " particles read");
        return NonRedoxedParticles.Values;
      }
    }

    /// <summary>
    /// Gets the groundwater concentration for each catchment using the particles and the Daisy output
    /// How much get into the catchment at a particular time step
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="NumberOfParticlesPrGrid"></param>
    public void BuildInputConcentration(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments, int NumberOfParticlesPrGrid)
    {
      NewMessage("Getting input for each catchment from daisy file");
      int numberofmonths = (End.Year - Start.Year) * 12 + End.Month - Start.Month;
     
      Parallel.ForEach(Catchments.Where(ca => ca.Particles.Count > 0),  c =>
        {
          float[] values;
          if (!GWInput.TryGetValue(c.ID, out values))
          {
            values = new float[numberofmonths];
            values.Initialize();
            lock(Lock)
              GWInput.Add(c.ID, values);
          }
          foreach (var p in c.Particles)
          {
            var newlist = leachdata.GetValues(p.XStart, p.YStart,Start.AddDays(-p.TravelTime * 365), End.AddDays(-p.TravelTime * 365));
            if(newlist !=null)
            for (int i = 0; i < numberofmonths; i++)
              values[i] += newlist[i] / NumberOfParticlesPrGrid;
          }
          c.Particles.Clear();
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
                c.Particles.Add(p);
              break;
            }
          }
        });
    }

#endregion

    #region Properties

    private SafeFile _BTCMap;
    public SafeFile BTCMap
    {
      get { return _BTCMap; }
      set
      {
        if (_BTCMap != value)
        {
          _BTCMap = value;
          NotifyPropertyChanged("BTCMap");
        }
      }
    }
    
    
    private DistributedLeaching _leachdata= new DistributedLeaching();
    public DistributedLeaching leachdata
    {
      get { return _leachdata; }
      set
      {
        if (_leachdata != value)
        {
          _leachdata = value;
          NotifyPropertyChanged("leachdata");
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
          NotifyPropertyChanged("SoilCodes");
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
          NotifyPropertyChanged("ParticleFiles");
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
          NotifyPropertyChanged("DaisyFiles");
        }
      }
    }

    private bool _WriteBreakthroughCurves=false;
    public bool WriteBreakthroughCurves
    {
      get { return _WriteBreakthroughCurves; }
      set
      {
        if (_WriteBreakthroughCurves != value)
        {
          _WriteBreakthroughCurves = value;
          NotifyPropertyChanged("WriteBreakthroughCurves");
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
          NotifyPropertyChanged("Start");
        }
      }
    }

    #endregion

  }
}
