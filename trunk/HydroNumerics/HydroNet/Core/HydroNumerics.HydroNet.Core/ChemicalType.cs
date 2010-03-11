using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class ChemicalType
  {
    public string Name { get; private set; }
    public double MolarWeight { get; private set; }
    public string Description { get; set; }

    public ChemicalType(string Name, double MolarWeight)
    {
      this.Name = Name;
      this.MolarWeight = MolarWeight;
    }
  }
}
