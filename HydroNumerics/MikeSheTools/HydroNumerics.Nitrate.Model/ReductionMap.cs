using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class ReductionMap : BaseModel
  {

    private SafeFile OutputFile;
    private List<SafeFile> ParticleFiles = new List<SafeFile>();
    private DataTable StateVariables;
    private DateTime Start;
    private DateTime End;
    private object Lock = new object();

    private DataTable _ReductionVariables;

    private DataTable ReductionVariables
    {
      get
      {
        if (_ReductionVariables == null)
        {
          _ReductionVariables = new DataTable();
          _ReductionVariables.Columns.Add("ID", typeof(int));
          _ReductionVariables.Columns.Add("Particles", typeof(int));
          _ReductionVariables.Columns.Add("RedoxedParticles", typeof(int));
          _ReductionVariables.Columns.Add("OutsideParticles", typeof(int));
          _ReductionVariables.Columns.Add("SeaParticles", typeof(int));

          _ReductionVariables.Columns.Add("GWSources", typeof(double));
          _ReductionVariables.Columns.Add("Sources", typeof(double));

          _ReductionVariables.Columns.Add("GWReductions", typeof(double));
          _ReductionVariables.Columns.Add("ConcepReductions", typeof(double));
          _ReductionVariables.Columns.Add("Internalreductions", typeof(double));
          _ReductionVariables.Columns.Add("MainReductions", typeof(double));
          _ReductionVariables.Columns.Add("AccSources", typeof(double));
          _ReductionVariables.Columns.Add("AccGWSources", typeof(double));
          _ReductionVariables.Columns.Add("AccInternalreductions", typeof(double));
          _ReductionVariables.Columns.Add("AccMainReductions", typeof(double));
          _ReductionVariables.Columns.Add("AccGWReductions", typeof(double));
          _ReductionVariables.Columns.Add("UpstreamInput", typeof(double));
          _ReductionVariables.Columns.Add("DownStreamOutput", typeof(double));
          _ReductionVariables.Columns.Add("ObservedNitrate", typeof(double));
          _ReductionVariables.Columns.Add("BiasFactor", typeof(double));
          _ReductionVariables.Columns.Add("InternalDischarge", typeof(double));
          _ReductionVariables.Columns.Add("MainDischarge", typeof(double));
          _ReductionVariables.Columns.Add("AccDischarge", typeof(double));
          _ReductionVariables.Columns.Add("SurfaceDischarge", typeof(double));
          _ReductionVariables.Columns.Add("ConceptualGWDischarge", typeof(double));
          _ReductionVariables.Columns.Add("GWDischarge", typeof(double));
          _ReductionVariables.Columns.Add("ConcExitDischarge", typeof(double));
          _ReductionVariables.Columns.Add("TotalDischarge", typeof(double));
          _ReductionVariables.PrimaryKey = new DataColumn[] { ReductionVariables.Columns["ID"] };
        }
        return _ReductionVariables;
      }
    }





    public override void ReadConfiguration(System.Xml.Linq.XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Include)
      {
        var ParticleFiles = Configuration.Element("ParticleFiles");
        UseUnsatFilter = ParticleFiles.SafeParseBool("RemoveUnsatParticles") ?? false;

        foreach (var parfile in ParticleFiles.Elements("ParticleFile"))
        {
          ParticleFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("ShapeFileName") });
        }

        Start = new DateTime(Configuration.SafeParseInt("FromYear") ?? 2000, Configuration.SafeParseInt("FromMonth") ?? 1, 1);
        End = new DateTime(Configuration.SafeParseInt("ToYear") ?? 2002, Configuration.SafeParseInt("ToMonth") ?? 1, 1);

        OutputFile = new SafeFile() { CheckIfFileExists = false, FileName = Configuration.SafeParseString("ShapeFileName") };
      }
    }


    public void MakeMap(Dictionary<int, Catchment> AllCatchments, IEnumerable<Catchment> EndCatchments, DataTable StateVariables, IEnumerable<INitrateModel> SourceModels, IEnumerable<INitrateModel> InternalSinkModels, IEnumerable<INitrateModel> MainSinkModels)
    {
      this.StateVariables = StateVariables;

      NewMessage("Creating reduction maps");

      ParticleReader pr = new ParticleReader();
      pr.Catchments = AllCatchments.Values;

      foreach (var s in ParticleFiles)
      {
        var particles = pr.ReadParticleFile(s.FileName, null);

        NewMessage("Reading particles from: " + s.FileName);


        NewMessage("Distributing " + particles.Count() + " particles on catchments");
        pr.Distribute(particles);
      }

      Dictionary<int, double> GroundwaterReduction = new Dictionary<int, double>();

      foreach (var v in AllCatchments.Values)
      {
        var row = GetReductionRow(v.ID);

        row["Particles"] = v.StartParticles.Count;
        row["RedoxedParticles"] = v.StartParticles.Count(p => p.Registration == 1);
        if (v.EndParticles.Count > 0)
          row["GWDischarge"] = 1.0 - (double)v.StartParticles.Count(p => p.Registration == 1) / ((double)v.StartParticles.Count);
        else
          row["GWDischarge"] = 1;

      }

      NewMessage("Calculated discharge percentage");

      //<ID15<ID15,no of particles>> For each ID15 there is a list of ID15s where the particles exit.
      Dictionary<int, Dictionary<int, int>> CatchmentPartCounts = new Dictionary<int, Dictionary<int, int>>();
      Dictionary<int, int> NonRedoxedSea = new Dictionary<int, int>();

      foreach (var v in AllCatchments.Values)
      {
        List<double> reductions = new List<double>();
        //Distribute the particles infiltrating on the cathcments where they exit
        Dictionary<int, int> partcounts = new Dictionary<int, int>();
        CatchmentPartCounts.Add(v.ID, partcounts);
        NonRedoxedSea.Add(v.ID, 0);
        partcounts.Add(v.ID, 0);
        //Only take the non redoxed
        Parallel.ForEach(v.StartParticles.Where(p => p.Registration != 1), p =>
        {
          if (v.Geometry.Contains(p.X, p.Y))
            lock (Lock)
              partcounts[v.ID]++;
          else
          {
            bool found = false;
            foreach (var c in AllCatchments.Values)
            {
              if (c.Geometry.Contains(p.X, p.Y))
              {

                lock (Lock)
                {
                  found=true;
                  if (partcounts.ContainsKey(c.ID))
                    partcounts[c.ID]++;
                  else
                    partcounts.Add(c.ID, 1);
                }
                break;
              }
            }
            if (!found)
              NonRedoxedSea[v.ID]++;
          }
        });

      }

      BuildReduction(AllCatchments, EndCatchments, SourceModels, InternalSinkModels, MainSinkModels);
      foreach (var v in AllCatchments.Values)
      {
        double totalSurface = 0;
        double totalConceptual = 0;

        var row = GetReductionRow(v.ID);
        row["OutsideParticles"] = CatchmentPartCounts[v.ID].Skip(1).Sum(k => k.Value);
        row["SeaParticles"] = v.StartParticles.Count(p => p.Registration != 1) - CatchmentPartCounts[v.ID].Sum(k => k.Value);
        int totalparticlesNotSea = CatchmentPartCounts[v.ID].Sum(k => k.Value);


        //Loop all the catchments where particles exit. This includes the actual catchment
        foreach (var kvp in CatchmentPartCounts[v.ID]) 
        {
          var temprow = GetReductionRow(kvp.Key);
          totalSurface += (double)temprow["SurfaceDischarge"] * kvp.Value; //Multiply the particles entering a particular catchment with the reductions of that catchment
          totalConceptual += (double)temprow["ConceptualGWDischarge"] * kvp.Value; //Multiply the particles entering a particular catchment with the reductions of that catchment
        }
        totalSurface /= totalparticlesNotSea;
        totalConceptual /= totalparticlesNotSea;
          
        row["ConcExitDischarge"]=totalConceptual;
        row["TotalDischarge"] = totalSurface * totalConceptual * (double)row["GWDischarge"];
      }


      Print(AllCatchments,"");

    }

    private DataRow GetReductionRow(int id)
    {
      var row = ReductionVariables.Rows.Find(id);
      if (row == null)
      {
        row = ReductionVariables.NewRow(); //Try to find first
        row["ID"] = id;
        ReductionVariables.Rows.Add(row);
      }
      return row;
    }


    private void GetCatchmentsWithObs(Catchment c, List<Catchment> Upstreams)
    {
      foreach (var upc in c.UpstreamConnections)
        GetCatchmentsWithObs(upc, Upstreams);
      if (c.Measurements != null)
        Upstreams.Add(c);
    }


    private void FindBiasFactors(Dictionary<int, Catchment> AllCatchments, IEnumerable<Catchment> EndCatchments, IEnumerable<INitrateModel> SourceModels, IEnumerable<INitrateModel> InternalSinkModels, IEnumerable<INitrateModel> MainSinkModels)
    {
      List<Catchment> SortedCatchments = new List<Catchment>();

      foreach (var item in EndCatchments)
      {
        GetCatchmentsWithObs(item, SortedCatchments);
      }


      foreach (var v in SortedCatchments)
      {
        var row = GetReductionRow(v.ID);

        double source = 0;
        double internalred = 0;
        double mainred = 0;
        double downstream = 0;
        DateTime time = Start;
        double GWSource = 0;
        double GWReductions = 0;
        double Observed = 0;

        //Time summation
        while (time <= End)
        {
          var staterow = StateVariables.Rows.Find(new object[] { v.ID, time });

          if (!staterow.IsNull("ObservedNitrate"))
          {
            foreach (var ir in SourceModels.Where(ss => !(ss is GroundWaterSource)))
              source += AccumulateUpstream(ir.Name, v, time, false);

            foreach (var ir in SourceModels.Where(ss => ss is GroundWaterSource))
            {
             GWSource += AccumulateGWSourceUpstream(ir.Name, v, time);
             GWReductions += AccumulateGWReductionUpstream(ir.Name, v, time);
            }

            foreach (var ir in InternalSinkModels)
              internalred += AccumulateUpstream(ir.Name, v, time, true);

            foreach (var ir in MainSinkModels)
              mainred += AccumulateUpstream(ir.Name, v, time, true);

            Observed += (double)staterow["ObservedNitrate"];
          }
          time = time.AddMonths(1);
        }

        if (Observed != 0)
        {
          row["ObservedNitrate"] = Observed;
          row["AccSources"] = source;
          row["AccGWSources"] = GWSource;
          row["AccInternalreductions"] = internalred;
          row["AccMainReductions"] = mainred;
          row["AccGWReductions"] = GWReductions;
          row["BiasFactor"] = (GWSource + source - Observed) / (internalred + mainred + GWReductions);
        }
      }
      foreach (var v in EndCatchments)
      {
        AddErrorToUpStream(1, v);
      }
    }

    private double AccumulateGWReductionUpstream(string Column, Catchment c, DateTime Time)
    {
      double Value = 0;
      var staterow = StateVariables.Rows.Find(new object[] { c.ID, Time });
      var row = GetReductionRow(c.ID);

      if (!staterow.IsNull(Column))
        Value += (double)staterow[Column] / (double)row["GWDischarge"] - (double)staterow[Column];
      foreach (var upc in c.UpstreamConnections)
      {
        Value += AccumulateGWReductionUpstream(Column, upc, Time);
      }
      return Value;
    }




    private double AccumulateGWSourceUpstream(string Column, Catchment c, DateTime Time)
    {
      double Value = 0;
      var staterow = StateVariables.Rows.Find(new object[] { c.ID, Time });
      var row = GetReductionRow(c.ID);

      if (!staterow.IsNull(Column))
        Value += (double)staterow[Column] / (double)row["GWDischarge"];
      foreach (var upc in c.UpstreamConnections)
      {
        Value += AccumulateGWSourceUpstream(Column, upc, Time);
      }
      return Value;
    }

    private double AccumulateUpstream(string Column, Catchment c, DateTime Time, bool WithBias)
    {
      double Value = 0;
      var staterow = StateVariables.Rows.Find(new object[] { c.ID, Time });
      if (!staterow.IsNull(Column))
      {
        double BiasFactor=1;

        if (WithBias)
        {
          var row = GetReductionRow(c.ID);
          if(!row.IsNull("BiasFactor"))
            BiasFactor =(double)row["BiasFactor"];
        }
        Value += (double)staterow[Column] * BiasFactor;
      }
      foreach (var upc in c.UpstreamConnections)
      {
        Value += AccumulateUpstream(Column, upc, Time, WithBias);
      }
      return Value;
    }

    private void BuildReduction(Dictionary<int, Catchment> AllCatchments, IEnumerable<Catchment> EndCatchments, IEnumerable<INitrateModel> SourceModels, IEnumerable<INitrateModel> InternalSinkModels, IEnumerable<INitrateModel> MainSinkModels)
    {
      List<ConceptualSourceReducer> conceptualLayers = InternalSinkModels.Where(Is => Is is ConceptualSourceReducer).Cast<ConceptualSourceReducer>().Where(Is=>!string.IsNullOrEmpty(Is.SourceModelName)).ToList();

      foreach (var v in AllCatchments.Values)
      {
        var row = GetReductionRow(v.ID);

        double source = 0;
        double internalred = 0;
        double mainred = 0;
        DateTime time = Start;
        double GWSource = 0;
        double ConceptualReduction = 0;
        double Observed = 0;
        bool UseObs = true;

        double BiasFactor = 1;
        if (!row.IsNull("BiasFactor"))
          BiasFactor = (double)row["BiasFactor"];

        //Time summation
        while (time <= End)
        {
          var staterow = StateVariables.Rows.Find(new object[] { v.ID, time });

          foreach (var ir in SourceModels.Where(ss => !(ss is GroundWaterSource)))
            if(!staterow.IsNull(ir.Name))
              source += (double)staterow[ir.Name];

          foreach (var ir in SourceModels.Where(ss => ss is GroundWaterSource))
            if (!staterow.IsNull(ir.Name))
              GWSource += (double)staterow[ir.Name];

          //Remove what has been reduced in the conceptual layer from the sources.
          foreach (var ir in conceptualLayers)
            if (!staterow.IsNull(ir.Name))
              ConceptualReduction += (double)staterow[ir.Name];

          //Take all internal sources except the ones removed as conceptual layers
          foreach (var ir in InternalSinkModels)
            if (!staterow.IsNull(ir.Name))
              internalred += (double)staterow[ir.Name];

          foreach (var ir in MainSinkModels)
            if (!staterow.IsNull(ir.Name))
              if (!staterow.IsNull(ir.Name))
                mainred += (double)staterow[ir.Name];

          time = time.AddMonths(1);
        }

        internalred -= ConceptualReduction;

        ConceptualReduction *= BiasFactor;
        internalred *= BiasFactor;
        mainred *= BiasFactor;

        double gwdischarge = (double)row["GWDischarge"];
        double GWTotalSource;
        if (gwdischarge > 0)
          GWTotalSource = GWSource / gwdischarge;
        else GWTotalSource = 0;
        double GWReduction =(GWTotalSource - GWSource)*BiasFactor;
        GWSource = GWTotalSource - GWReduction;

        row["GWSources"] = GWTotalSource;
        row["GWReductions"] = GWReduction;

        if(GWTotalSource>0)
          row["GWDischarge"] = 1.0 - GWReduction / GWTotalSource;
        row["Sources"] = source;
        row["ConcepReductions"] = ConceptualReduction;
        row["Internalreductions"] = internalred;
        row["MainReductions"] = mainred;
        row["InternalDischarge"] = 1.0 - internalred / (source + GWSource -ConceptualReduction);
        if(GWSource>0)
          row["ConceptualGWDischarge"] = 1.0 - ConceptualReduction / GWSource;
        else
          row["ConceptualGWDischarge"] = 1.0;
      }

      foreach (var item in EndCatchments)
      {
        AddToUpStream1(item);
      }

      foreach (var item in EndCatchments)
      {
        AddToUpStream(1, item);
      }


      foreach (var c in AllCatchments.Values)
      {
        var staterow = ReductionVariables.Rows.Find(new object[] { c.ID });
        staterow["SurfaceDischarge"] = (double)staterow["InternalDischarge"] * (double)staterow["AccDischarge"];
      }
    }

    private void AddToUpStream1(Catchment c)
    {
      var row = ReductionVariables.Rows.Find(new object[] { c.ID });

      double UpstreamInput = 0;
      foreach (var opc in c.UpstreamConnections)
      {
        AddToUpStream1(opc);
        UpstreamInput += (double)ReductionVariables.Rows.Find(new object[] { opc.ID })["DownStreamOutput"];
      }

      double GWSource = (double)row["GWSources"];
      double GWReduction = (double)row["GWReductions"];
      double source = (double)row["Sources"];
      double ConceptualReduction = (double)row["ConcepReductions"];
      double internalred = (double)row["Internalreductions"];
      double mainred = (double)row["MainReductions"];


      row["DownStreamOutput"] = source + GWSource - mainred - internalred - ConceptualReduction - GWReduction + UpstreamInput;
      row["UpstreamInput"] = UpstreamInput;

      row["MainDischarge"] = 1.0 - mainred / (source + GWSource - internalred - ConceptualReduction - GWReduction + UpstreamInput);

    }


    private void AddToUpStream(double d, Catchment c)
    {
      var row = GetReductionRow(c.ID);
      double accumulated = (double)row["MainDischarge"] * d;
      row["AccDischarge"] = accumulated;
      foreach (var opc in c.UpstreamConnections)
      {
        AddToUpStream(accumulated, opc);
      }

    }

    private void AddErrorToUpStream(double d, Catchment c)
    {
      var staterow = ReductionVariables.Rows.Find(new object[] { c.ID });
      if (!staterow.IsNull("BiasFactor"))
        d = (double)staterow["BiasFactor"];
      else
        staterow["BiasFactor"] = d;
      foreach (var opc in c.UpstreamConnections)
        AddErrorToUpStream(d, opc);
    }

    private bool _UseUnsatFilter = false;
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


    private void Print(Dictionary<int, Catchment> AllCatchments, string FileNameAttach)
    {
      //Get the output coordinate system
      ProjNet.CoordinateSystems.ICoordinateSystem projection;
      using (System.IO.StreamReader sr = new System.IO.StreamReader(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Default.prj")))
      {
        ProjNet.CoordinateSystems.CoordinateSystemFactory cs = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
        projection = cs.CreateFromWkt(sr.ReadToEnd());
      }

      using (ShapeWriter sw = new ShapeWriter(Path.Combine(Path.GetDirectoryName(OutputFile.FileName),Path.GetFileNameWithoutExtension(OutputFile.FileName)+FileNameAttach)) { Projection = projection })
      {
        for (int i = 0; i < ReductionVariables.Rows.Count; i++)
        {
          GeoRefData gd = new GeoRefData() { Data = ReductionVariables.Rows[i], Geometry = AllCatchments[(int)ReductionVariables.Rows[i]["ID"]].Geometry };
          sw.Write(gd);
        }
      }
    }


  }
}
