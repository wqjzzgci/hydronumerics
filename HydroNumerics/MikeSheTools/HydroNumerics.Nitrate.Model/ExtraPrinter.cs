using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;

using MathNet.Numerics.Statistics;

using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class ExtraPrinter
  {
    private List<SafeFile> ParticleFiles = new List<SafeFile>();
    private SafeFile CatchmentsShape;
          ParticleReader Pr = new ParticleReader();

          private string OutPutDir = @"d:\temp\part";


    Dictionary<int, Catchment> AllCatchments;
    private object Lock = new object();

    public void FromConfigFile(string xmlfilename)
    {

      string dir = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(Path.GetDirectoryName(xmlfilename));


      XElement Configuration = XDocument.Load(xmlfilename).Element("Configuration");

      OutPutDir = Configuration.SafeParseString("OutputDir");

     
        foreach (var parfile in Configuration.Element("ParticleFiles").Elements("ParticleFile"))
        {
          ParticleFiles.Add(new SafeFile() { FileName = parfile.SafeParseString("ShapeFileName") });
        }
        CatchmentsShape = new SafeFile() { FileName = Configuration.Element("Catchments").SafeParseString("ShapeFileName") };

        Directory.SetCurrentDirectory(dir);
        Directory.CreateDirectory(OutPutDir);
      

      MainViewModel mw = new MainViewModel();
      mw.LoadCatchments(CatchmentsShape.FileName);
      AllCatchments = mw.AllCatchments;

      Pr.Catchments = AllCatchments.Values;

      foreach (var s in ParticleFiles)
      {
        Pr.ReadParticleFile(s.FileName);
      }

      using (ShapeWriter sw = new ShapeWriter(Path.Combine(OutPutDir,"out.shp")))
      {
        DataTable dt = new DataTable();
        dt.Columns.Add("ID15", typeof(int));
        dt.Columns.Add("Infilt", typeof(int));
        dt.Columns.Add("River", typeof(int));
        dt.Columns.Add("River_ox", typeof(int));
        dt.Columns.Add("FromOther", typeof(int));
        dt.Columns.Add("ToOther", typeof(int));
        dt.Columns.Add("Total_accu", typeof(int));
        dt.Columns.Add("Ox_accu", typeof(int));
        dt.Columns.Add("Frak1", typeof(double));
        dt.Columns.Add("Frak2", typeof(double));
        dt.Columns.Add("Frak3", typeof(double));



        double np = 20.0;
        foreach (var c in AllCatchments.Values)
        {

          //Do the breakthrough curves
          c.Particles = Pr.EndDistribution[c.ID].Where(pa => pa.Registration != 1).ToList();
          if (c.Particles.Count >= 20)
          {
            List<Particle> OutSideParticles = new List<Particle>();
            List<Particle> InSideParticles = new List<Particle>();

            foreach (var pa in c.Particles)
            {
              if (c.Geometry.Contains(pa.XStart, pa.YStart))
                InSideParticles.Add(pa);
              else
                OutSideParticles.Add(pa);
            }
            //All Particles
            Percentile p = new Percentile(c.Particles.Select(pa => pa.TravelTime));
            Percentile poutside = null;
            if (OutSideParticles.Count > 20)
              poutside = new Percentile(OutSideParticles.Select(pa => pa.TravelTime));

            Percentile pinside = new Percentile(InSideParticles.Select(pa => pa.TravelTime));

            List<double> ParticleBreakthroughCurves = new List<double>();
            List<double> InsideBreakthroughCurves = new List<double>();
            List<double> OutsideBreakthroughCurves = new List<double>();

            //1
            for (int i = 1; i < np; i++)
            {
              ParticleBreakthroughCurves.Add(p.Compute(i / np));
              InsideBreakthroughCurves.Add(pinside.Compute(i / np));

              if (poutside != null)
                OutsideBreakthroughCurves.Add(poutside.Compute(i / np));
            }


            using (System.IO.StreamWriter csv = new System.IO.StreamWriter(Path.Combine(OutPutDir, c.ID + "BC.csv")))
            {
              StringBuilder headline = new StringBuilder();
              headline.Append("Type\tNumber of Particles");

              for (int i = 1; i < np; i++)
              {
                headline.Append("\t + " + (i / np * 100.0));
              }
              csv.WriteLine(headline);

                StringBuilder line = new StringBuilder();
                line.Append("All particles\t" + c.Particles.Count);
                foreach (var pe in ParticleBreakthroughCurves)
                  line.Append("\t" + pe);
                csv.WriteLine(line);
              line.Clear();

              line.Append("From this catchment\t" + InSideParticles.Count);
                foreach (var pe in InsideBreakthroughCurves)
                  line.Append("\t" + pe);
                csv.WriteLine(line);
              line.Clear();

              if(OutsideBreakthroughCurves!=null)
              {
                line.Append("From other catchments\t" + OutSideParticles.Count);
                foreach (var pe in OutsideBreakthroughCurves)
                  line.Append("\t" + pe);
                csv.WriteLine(line);
              line.Clear();
              }

              }
            
            //Do the catchment stats
            Geometry.GeoRefData gd = new Geometry.GeoRefData();
            gd.Geometry = c.Geometry;
            gd.Data = dt.NewRow();

            gd.Data[0] = c.ID;
            gd.Data[1] = Pr.StartDistribution[c.ID].Where(pa => pa.Registration != 1).Count();
            gd.Data[2] = Pr.EndDistribution[c.ID].Count;
            gd.Data[3] = c.Particles.Count;
            gd.Data[4] = OutSideParticles.Count;
            gd.Data[5] = Pr.StartDistribution[c.ID].Count(pa => pa.Registration != 1 && !c.Geometry.Contains(pa.X, pa.Y));
            gd.Data[6] = UpstreamCount(c, pa => true);
            gd.Data[7] = UpstreamCount(c, pa => pa.Registration != 1);
            gd.Data[8] = (int)gd.Data[5] / (double)(int)gd.Data[1];
            gd.Data[9] = (int)gd.Data[4] / (double)(int)gd.Data[1];
            gd.Data[10] = (int)gd.Data[7] / (double)(int)gd.Data[6];
            sw.Write(gd);

          }
        }
      }
    }


    private int UpstreamCount(Catchment c, Func<Particle,bool> CountSelector)
    {
      int toreturn = Pr.EndDistribution[c.ID].Count(CountSelector);
      foreach (var upperc in c.UpstreamConnections)
        toreturn += UpstreamCount(upperc, CountSelector);

      return toreturn;
    }


    }
  
}

