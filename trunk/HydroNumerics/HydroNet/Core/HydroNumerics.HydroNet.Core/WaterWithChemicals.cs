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
    internal Dictionary<ChemicalType, double> _chemicals = new Dictionary<ChemicalType, double>();
    private double _volumeOfNonChemicalWater = 0;

    public WaterWithChemicals(double Volume)
      : base(Volume)
    {
    }

    public WaterWithChemicals(int IDForTracking, double Volume)
      : base(IDForTracking, Volume)
    {
    }


    public void AddChemical(ChemicalType Chemical, double Amount)
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


    public override IWaterPacket Substract(double Volume)
    {

      double factor = this.Volume;
      IWaterPacket WC = base.Substract(Volume);
      WaterWithChemicals WCnew = new WaterWithChemicals(WC.Volume);
      WCnew._composition = WC.Composition;

      factor = WC.Volume / factor;

      foreach (KeyValuePair<string, Chemical> KVP in _chemicals)
      {
        WCnew.AddChemical(KVP.Value.Split(factor));
      }

      return WCnew;
    }


    public override void Add(IWaterPacket W)
    {
      base.Add(W);
      if (W.GetType().Equals(this.GetType()))
      {
        foreach (KeyValuePair<string, Chemical> KVP in ((WaterWithChemicals)W)._chemicals)
        {
          if (_chemicals.ContainsKey(KVP.Key))
            _chemicals[KVP.Key].Moles += KVP.Value.Moles;
          else
            _chemicals.Add(KVP.Key, KVP.Value);
        }
      }
      else
      {
        _volumeOfNonChemicalWater += W.Volume;
      }
    }

    /// <summary>
    /// Gets the concentration in Moles/m3;
    /// </summary>
    /// <param name="ChemicalName"></param>
    /// <returns></returns>
    public double GetConcentration(string ChemicalName)
    {
      Chemical C;
      if (_chemicals.TryGetValue(ChemicalName, out C))
        return C.Moles / Volume;
      else
        return 0;
    }


    /// <summary>
    /// ToDo: this method does not yet work
    /// </summary>
    /// <param name="Volume"></param>
    /// <returns></returns>
    public override IWaterPacket DeepClone(double Volume)
    {
      WaterWithChemicals WCC = new WaterWithChemicals(Volume);
      foreach (KeyValuePair<int, double> KVP in this.Composition)
      {
        WCC.Composition.Add(KVP.Key, KVP.Value);
      }

      double factor = Volume / this.Volume;
      foreach (KeyValuePair<string, Chemical> KVP in ((WaterWithChemicals)this).Chemicals)
        WCC.AddChemical(new Chemical(KVP.Value.Type, KVP.Value.Moles * factor));

      return WCC;
    }

    public Dictionary<string, Chemical> Chemicals
    {
      get
      {
        return _chemicals;
      }
    }
  
  }

}
