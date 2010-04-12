using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{

  /// <summary>
  /// Provides conservative transport
  /// </summary>
  public class WaterWithChemicals:WaterPacket
  {
    internal Dictionary<Chemical, double> _chemicals = new Dictionary<Chemical, double>();
    private double _volumeOfNonChemicalWater = 0;

    public WaterWithChemicals(double Volume)
      : base(Volume)
    {
    }

    public WaterWithChemicals(int IDForTracking, double Volume)
      : base(IDForTracking, Volume)
    {
    }


    public void AddChemical(Chemical Chemical, double Amount)
    {
      double d;
      if (_chemicals.TryGetValue(Chemical, out d))
      {
        d += Amount;
        _chemicals[Chemical] = d;
      }
      else
        _chemicals.Add(Chemical, Amount);
    }


    public override void Add(IWaterPacket W)
    {
      base.Add(W);
      if (W.GetType().Equals(this.GetType()))
      {
        foreach (KeyValuePair<Chemical, double> KVP in ((WaterWithChemicals)W)._chemicals)
        {
          if (_chemicals.ContainsKey(KVP.Key))
            _chemicals[KVP.Key] += KVP.Value;
          else
            _chemicals.Add(KVP.Key, KVP.Value);
        }
      }
      else
      {
        _volumeOfNonChemicalWater += W.Volume;
      }
    }

    public override IWaterPacket Substract(double Volume)
    {
      double v1 = this.Volume;

      IWaterPacket w = base.Substract(Volume);
      double factor = this.Volume / v1;

      foreach (Chemical c in _chemicals.Keys.ToArray())
        _chemicals[c] *= factor;

      return w;
    }

    /// <summary>
    /// Gets the concentration in Moles/m3;
    /// </summary>
    /// <param name="ChemicalName"></param>
    /// <returns></returns>
    public double GetConcentration(Chemical ChemicalName)
    {
      double m;
      if (_chemicals.TryGetValue(ChemicalName, out m))
        return m / Volume;
      else
        return 0;
    }



    public override IWaterPacket DeepClone()
    {
      return DeepClone(Volume);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Volume"></param>
    /// <returns></returns>
    public override IWaterPacket DeepClone(double Volume)
    {
      WaterWithChemicals WCC = new WaterWithChemicals(Volume);
      double factor = Volume / this.Volume;

      base.DeepClone(WCC);
      foreach (KeyValuePair<Chemical,double> KVP in _chemicals)
        WCC.AddChemical(KVP.Key, KVP.Value * factor);

      return WCC;
    }

    public Dictionary<Chemical, double> Chemicals
    {
      get
      {
        return _chemicals;
      }
    }
  
  }

}
