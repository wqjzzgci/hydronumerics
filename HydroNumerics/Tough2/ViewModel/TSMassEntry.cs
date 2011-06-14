using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class TSMassEntry
  {
    public TimeSpan Time { get; set; }
    public double WaterVolume { get; set; }
    public double VOCSorbed { get; set; }
    public double VOCMass { get; set; }
    public double NAPLMass { get; set; }
    public double TotalEnthalpy { get; set; }

    public TSMassEntry(TimeSpan time, double[] values)
    {
      Time = time;
      WaterVolume = values[0];
      VOCSorbed = values[1];
      VOCMass = values[2];
      NAPLMass = values[3];
      TotalEnthalpy = values[4];
    }
  }
}
