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
      Reader r = new Reader(@"C:\Users\Jacob\Projekter\JupiterPlus\DataFraAnker\novomr12_15feb2010.mdb");

      var WellsFromDB = r.WellsForNovana(false, false, false, false);
      var PlantsFromDB = r.ReadPlants(WellsFromDB);

      Dictionary<string, IWell> WellsFromShape = new Dictionary<string, IWell>();
      Dictionary<int, Plant> PlantsFromShape = new Dictionary<int, Plant>();

      ShapeReader sr = new ShapeReader(@"C:\Users\Jacob\Projekter\JupiterPlus\DataFraAnker\Novomr12_indv_ks_15feb2010.shp");
      var DT = sr.Data.Read();
      ShapeReaderConfiguration ShpConfig;
      XmlSerializer x = new XmlSerializer(typeof(ShapeReaderConfiguration));


      using (FileStream fs = new FileStream(@"C:\Users\Jacob\Work\HydroNumerics\MikeSheTools\WellViewer\ShapeReaderConfig.xml", FileMode.Open))
      {
        ShpConfig = (ShapeReaderConfiguration)x.Deserialize(fs);

        HeadObservations.FillInFromNovanaShape(DT.Select(), ShpConfig, WellsFromShape, PlantsFromShape);
      }

      ChangeWriter cw = new ChangeWriter();
      cw.AddChangeItem("JACOB","NOVANA", new DateTime(2010,02,15));


      //Loop the wells;
      foreach (var kvp in WellsFromShape)
      {
        IWell DBWell;

        if (WellsFromDB.TryGetValue(kvp.Key, out DBWell))
        {
          if ((kvp.Value.X  - DBWell.X ) > 0.1)
          {
            cw.AddWellX(DBWell.ID, DBWell.X);
          }
          if (kvp.Value.Y != DBWell.Y)
          {
            cw.AddWellY(DBWell.ID, DBWell.Y);
          }

          if (kvp.Value.Terrain != DBWell.Terrain)
          {
            cw.AddWellTerrain(DBWell.ID, DBWell.Terrain);
          }

          //There can be more intakes in the database because the shape only contains active pumping intakes
          if (kvp.Value.Intakes.Count() > DBWell.Intakes.Count())
          {
            string n = "Nogether";
          }


        }
        else
        {
          string k_ = "what the fuck";
        }
      }

      //Loop the plants
      foreach (var kvp in PlantsFromShape)
      {
        Plant newPLant;
        if (PlantsFromDB.TryGetValue(kvp.Key,out newPLant))
        {
          foreach (var I in kvp.Value.PumpingIntakes)
          {
            //if (!newPLant.PumpingIntakes.fin
            {
              string k = "r";

            }

          }

        }

      }


      cw.Save(@"c:\temp\xchange");


    }
  }
}
