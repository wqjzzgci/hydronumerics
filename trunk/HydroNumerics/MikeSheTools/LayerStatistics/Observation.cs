using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.Core;

namespace HydroNumerics.MikeSheTools.LayerStatistics
{
  public class Observation
  {

    public Observation(DateTime time, double Value, MikeSheWell well)
    {
      this.Time = time;
      this.Value = Value;
      Well = well;
    }

    public DateTime Time { get; private set; }
    public double Value { get; private set; }
    public double? SimulatedValueCell { get; set; }
    public double? InterpolatedValue { get; set; }

    public int DryCells { get; set; }
    public int BoundaryCells { get; set; }

    public MikeSheWell Well { get; private set; }

  }
}
