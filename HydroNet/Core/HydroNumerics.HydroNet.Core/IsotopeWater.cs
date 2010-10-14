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

    public IsotopeWater(double Volume)
      : base(Volume)
    {
    }

    public void SetIsotopeRatio(double ratio)
    {
      Chemical iso = ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction);
      AddChemical(iso, ratio * Volume);
    }

    public double GetIsotopeRatio()
    {
      return GetConcentration(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction));
    }

    public override void Evaporate(double Volume)
    {
      Chemical iso = ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction);
      double ConcFactor = .1;
      double vol = this.Volume;
      base.Evaporate(Volume);

      this.Chemicals[iso] *= (1 - Volume / vol) / (1-ConcFactor * Volume / vol); 
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
