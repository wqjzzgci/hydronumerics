using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class ReductionMap:BaseModel
  {

    private SafeFile OutputFile;
    private List<SafeFile> ParticleFiles = new List<SafeFile>();
    private DataTable StateVariables;
    private DataTable ReductionVariables;
    private DateTime Start;
    private DateTime End;
    private object Lock = new object();

    public override void ReadConfiguration(System.Xml.Linq.XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Include)
      {
        foreach (var parfile in Configuration.Element("ParticleFiles").Elements("ParticleFile"))
        {
          ParticleFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("ShapeFileName") });
        }
        Start = new DateTime(Configuration.SafeParseInt("FromYear") ?? 2000, Configuration.SafeParseInt("FromMonth") ?? 1, 1);
        End = new DateTime(Configuration.SafeParseInt("ToYear") ?? 2002, Configuration.SafeParseInt("ToMonth") ?? 1, 1);

        OutputFile = new SafeFile() { CheckIfFileExists = false, FileName = Configuration.SafeParseString("ShapeFileName") };
      }
    }

    private void Print(Dictionary<int, Catchment> AllCatchments)
    {
      //Get the output coordinate system
      ProjNet.CoordinateSystems.ICoordinateSystem projection;
      using (System.IO.StreamReader sr = new System.IO.StreamReader(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Default.prj")))
      {
        ProjNet.CoordinateSystems.CoordinateSystemFactory cs = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
        projection = cs.CreateFromWkt(sr.ReadToEnd());
      }

      using (ShapeWriter sw = new ShapeWriter(OutputFile.FileName) { Projection = projection })
      {
        for (int i = 0; i < ReductionVariables.Rows.Count; i++)
        {
          GeoRefData gd = new GeoRefData() { Data = ReductionVariables.Rows[i], Geometry = AllCatchments[(int)ReductionVariables.Rows[i]["ID"]].Geometry };
          sw.Write(gd);
        }
      }
    }


    public void MakeMap(Dictionary<int, Catchment> AllCatchments, IEnumerable<Catchment> EndCatchments, DataTable StateVariables, IEnumerable<INitrateModel> SourceModels, IEnumerable<INitrateModel> InternalSinkModels, IEnumerable<INitrateModel> MainSinkModels)
    {
      this.StateVariables = StateVariables;

      NewMessage("Creating reduction maps");

      NewMessage("Calculated discharge percentage");
      BuildReduction(AllCatchments, EndCatchments, SourceModels, InternalSinkModels, MainSinkModels);


      foreach (var s in ParticleFiles)
      {
        List<int> RedoxedParticles = new List<int>();
        Dictionary<int, Particle> NonRedoxedParticles = new Dictionary<int, Particle>();
        NewMessage("Reading particles from: " + s.FileName);

        using (ShapeReader sr = new ShapeReader(s.FileName))
        {
          for (int i = 0; i < sr.Data.NoOfEntries; i++)
          {
            int id = sr.Data.ReadInt(i, "ID");
            if (sr.Data.ReadString(i, "SinkType") == "Active_cell")
              RedoxedParticles.Add(id);
            else
            {
              Particle p = new Particle();
              p.Registration = sr.Data.ReadInt(i, "Registrati");
              p.XStart = sr.Data.ReadDouble(i, "X-Birth");
              p.YStart = sr.Data.ReadDouble(i, "Y-Birth");
              p.X = sr.Data.ReadDouble(i, "X-Reg");
              p.Y = sr.Data.ReadDouble(i, "Y-Reg");
              NonRedoxedParticles.Add(id, p);
            }
          }
        }

        //Set the registration on all particles that have be redoxed
        foreach (var pid in RedoxedParticles)
          if (NonRedoxedParticles.ContainsKey(pid))
            NonRedoxedParticles[pid].Registration = 1;


        NewMessage("Distributing " + NonRedoxedParticles.Count + " particles on catchments");

        var bb = HydroNumerics.Geometry.XYGeometryTools.BoundingBox(NonRedoxedParticles.Values);

        var selectedCatchments = AllCatchments.Values.Where(c => c.Geometry.OverLaps(bb)).ToArray();


        Parallel.ForEach(NonRedoxedParticles.Values,
          (p) =>
          {
            foreach (var c in selectedCatchments)
            {
              if (c.Geometry.Contains(p.XStart, p.YStart))
              {
                lock (Lock)
                  c.Particles.Add(p);
                break;
              }
            }
          });

      }

      Dictionary<int, double> GroundwaterReduction = new Dictionary<int, double>();

      foreach (var v in AllCatchments.Values)
      {
        var row = ReductionVariables.Rows.Find(v.ID);

        row["Particles"] = v.Particles.Count;
        row["RedoxedParticles"] = v.Particles.Count(p => p.Registration == 1);
        row["GWDischarge"] = 1.0 - (double)v.Particles.Count(p => p.Registration == 1) / ((double)v.Particles.Count);

        double totalred = 0;
        List<double> reductions = new List<double>();
        Dictionary<int, int> partcounts = new Dictionary<int, int>();
        partcounts.Add(v.ID, 0);
        Parallel.ForEach(v.Particles.Where(p => p.Registration != 1), p =>
        {
          if (v.Geometry.Contains(p.X, p.Y))
            lock (Lock)
              partcounts[v.ID]++;
          else
          {
            foreach (var c in AllCatchments.Values)
            {
              if (c.Geometry.Contains(p.X, p.Y))
              {
                lock (Lock)
                {
                  if (partcounts.ContainsKey(c.ID))
                    partcounts[c.ID]++;
                  else
                    partcounts.Add(c.ID, 1);
                }
                break;
              }
            }
          }
        });
        row["OutsideParticles"] = partcounts.Skip(1).Sum(k => k.Value);

        foreach (var kvp in partcounts)
        {
          var temprow = ReductionVariables.Rows.Find(kvp.Key);

          totalred += (double)temprow["SurfaceDischarge"] * kvp.Value;
        }
        row["TotalDischarge"] = (double)row["GWDischarge"] * totalred / (double)v.Particles.Count(p => p.Registration != 1);
      }

      Print(AllCatchments);

    }


    private void BuildReduction(Dictionary<int, Catchment> AllCatchments, IEnumerable<Catchment> EndCatchments, IEnumerable<INitrateModel> SourceModels, IEnumerable<INitrateModel> InternalSinkModels, IEnumerable<INitrateModel> MainSinkModels)
    {
      ReductionVariables = new DataTable();
      ReductionVariables.Columns.Add("ID", typeof(int));
      ReductionVariables.Columns.Add("Sources", typeof(double));
      ReductionVariables.Columns.Add("Internalreductions", typeof(double));
      ReductionVariables.Columns.Add("MainReductions", typeof(double));
      ReductionVariables.Columns.Add("UpstreamInput", typeof(double));
      ReductionVariables.Columns.Add("DownStreamOutput", typeof(double));
      ReductionVariables.Columns.Add("InternalDischarge", typeof(double));
      ReductionVariables.Columns.Add("UpstreamDischarge", typeof(double));
      ReductionVariables.Columns.Add("AccDischarge", typeof(double));
      ReductionVariables.Columns.Add("SurfaceDischarge", typeof(double));
      ReductionVariables.Columns.Add("Particles", typeof(int));
      ReductionVariables.Columns.Add("RedoxedParticles", typeof(int));
      ReductionVariables.Columns.Add("OutsideParticles", typeof(int));
      ReductionVariables.Columns.Add("GWDischarge", typeof(double));
      ReductionVariables.Columns.Add("TotalDischarge", typeof(double));
      ReductionVariables.PrimaryKey = new DataColumn[] { ReductionVariables.Columns["ID"] };


      foreach (var v in AllCatchments.Values)
      {
        var row = ReductionVariables.NewRow();
        row["ID"] = v.ID;

        double source = 0;
        double internalred = 0;
        double mainred = 0;
        double downstream = 0;
        DateTime time = Start;
        while (time < End)
        {
          var staterow = StateVariables.Rows.Find(new object[] { v.ID, time });

          foreach (var ir in SourceModels)
            source += (double)staterow[ir.Name];

          foreach (var ir in InternalSinkModels)
            internalred += (double)staterow[ir.Name];

          foreach (var ir in MainSinkModels)
            if (!staterow.IsNull(ir.Name))
              mainred += (double)staterow[ir.Name];

          downstream += (double)staterow["DownStreamOutput"];
          time = time.AddMonths(1);
        }

        row["Sources"] = source;
        row["Internalreductions"] = internalred;
        row["MainReductions"] = mainred;
        double upstreaminput = downstream - source + mainred + internalred;
        row["UpstreamInput"] = upstreaminput;
        row["DownStreamOutput"] = downstream;
        double internaldist = (source - internalred) / (source - internalred + upstreaminput);

        row["InternalDischarge"] = (1 - internalred / source) * (1 - mainred / (source - internalred + upstreaminput) * internaldist);
        row["UpstreamDischarge"] = 1 - mainred / (source - internalred + upstreaminput) * (1 - internaldist);

        ReductionVariables.Rows.Add(row);
      }

      foreach (var v in EndCatchments)
      {
        AddToUpStream(1, v);
      }

      foreach (var c in AllCatchments.Values)
      {
        var staterow = ReductionVariables.Rows.Find(new object[] { c.ID });
        staterow["SurfaceDischarge"] = (double)staterow["InternalDischarge"] * (double)staterow["AccDischarge"];
      }

    }

    private void AddToUpStream(double d, Catchment c)
    {
      var staterow = ReductionVariables.Rows.Find(new object[] { c.ID });
      double accumulated = (double)staterow["UpstreamDischarge"] * d;
      staterow["AccDischarge"] = accumulated;
      foreach (var opc in c.UpstreamConnections)
        AddToUpStream(accumulated, opc);
    }




  }
}
