using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class TSBrtEntry
  {
    public TimeSpan Time { get; set; }
    public double Temperature { get; set; }
    public double WaterSaturation { get; set; }
    public double NAPLSaturation { get; set; }
    public double Pressure { get; set; }
    public double WaterGasCapillaryPressure { get; set; }
    public double VOCGasPhase { get; set; }
    public double VOCWaterPhase { get; set; }
    public double AirPartialPressure { get; set; }
    public double AirInWater { get; set; }
    public double RelativePermeabilityGas { get; set; }
    public double RelativePermeabilityWater { get; set; }
    public double RelativePermeabilityNAPL { get; set; }
    public double DensityGas { get; set; }


    public TSBrtEntry(TimeSpan time, double[] values)
    {
      Time = time;
      Temperature = values[0];
      WaterSaturation = values[1];
      NAPLSaturation = values[2];
      Pressure = values[3];
      WaterGasCapillaryPressure = values[4];
      VOCGasPhase = values[5];
      VOCWaterPhase = values[6];
      AirPartialPressure = values[7];
      AirInWater = values[8];
      RelativePermeabilityGas = values[9];
      RelativePermeabilityWater = values[10];
      RelativePermeabilityNAPL = values[11];
      DensityGas = values[12];
    }
  }
}
