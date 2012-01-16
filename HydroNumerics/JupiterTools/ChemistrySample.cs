using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HydroNumerics.JupiterTools
{
  [DataContract]
  public class ChemistrySample
  {
    [DataMember]
    public int SampleID { get; set; }
    [DataMember]
    public DateTime SampleDate { get; set; }
    [DataMember]
    public int CompoundNo { get; set; }
    [DataMember]
    public string CompoundName { get; set; }
    [DataMember]
    public int? Unit { get; set; }
    [DataMember]
    public double Amount { get; set; }

    [DataMember]
    public string Description { get; set; }

    public ChemistrySample()
    { }

    public override string ToString()
    {
      return SampleDate.ToShortDateString() + ": "+ Amount +" "+ CompoundName;
    }
  }
}
