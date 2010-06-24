using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public sealed class IsotopeWater:WaterWithChemicals 
  {
      [DataMember]
    Chemical iso = ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction);

    public IsotopeWater(double Volume)
      : base(Volume)
    {
    }

    public void SetIsotopeRatio(double ratio)
    {
      AddChemical(iso, ratio);
    }

    public override void Evaporate(double Volume)
    {
      double ConcFactor = 2;
      double vol = this.Volume;
      base.Evaporate(Volume);
      this.Chemicals[iso] /= (ConcFactor * vol / Volume); 
    }

    public override IWaterPacket DeepClone()
    {
      return DeepClone(this.Volume);
    }


    
    /// <summary>
    /// returns a deep clone with a certain volume
    /// </summary>
    /// <param name="Volume"></param>
    /// <returns></returns>
    public override IWaterPacket DeepClone(double Volume)
    {
      IsotopeWater WCC = new IsotopeWater(Volume);
      base.DeepClone(WCC, Volume);
      return WCC;
    }
  }
}
