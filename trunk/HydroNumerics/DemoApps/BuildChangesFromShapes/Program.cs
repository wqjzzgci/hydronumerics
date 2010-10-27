using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using HydroNumerics.Geometry.Shapes;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;
using HydroNumerics.MikeSheTools.WellViewer;

namespace BuildChangesFromShapes
{
  class Program
  {
    static void Main(string[] args)
    {
      Reader r = new Reader(@"C:\Users\Jacob\Projekter\JupiterPlus\nov12.mdb");

      var WellsNew = r.WellsForNovana(false, false, false, false);
      var PlantsNew = r.ReadPlants(WellsNew);

      Dictionary<string, IWell> OldWells = new Dictionary<string, IWell>();
      Dictionary<int, Plant> OldPlants = new Dictionary<int, Plant>();

      ShapeReader sr = new ShapeReader(@"C:\Users\Jacob\Projekter\JupiterPlus\Novomr12_indv_ks_15feb2010.shp");
      var DT = sr.Data.Read();
      ShapeReaderConfiguration ShpConfig;
      XmlSerializer x = new XmlSerializer(typeof(ShapeReaderConfiguration));


      using (FileStream fs = new FileStream(@"C:\Program Files (x86)\MikeSheWrapper\bin\ShapeReaderConfig.xml", FileMode.Open))
      {
        ShpConfig = (ShapeReaderConfiguration)x.Deserialize(fs);

        HeadObservations.FillInFromNovanaShape(DT.Select(), ShpConfig, OldWells, OldPlants);
      }

      ChangeWriter cw = new ChangeWriter();


      //Loop the wells;
      foreach (var kvp in OldWells)
      {
        IWell newWell;

        if (WellsNew.TryGetValue(kvp.Key, out newWell))
        {
          if (kvp.Value.X != newWell.X)
          {
            Change<double> c = new Change<double>();
            c.User = "JAG";
            c.Project = "KS15FEB";
            c.Date = new DateTime(2010, 1, 15);
            c.NewValue = kvp.Value.X;
            c.OldValue = newWell.X;
            cw.AddWellX(kvp.Key, c);
          }
          if (kvp.Value.Y != newWell.Y)
          {
            Change<double> c = new Change<double>();
            c.User = "JAG";
            c.Project = "KS15FEB";
            c.Date = new DateTime(2010, 1, 15);
            c.NewValue = kvp.Value.Y;
            c.OldValue = newWell.Y;
            cw.AddWellY (kvp.Key, c);
          }
          if (kvp.Value.Terrain != newWell.Terrain )
          {
            Change<double> c = new Change<double>();
            c.User = "JAG";
            c.Project = "KS15FEB";
            c.Date = new DateTime(2010, 1, 15);
            c.NewValue = kvp.Value.Terrain ;
            c.OldValue = newWell.Terrain;
            cw.AddWellTerrain(kvp.Key, c);
          }
        }
      }

      //Loop the plants
      foreach (var kvp in OldPlants)
      {
        Plant newPLant;
        if (PlantsNew.TryGetValue(kvp.Key,out newPLant))
        {
          foreach (var I in kvp.Value.PumpingIntakes)
          {
            if (!newPLant.PumpingIntakes.Contains(I))
            {
              I.
            }

          }

        }

      }


      cw.Save(@"c:\temp\xchange");


    }
  }
}
