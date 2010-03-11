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
    internal Dictionary<string, Chemical> _chemicals = new Dictionary<string, Chemical>();
    private double _volumeOfNonChemicalWater = 0;

    public WaterWithChemicals(double Volume)
      : base(Volume)
    {
    }

    public WaterWithChemicals(int IDForTracking, double Volume)
      : base(IDForTracking, Volume)
    {
    }


    public void AddChemical(Chemical chem)
    {
      _chemicals.Add(chem.Type.Name, chem);
    }


    public override IWaterPacket  Substract(double Volume)
    {

      double factor = this.Volume;
      IWaterPacket WC = base.Substract(Volume);
     WaterWithChemicals WCnew = new WaterWithChemicals(WC.Volume);
     WCnew._composition = WC.Composition;

     factor = WC.Volume/factor;

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
    /// Gets the concentration in Moles/kg;
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

    public Dictionary<string, Chemical> Chemicals
    {
      get
      {
        return _chemicals;
      }
    }
  
  }

}
