using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Rock
  {
    public string Name { get; set; }
    public double Density { get; set; }
    public double Porosity { get; set; }
    public double PermX { get; set; }
    public double PermY { get; set; }
    public double PermZ { get; set; }
    public double WetHeatConductivity { get; set; }
    public double HeatCapacity { get; set; }

    public double Compressibility { get; set; }

    public int RelativePermeabilityModel { get; set; }
    public double[] RelativePermebilityParameters { get; set; }
    public int CapillaryPressureModel { get; set; }
    public double[] CapillaryPressureParameters { get; set; }

    public override string ToString()
    {
      return Name;
    }

    public override bool Equals(object obj)
    {
      return Name.Equals(((Rock)obj).Name);
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }
  }
}
