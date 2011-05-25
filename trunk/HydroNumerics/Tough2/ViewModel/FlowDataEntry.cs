using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class FlowDataEntry
  {
    public TimeSpan Time {get;set;}
    public double Water { get; set; }
    public double LiquidCO2 { get; set; }
    public double GasCO2 { get; set; }
    public double TotalCO2 { get; set; }


    public FlowDataEntry(TimeSpan time, double[] values)
    {
      Time = time;
      Water = values[0];
      LiquidCO2 = values[1];
      GasCO2 = values[2];
      TotalCO2 = values[3];
    }
  }
}
