using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{

  public enum SinkType
  {
    Active_cell,
    Drain_to_River,
    Drain_to_Boundary,
    Unsaturated_zone,
    River,
    Removed_by_Well,
    Removed_by_Fixed_C_cell
  }


  public class ParticleReader : BaseModel
  {

    List<Particle> StartNotFound = new List<Particle>();
    List<Particle> EndNotFound = new List<Particle>();


    private object Lock = new object();

    public ParticleReader()
    {
      Name = "ParticleReader";
    }


    public Func<DBFReader, int, SinkType, bool> UnSatFilter = new Func<DBFReader, int, SinkType, bool>((Data, i, sinktype) =>
      sinktype == SinkType.Unsaturated_zone
      && Data.ReadInt(i, "IX-Birth") == Data.ReadInt(i, "IX-Reg")
      && Data.ReadInt(i, "IY-Birth") == Data.ReadInt(i, "IY-Reg"));

    /// <summary>
    /// Reads the particles and distributes them on the dictionaries
    /// </summary>
    /// <param name="shapefilename"></param>
    public IEnumerable<Particle> ReadParticleFile(string shapefilename, Func<DBFReader, int, SinkType,bool> ExcludeThis)
    {
      List<int> RedoxedParticles = new List<int>();
      Dictionary<int, Particle> NonRedoxedParticles = new Dictionary<int, Particle>();
      NewMessage("Reading particles from: " + shapefilename);
      int k = 0;


      using (DBFReader sr = new DBFReader(Path.ChangeExtension(shapefilename, "dbf")))

      {
        for (int i = 0; i < sr.NoOfEntries; i++)
        {
          int id = sr.ReadInt(i, "ID");
          SinkType sinktype = (SinkType)Enum.Parse(typeof(SinkType), sr.ReadString(i, "SinkType"));
          if (sinktype == SinkType.Active_cell) //Do not create a new particle
            RedoxedParticles.Add(id);
          else if (ExcludeThis != null && ExcludeThis(sr, i, sinktype))
          {
            k++;
          }
          else
          {
            Particle p = new Particle();
            p.Registration = sr.ReadInt(i, "Registrati");
            p.XStart = sr.ReadDouble(i, "X-Birth");
            p.YStart = sr.ReadDouble(i, "Y-Birth");
            p.ZStart = sr.ReadDouble(i, "Z-Birth");
            p.X = sr.ReadDouble(i, "X-Reg");
            p.Y = sr.ReadDouble(i, "Y-Reg");
            p.Z = sr.ReadDouble(i, "Z-Reg");
            p.TravelTime = sr.ReadDouble(i, "TravelTime");
            p.SinkType = sinktype;
            NonRedoxedParticles.Add(id, p);
          }
        }
      }
      

      //Set the registration on all particles that have be redoxed
      foreach (var pid in RedoxedParticles)
        if (NonRedoxedParticles.ContainsKey(pid))
          NonRedoxedParticles[pid].Registration = 1;

      NewMessage("Excluded: " + k + " particles.");

      return NonRedoxedParticles.Values;

    }
      public void Distribute(IEnumerable<Particle> Particles)
      {
        //Get the bounding box immediately
        double xmin = double.MaxValue;
        double xmax = double.MinValue;
        double ymin = double.MaxValue;
        double ymax = double.MinValue;

        foreach (var p in Particles)
        {
          //Get min-max coordinates from both start and end
          xmin = Math.Min(Math.Min(xmin, p.X), p.XStart);
          xmax = Math.Max(Math.Max(xmax, p.X), p.XStart);
          ymin = Math.Min(Math.Min(ymin, p.Y), p.YStart);
          ymax = Math.Max(Math.Max(ymax, p.Y), p.YStart);
        }
        var Boundingbox = Geometry.XYGeometryTools.Box(xmin, xmax, ymin, ymax);

        NewMessage("Distributing " + Particles.Count() + " particles on catchments");
      //Only test the catchments that overlaps the particles.
      var selectedCatchments = Catchments.Where(c => c.Geometry.OverLaps(Boundingbox)).ToArray();

      int noend = EndNotFound.Count;
      int nostart = StartNotFound.Count;

      Parallel.ForEach(Particles,
        (p) =>
        {
          bool endfound = false;
          bool startfound = false;
          foreach (var c in selectedCatchments)
          {
            if (c.Geometry.Contains(p.X, p.Y))
            {
              lock (Lock)
                c.EndParticles.Add(p);
              endfound = true;
              if (c.Geometry.Contains(p.XStart, p.YStart))
              {
                lock (Lock)
                  c.StartParticles.Add(p);
                startfound = true;
              }
              break;
            }
          }
          if(!startfound)
            foreach (var innerc in selectedCatchments)
            {
              if (innerc.Geometry.Contains(p.XStart, p.YStart))
              {
                lock (Lock)
                  innerc.StartParticles.Add(p);
                startfound = true;
                break;
              }
            }
          if (!startfound)
            lock (Lock)
              StartNotFound.Add(p);
          if (!endfound)
            lock (Lock)
              EndNotFound.Add(p);

        });

      NewMessage((EndNotFound.Count -noend) + " endpoints could not be found in the catchments");
      NewMessage((StartNotFound.Count-nostart) + " endpoints could not be found in the catchments");

    }

    private IEnumerable<Catchment> catchments;

    public IEnumerable<Catchment> Catchments
    {
      get { return catchments; }
      set
      {
        catchments = value;
      }
    }


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
          debugData.PrimaryKey = new DataColumn[] { debugData.Columns[0] };
        }
        return debugData;
      }
    }

    double np = 20.0;

    public  void DebugPrint(string Directory)
    {
      //We need to process data for extra output while we have the particles
      {
        foreach (var c in Catchments.Where(ca => ca.EndParticles.Count >= 1))
        {
          if (c.EndParticles.Count >= 20)
          {
            c.ParticleBreakthroughCurves = new List<Tuple<double, double>>();
            MathNet.Numerics.Statistics.Percentile p = new MathNet.Numerics.Statistics.Percentile(c.EndParticles.Select(pa => pa.TravelTime));
            for (int i = 1; i < np; i++)
            {
              c.ParticleBreakthroughCurves.Add(new Tuple<double, double>(i / np * 100.0, p.Compute(i / np)));
            }
            //Also do oxidized breakthrough curves
            if (c.EndParticles.Count(pp => pp.Registration != 1) >= 20)
            {
              c.ParticleBreakthroughCurvesOxidized = new List<Tuple<double, double>>();
              p = new MathNet.Numerics.Statistics.Percentile(c.EndParticles.Where(pp => pp.Registration != 1).Select(pa => pa.TravelTime));
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

          row["PartCount"] = c.EndParticles.Count;
          row["RedoxCount"] = c.EndParticles.Count(pp => pp.Registration == 1);
          row["RedoxRatio"] = c.EndParticles.Count(pp => pp.Registration == 1) / (double)c.EndParticles.Count;
          if (c.EndParticles.Count > 0)
          {
            row["Drain_to_River"] = c.EndParticles.Count(pa => pa.SinkType == SinkType.Drain_to_River) / (double)c.EndParticles.Count;
            row["Drain_to_Boundary"] = c.EndParticles.Count(pa => pa.SinkType == SinkType.Drain_to_Boundary) / (double)c.EndParticles.Count;
            row["Unsaturated_zone"] = c.EndParticles.Count(pa => pa.SinkType == SinkType.Unsaturated_zone) / (double)c.EndParticles.Count;
            row["River"] = c.EndParticles.Count(pa => pa.SinkType == SinkType.River) / (double)c.EndParticles.Count;
          }
        }
      }




      NewMessage("Writing breakthrough curves");

      var selectedCatchments = Catchments.Where(cc => cc.ParticleBreakthroughCurves != null);

      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(Directory, "BC.csv")))
      {
        StringBuilder headline = new StringBuilder();
        headline.Append("ID\tNumber of Particles");

        for (int i = 1; i < np; i++)
        {
          headline.Append("\t + " + (i / np * 100.0));
        }
        sw.WriteLine(headline);

        foreach (var c in selectedCatchments.Where(cc => cc.ParticleBreakthroughCurves != null))
        {
          StringBuilder line = new StringBuilder();
          line.Append(c.ID + "\t" + c.EndParticles.Count);
          foreach (var pe in c.ParticleBreakthroughCurves)
          {
            line.Append("\t" + pe.Item2);
          }
          sw.WriteLine(line);
        }
      }

      if (selectedCatchments.Count() > 0)
      {


        using (ShapeWriter sw = new ShapeWriter(Path.Combine(Directory, Name + "_debug.shp")) { Projection = MainModel.projection })
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

            if (c.ParticleBreakthroughCurves != null)
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

  }
}
