using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class InfiniteSource
  {
    [DataMember]
    public IWaterPacket Sample { get; set; }

    public InfiniteSource()
    { }

    public InfiniteSource(IWaterPacket sample)
    {
      Sample = sample;
    }

    public IWaterPacket GetWater(double Volume)
    {      
      if (Sample.GetType() == typeof(WaterPacket))
      {
        WaterPacket W = new WaterPacket(Volume);
        foreach (KeyValuePair<int, double> KVP in Sample.Composition)
        {
          W.Composition.Add(KVP.Key, KVP.Value);
        }
        return W;
      }
      else if (Sample.GetType() == typeof(WaterWithChemicals))
      {
        WaterWithChemicals WCC = new WaterWithChemicals(Volume);
        foreach (KeyValuePair<int, double> KVP in Sample.Composition)
        {
          WCC.Composition.Add(KVP.Key, KVP.Value);
        }
        double factor = Volume / Sample.Volume;
        foreach (KeyValuePair<string, Chemical> KVP in ((WaterWithChemicals)Sample).Chemicals)
          WCC.AddChemical(new Chemical(KVP.Value.Type, KVP.Value.Moles*factor));
        return WCC;
      }
      else
        return null;
    }
  }
}
