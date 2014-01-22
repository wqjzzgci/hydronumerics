using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.Core;

using HydroNumerics.Time2;

namespace HydroNumerics.Nitrate.Model
{
  public class FlowBiasCorrector
  {

    public void Open(string MSHEFileName)
    {
      MikeSheTools.Core.Model mshe = new MikeSheTools.Core.Model(MSHEFileName);
      var tocorrect = mshe.Results.Mike11Observations.Where(m11o => m11o.Simulation != null & m11o.Observation != null).ToList();

      using (StreamWriter sw = new StreamWriter(Path.Combine(Path.GetDirectoryName(MSHEFileName), "M11Monthly.csv")))
      {
        foreach (var obs in tocorrect)
        {
          var tssim = TSTools.ChangeZoomLevel(obs.Simulation, TimeStepUnit.Month, true);
          var tsobs = TSTools.ChangeZoomLevel(obs.Observation, TimeStepUnit.Month, true);

          if (tssim.StartTime < tsobs.StartTime)
            tsobs.Items.Insert(0, new TimeStampValue(tssim.StartTime, tsobs.DeleteValue));
          if (tssim.EndTime > tsobs.EndTime)
            tsobs.Items.Add(new TimeStampValue(tssim.EndTime, tsobs.DeleteValue));

          tsobs.GapFill(InterpolationMethods.DeleteValue);

          for (int i = 0; i < tssim.Items.Count; i++)
          {
            if (tssim.Items[i].Value != obs.Simulation.DeleteValue && tsobs.Items[i].Value != tsobs.DeleteValue)
              sw.WriteLine(tssim.Items[i].Time.ToString("yyyy\tMM\tdd") + "\t" + obs.Name + "\t" + obs.Location.X.ToString() + "\t" + obs.Location.Y + "\t" + tsobs.Items[i].Value + "\t" + tssim.Items[i].Value);
          }
        }
      }
    }



  }
}
