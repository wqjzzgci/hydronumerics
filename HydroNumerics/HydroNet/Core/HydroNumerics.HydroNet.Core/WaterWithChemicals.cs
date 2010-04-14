using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{

  /// <summary>
  /// Provides conservative transport
  /// </summary>
  [DataContract]
  public class WaterWithChemicals:WaterPacket
  {
    #region Persisted properties
    [DataMember]
    private Dictionary<Chemical, double> _chemicals = new Dictionary<Chemical, double>();
    [DataMember]
    private double _volumeOfNonChemicalWater = 0;

    #endregion

    #region Constructors
    public WaterWithChemicals(double Volume)
      : base(Volume)
    {
    }

    public WaterWithChemicals(int IDForTracking, double Volume)
      : base(IDForTracking, Volume)
    {
    }

    #endregion
    /// <summary>
    /// Adds a chemical to the water
    /// </summary>
    /// <param name="Chemical"></param>
    /// <param name="Amount"></param>
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

    /// <summary>
    /// Adds water
    /// </summary>
    /// <param name="W"></param>
    public override void Add(IWaterPacket W)
    {
      base.Add(W);
      //If we have chemicals add them
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
        //Keep track of how much non-chemical water is added.
        _volumeOfNonChemicalWater += W.Volume;
      }
    }

    /// <summary>
    /// Substracts a volume of water
    /// </summary>
    /// <param name="Volume"></param>
    /// <returns></returns>
    public override IWaterPacket Substract(double Volume)
    {
      //Remember the volume
      double v1 = this.Volume;

      //Use the base method. This method will call inherited DeepClone method
      IWaterPacket w = base.Substract(Volume);

      double factor = this.Volume / v1;

      //Now adjust the concentrations
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
      if (_chemicals.TryGetValue(ChemicalName, out m) & Volume !=0)
        return m / Volume;
      else
        return 0;
    }
    
    /// <summary>
    /// Returns a deep clone
    /// </summary>
    /// <returns></returns>
    public override IWaterPacket DeepClone()
    {
      return DeepClone(Volume);
    }

    /// <summary>
    /// returns a deep clone with a certain volume
    /// </summary>
    /// <param name="Volume"></param>
    /// <returns></returns>
    public override IWaterPacket DeepClone(double Volume)
    {
      WaterWithChemicals WCC = new WaterWithChemicals(Volume);
      double factor = Volume / this.Volume;

      //DeepClone the properties of the base clas
      base.DeepClone(WCC);
      //Now clone the chemicals
      foreach (KeyValuePair<Chemical,double> KVP in _chemicals)
        WCC.AddChemical(KVP.Key, KVP.Value * factor);

      return WCC;
    }
    
    /// <summary>
    /// Gets the dictionary with chemicals.
    /// </summary>
    public Dictionary<Chemical, double> Chemicals
    {
      get
      {
        return _chemicals;
      }
    }


  
  }

}
