using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace HydroNumerics.JupiterTools
{
  [DataContract]
  public class CompoundDescription
  {

    [DataMember]
    public int CompundNumber { get; set; }

    [DataMember]
    public string CompoundName { get; set; }

    [DataMember]
    public double DrinkingWaterLimit { get; set; }

    [DataMember]
    public string DringingWaterlimitUnit { get; set; }

  }
}
