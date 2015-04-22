﻿using System;
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
    double np = 20.0;




    private DataTable debugData;
    private DataTable DebugData
    {
      get
      {
        if (debugData == null)
        {
          debugData = new DataTable();
          debugData.Columns.Add("ID15", typeof(int));
          debugData.Columns.Add("PartCount", typeof(int));
          debugData.Columns.Add("RedoxCount", typeof(int));
          debugData.Columns.Add("RedoxRatio", typeof(double));
          debugData.Columns.Add("Drain_to_River", typeof(double));
          debugData.Columns.Add("Drain_to_Boundary", typeof(double));
          debugData.Columns.Add("Unsaturated_zone", typeof(double));
          debugData.Columns.Add("River", typeof(double));
          debugData.PrimaryKey = new DataColumn[] { debugData.Columns[0]};
        }
        return debugData;
      }
    }

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
        foreach (var parfile in Configuration.Element("ParticleFiles").Elements("ParticleFile"))
        {
          ParticleFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("ShapeFileName") });
          ParticleFiles.Last().Parameters.Add(parfile.SafeParseInt("NumberOfParticlesInGridBlock")??100);
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

      foreach (var parfile in ParticleFiles)
      {
        var particles = LoadParticles(parfile.FileName);
        if(ExtraOutput)
          CombineParticlesAndCatchments(Catchments, particles);
        else
          CombineParticlesAndCatchments(Catchments, particles.Where(p=>p.Registration!=1));

        int NumberOfParticles = (int)parfile.Parameters.First();
      }

      //Careful!!! we need to change this because the number of particles is hardcoded.
      BuildInputConcentration(Start, End, Catchments, 100);

      //We need to process data for extra output while we have the particles
      if (ExtraOutput)
      {
        foreach (var c in Catchments.Where(ca => ca.Particles.Count >= 1))
        {
          if (c.Particles.Count >= 20)
          {
            c.ParticleBreakthroughCurves = new List<Tuple<double, double>>();
            MathNet.Numerics.Statistics.Percentile p = new MathNet.Numerics.Statistics.Percentile(c.Particles.Select(pa => pa.TravelTime));
            for (int i = 1; i < np; i++)
            {
              c.ParticleBreakthroughCurves.Add(new Tuple<double, double>(i / np * 100.0, p.Compute(i / np)));
            }
            //Also do oxidized breakthrough curves
            if (c.Particles.Count(pp => pp.Registration != 1) >= 20)
            {
              c.ParticleBreakthroughCurvesOxidized = new List<Tuple<double, double>>();
              p = new MathNet.Numerics.Statistics.Percentile(c.Particles.Where(pp => pp.Registration != 1).Select(pa => pa.TravelTime));
              for (int i = 1; i < np; i++)
              {
                c.ParticleBreakthroughCurvesOxidized.Add(new Tuple<double, double>(i / np * 100.0, p.Compute(i / np)));
              }
            }

          }
          DataRow row = DebugData.Rows.Find(c.ID);
          if (row == null)
          {
            row = DebugData.NewRow();
            row[0] = c.ID;
            DebugData.Rows.Add(row);
          }

          row["PartCount"] = c.Particles.Count;
          row["RedoxCount"] = c.Particles.Count(pp => pp.Registration == 1);
          row["RedoxRatio"] = c.Particles.Count(pp => pp.Registration == 1) / (double)c.Particles.Count;
          if (c.Particles.Count > 0)
          {
            row["Drain_to_River"] = c.Particles.Count(pa => pa.SinkType == "Drain_to_River") / (double)c.Particles.Count;
            row["Drain_to_Boundary"] = c.Particles.Count(pa => pa.SinkType == "Drain_to_Boundary") / (double)c.Particles.Count;
            row["Unsaturated_zone"] = c.Particles.Count(pa => pa.SinkType == "Unsaturated_zone") / (double)c.Particles.Count;
            row["River"] = c.Particles.Count(pa => pa.SinkType == "River") / (double)c.Particles.Count;
          }
        }
      }
      //Now clear memory
      leachdata.ClearMemory();

      foreach (var c in Catchments)
        c.Particles.Clear();
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


    public override void DebugPrint(string Directory, Dictionary<int, Catchment> Catchments)
    {

      NewMessage("Writing breakthrough curves");

      var selectedCatchments = Catchments.Values.Where(cc => cc.ParticleBreakthroughCurves != null);

      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(Directory, "BC.csv")))
      {
        StringBuilder headline = new StringBuilder();
        headline.Append("ID\tNumber of Particles");

        for (int i = 1; i < np; i++)
        {
          headline.Append("\t + " + (i / np * 100.0));
        }
        sw.WriteLine(headline);

        foreach (var c in selectedCatchments.Where(cc=>cc.ParticleBreakthroughCurves!=null))
        {
          StringBuilder line = new StringBuilder();
          line.Append(c.ID + "\t" + c.Particles.Count);
          foreach(var pe in c.ParticleBreakthroughCurves)
          {
            line.Append("\t" + pe.Item2);
          }
          sw.WriteLine(line);
        }
      }

      if (selectedCatchments.Count()>0)
      {

        //Get the output coordinate system
        ProjNet.CoordinateSystems.ICoordinateSystem projection;
        using (System.IO.StreamReader sr = new System.IO.StreamReader(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Default.prj")))
        {
          ProjNet.CoordinateSystems.CoordinateSystemFactory cs = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
          projection = cs.CreateFromWkt(sr.ReadToEnd());
        }


        using (ShapeWriter sw = new ShapeWriter(Path.Combine(Directory, Name + "_debug.shp")) { Projection = projection })
        {
          foreach (var bc in selectedCatchments.First().ParticleBreakthroughCurves)
          {
            DebugData.Columns.Add(((int)bc.Item1).ToString(), typeof(double));
          }
          foreach (var bc in selectedCatchments.First().ParticleBreakthroughCurves)
          {
            DebugData.Columns.Add(((int)bc.Item1).ToString() + "Ox", typeof(double));
          }

          foreach (var c in selectedCatchments)
          {
            GeoRefData gd = new GeoRefData() { Geometry = c.Geometry };
            var row = DebugData.Rows.Find(c.ID);

            if(c.ParticleBreakthroughCurves!=null)
              foreach (var bc in c.ParticleBreakthroughCurves)
                row[((int)bc.Item1).ToString()] = bc.Item2;

            if (c.ParticleBreakthroughCurvesOxidized != null)
              foreach (var bc in c.ParticleBreakthroughCurvesOxidized)
                row[((int)bc.Item1).ToString() + "Ox"] = bc.Item2;

            gd.Data = row;
            sw.Write(gd);
          }
        }
      }
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
    public IEnumerable<Particle> LoadParticles(string ShapeFileName)
    {
      NewMessage("Reading particles from: " + ShapeFileName);
      List<int> RedoxedParticles = new List<int>();
      Dictionary<int, Particle> NonRedoxedParticles = new Dictionary<int, Particle>();


      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          Particle p = new Particle();
          p.ID = sr.Data.ReadInt(i, "ID");
          p.XStart = sr.Data.ReadDouble(i, "X-Birth");
          p.YStart = sr.Data.ReadDouble(i, "Y-Birth");
          p.X = sr.Data.ReadDouble(i, "X-Reg");
          p.Y = sr.Data.ReadDouble(i, "Y-Reg");
          p.TravelTime = sr.Data.ReadDouble(i, "TravelTime");
          p.SinkType = sr.Data.ReadString(i, "SinkType");
          p.Registration = sr.Data.ReadInt(i, "Registrati");

          if (p.SinkType.ToLower().Trim() == "active_cell")
            RedoxedParticles.Add(p.ID);
          else if (!p.SinkType.Trim().StartsWith("Removed"))
            NonRedoxedParticles.Add(p.ID, p);
        }

        int k=0;

        foreach (var pid in RedoxedParticles)
        {
          if (NonRedoxedParticles.ContainsKey(pid))
            NonRedoxedParticles[pid].Registration = 1;
          else
            k++;
        }

        foreach (var p in NonRedoxedParticles.Values)
        {
          //Adjust traveltime from dfs2
          foreach (var dfs in UnsatAgeFiles)
          {

            if (p.HorizontalTravelDistance >= dfs.MinHorizontalTravelDistance & p.TravelTime>=dfs.MinTravelTime)
            {
              int col = dfs.dfs.GetColumnIndex(p.X);
              int row = dfs.dfs.GetRowIndex(p.Y);

              if (col >= 0 & row >= 0)
                p.TravelTime += Math.Max(0, dfs.Data[row, col]);
            }
          }
        }


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
     
      Parallel.ForEach(Catchments.Where(ca => ca.Particles.Count(pp=>pp.Registration!=1) > 0),  c =>
        {
          float[] values;
          if (!GWInput.TryGetValue(c.ID, out values))
          {
            values = new float[numberofmonths];
            values.Initialize();
            lock(Lock)
              GWInput.Add(c.ID, values);
          }
          foreach (var p in c.Particles.Where(pp => pp.Registration != 1))
          {
            var newlist = leachdata.GetValues(p.XStart, p.YStart,Start.AddDays(-p.TravelTime * 365.0), End.AddDays(-p.TravelTime * 365.0));
            if(newlist !=null)
            for (int i = 0; i < numberofmonths; i++)
              values[i] += newlist[i] / NumberOfParticlesPrGrid;
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
                c.Particles.Add(p);
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



    #endregion

  }
}