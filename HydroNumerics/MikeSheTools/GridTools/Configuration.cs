using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace GridTools
{

  public enum GridOperation
  {
    LayerSummation,
    FactorMath,
    MonthlyMath,
    GridMath,
    TimeSummation
  }

  public enum Time
  {
    Week,
    Month,
    Year
  }

 

  [DataContract]
  public class Configuration
  {
    [DataMember]
    public string DFS3FileName { get; set; }
    [DataMember]
    public int[] Items { get; set; }
    [DataMember]
    public int[] Layers { get; set; }
    [DataMember]
    public int[] TimeSteps { get; set; }
    [DataMember]
    public string OutputDFS2FileName { get; set; }
  }
}
