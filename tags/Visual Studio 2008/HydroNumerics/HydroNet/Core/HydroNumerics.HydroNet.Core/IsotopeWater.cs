using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public sealed class IsotopeWater:WaterPacket 
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

    /// <summary>
    /// Gets and sets the concentration in the evaporated water.
    /// </summary>
    public TimestampSeries EvaporationConcentration { get; set; }

    private double _currentEvaporationConcentration = 0;
    /// <summary>
    /// Gets and sets the current time
    /// It is necessary for the Isotopewater to know the time because the evaporation concentration my vary over time
    /// </summary>
    public DateTime CurrentTime { get; set; }

    public override void Evaporate(double Volume)
    {
      Chemical iso = ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction);
      base.Evaporate(Volume);

      this.Chemicals[iso] -= Volume * _currentEvaporationConcentration;  
    }


    public override void MoveInTime(TimeSpan TimeStep, Dictionary<Chemical, double> FirstOrderDegradationRates, double SurfaceArea)
    {
      base.MoveInTime(TimeStep, FirstOrderDegradationRates, SurfaceArea);
      CurrentTime += TimeStep;
      if (EvaporationConcentration != null)
        _currentEvaporationConcentration = EvaporationConcentration.GetSiValue(CurrentTime);
    }

    public override void MoveInTime(TimeSpan TimeStep, double SurfaceArea)
    {
      base.MoveInTime(TimeStep, SurfaceArea);
      CurrentTime += TimeStep;
      if (EvaporationConcentration!=null)
        _currentEvaporationConcentration = EvaporationConcentration.GetSiValue(CurrentTime);
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
      if (this.EvaporationConcentration!=null)
        WCC.EvaporationConcentration = new TimestampSeries(this.EvaporationConcentration);
      WCC._currentEvaporationConcentration = this._currentEvaporationConcentration;
      WCC.CurrentTime = this.CurrentTime;
      base.DeepClone(WCC, Volume);
      return WCC;
    }
  }
}
