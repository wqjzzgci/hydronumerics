using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.Geometry.Shapes;

using HydroNumerics.Core.Time;



namespace HydroNumerics.Nitrate.Model
{
  public class FlowBiasCorrector
  {

    //public void Open(string MSHEFileName)
    //{
      //var excel = new ExcelQueryFactory();
      //excel.FileName = @"d:\temp\Qdata_DK1990-2012.xlsx";

      //var ddh = (from x in excel.Worksheet("Qdata_DK1990-2012")
      //           select x).ToList();


    //  MikeSheTools.Core.Model mshe = new MikeSheTools.Core.Model(MSHEFileName);
    //  var tocorrect = mshe.Results.Mike11Observations.Where(m11o => m11o.Simulation != null & m11o.Observation != null).ToList();


    //  using (StreamWriter sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(MSHEFileName), "ToBiasCorrection.csv"), false, Encoding.Default))
    //  {
    //    sw.WriteLine("DMU-Nummer,Name,CatchmentArea,UTMX,UTMY,Year,Month,Day,QObs,QSim");
    //    foreach (var obs in tocorrect)
    //    {

    //      string dmunummer = obs.Name.ToString();
    //      var station = ddh.FirstOrDefault(r => r[2].Value.ToString() == obs.Name);
          
    //      if (station == null)
    //        station = ddh.FirstOrDefault(r => r[3].Value.ToString() == obs.Name);
          
    //      string oplandsareal = "";
    //      string stationname ="";
    //      if (station != null)
    //      {
    //        dmunummer = station[3].Value.ToString();
    //        oplandsareal = station[4].Value.ToString();
    //        stationname = station[8].Value.ToString();
    //      }

    //      for (int i = 0; i < obs.Observation.Items.Count; i++)
    //      {
    //        {
    //          double sim = obs.Simulation.GetValue(obs.Observation.Items[i].Time, InterpolationMethods.DeleteValue);
    //          if (sim != obs.Simulation.DeleteValue)
    //          {
    //            if (obs.Observation.Items[i].Value != obs.Observation.DeleteValue)
    //              sw.WriteLine(dmunummer + "," + stationname + "," + oplandsareal + "," + obs.Location.X.ToString() + "," + obs.Location.Y + "," + obs.Observation.Items[i].Time.ToString("yyyy,MM,dd") + "," + obs.Observation.Items[i].Value + "," + sim);
    //            else
    //              sw.WriteLine(dmunummer + "," + stationname + "," + oplandsareal + "," + obs.Location.X.ToString() + "," + obs.Location.Y + "," + obs.Observation.Items[i].Time.ToString("yyyy,MM,dd") + ",," + sim);
    //          }
    //        }
    //      }
    //    }
    //  }
    //}



  }
}
