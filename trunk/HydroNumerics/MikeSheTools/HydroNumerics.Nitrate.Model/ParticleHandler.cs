using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class ParticleHandler
  {

    public static void ReadAll(string Filename)
    {
      List<Particle> ids = new List<Particle>();

      MainViewModel mvm = new MainViewModel();
      mvm.LoadCatchments(@"D:\DK_information\id15_NSTmodel\id15_NSTmodel.shp");

      foreach (var c in mvm.AllCatchments.Values)
      {
        var b = c.Geometry.BoundingBox;
      }

    using (ShapeReader sr = new ShapeReader(Filename))
    {
      //foreach (var v in sr.GeoData)
      //{
      //  ids.Add((int)v.Data[0]);
      //}


      for (int i = 0; i < sr.Data.NoOfEntries; i++)
      {
        double x = sr.Data.ReadDouble(i,"X-Reg");
        double y = sr.Data.ReadDouble(i, "Y-Reg");

        Particle p = new Particle();
        p.X = x;
        p.Y = y;
        p.StartX = sr.Data.ReadInt(i, "IX-Birth");
        p.StartY = sr.Data.ReadInt(i, "IY-Birth");
        p.EndX = sr.Data.ReadInt(i, "IX-Reg");
        p.EndY = sr.Data.ReadInt(i, "IY-Reg");
        p.TravelTime = sr.Data.ReadDouble(i, "TravelTime");
        ids.Add(p);
      }

      Parallel.ForEach(ids.Take(100000), new ParallelOptions(){ MaxDegreeOfParallelism=7},
        (p) =>
        {
          foreach (var c in mvm.AllCatchments.Values)
          {
            if (c.Geometry.Contains(p.X, p.Y))
              break;
          }
        }
      );



      //while (!sr.Data.EndOfData)
      //{
      // ids.Add((int) sr.Data.ReadNext()[0]);
      //}
    }
    int k = ids.Count;
    }
  }
}
