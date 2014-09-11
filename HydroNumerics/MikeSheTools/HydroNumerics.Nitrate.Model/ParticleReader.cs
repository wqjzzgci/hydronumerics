using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class ParticleReader:BaseModel
  {
    public Dictionary<int, List<Particle>> StartDistribution { get; private set; }
    public Dictionary<int, List<Particle>> EndDistribution { get; private set; }

    private object Lock = new object();

    public ParticleReader()
    {
      StartDistribution = new Dictionary<int, List<Particle>>();
      EndDistribution = new Dictionary<int, List<Particle>>();
    }

    /// <summary>
    /// Reads the particles and distributes them on the dictionaries
    /// </summary>
    /// <param name="shapefilename"></param>
    public void ReadParticleFile(string shapefilename)
    {
      List<int> RedoxedParticles = new List<int>();
      Dictionary<int, Particle> NonRedoxedParticles = new Dictionary<int, Particle>();
      NewMessage("Reading particles from: " + shapefilename);


      HydroNumerics.Geometry.XYPolygon Boundingbox;
      using (ShapeReader sr = new ShapeReader(shapefilename))
      {
        //Get the bounding box immediately
        double xmin = double.MaxValue;
        double xmax = double.MinValue;
        double ymin = double.MaxValue;
        double ymax = double.MinValue;

        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          int id = sr.Data.ReadInt(i, "ID");
          string sinktype = sr.Data.ReadString(i, "SinkType");
          if (sinktype == "Active_cell")
            RedoxedParticles.Add(id);
          else if (!sinktype.StartsWith("Removed"))
          {
            Particle p = new Particle();
            p.Registration = sr.Data.ReadInt(i, "Registrati");
            p.XStart = sr.Data.ReadDouble(i, "X-Birth");
            p.YStart = sr.Data.ReadDouble(i, "Y-Birth");
            p.X = sr.Data.ReadDouble(i, "X-Reg");
            p.Y = sr.Data.ReadDouble(i, "Y-Reg");
            p.TravelTime = sr.Data.ReadDouble(i, "TravelTime");
            NonRedoxedParticles.Add(id, p);

            //Get min-max coordinates from both start and end
            xmin = Math.Min(Math.Min(xmin, p.X), p.XStart);
            xmax = Math.Max(Math.Max(xmax, p.X), p.XStart);
            ymin = Math.Min(Math.Min(ymin, p.Y), p.YStart);
            ymax = Math.Max(Math.Max(ymax, p.Y), p.YStart);

          }
        }
        Boundingbox = Geometry.XYGeometryTools.Box(xmin, xmax, ymin, ymax);
      }

      //Set the registration on all particles that have be redoxed
      foreach (var pid in RedoxedParticles)
        if (NonRedoxedParticles.ContainsKey(pid))
          NonRedoxedParticles[pid].Registration = 1;


      NewMessage("Distributing " + NonRedoxedParticles.Count + " particles on catchments");
      //Only test the catchments that overlaps the particles.
      var selectedCatchments = Catchments.Where(c => c.Geometry.OverLaps(Boundingbox)).ToArray();

      Parallel.ForEach(NonRedoxedParticles.Values,
        (p) =>
        {
          foreach (var c in selectedCatchments)
          {
            if (c.Geometry.Contains(p.X, p.Y))
            {
              lock (Lock)
                EndDistribution[c.ID].Add(p);
              if (c.Geometry.Contains(p.XStart, p.YStart))
              {
                lock (Lock)
                  StartDistribution[c.ID].Add(p);
              }
              else
              {
                foreach (var innerc in selectedCatchments)
                {
                  if (innerc.Geometry.Contains(p.XStart, p.YStart))
                  {
                    lock (Lock)
                      StartDistribution[innerc.ID].Add(p);
                    break;
                  }
                }
              }
              break;
            }
          }
        });
    }

    private IEnumerable<Catchment> catchments;

    public IEnumerable<Catchment> Catchments
    {
      get { return catchments; }
      set { catchments = value;
      foreach (var c in catchments)
      {
        StartDistribution.Add(c.ID, new List<Particle>());
        EndDistribution.Add(c.ID, new List<Particle>());
      }

      }
    }
    




  }
}
