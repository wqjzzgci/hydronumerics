using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class TSEntry
  {
    public TimeSpan Time {get;set;}
    public double Pressure { get; set; }
    public double Temperature { get; set; }
    public double GasCO2 { get; set; }
    public double LiquidCO2 { get; set; }
    public double DissolvedCO2 { get; set; }


    public TSEntry(TimeSpan time, double[] values)
    {
      Time = time;
      Pressure = values[0];
      Temperature = values[1];
      GasCO2 = values[2];
      LiquidCO2 = values[3];
      DissolvedCO2 = values[4];
    }
  }
}
