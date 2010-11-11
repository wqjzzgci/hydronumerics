using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class Chemical
  {
    [DataMember]
    public string Name { get; private set; }
    [DataMember]
    public double MolarWeight { get; private set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public bool IsVolatile { get; set; }
    [DataMember]
    public bool IsFirstOrderDegradable { get; set; }

    [DataMember]
    public double FirstOrderDegradationRate { get; set; }

    public Chemical(string Name, double MolarWeight)
    {
      this.Name = Name;
      this.MolarWeight = MolarWeight;
      IsVolatile = false;
      IsFirstOrderDegradable = false;
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
