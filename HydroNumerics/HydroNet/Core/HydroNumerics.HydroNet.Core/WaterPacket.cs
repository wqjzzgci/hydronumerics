using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class WaterPacket:IWaterPacket
  {
    #region Persisted properties
    [DataMember]
    private double _volume;

    /// <summary>
    /// Gets the temperature
    /// </summary>
    [DataMember]
    public double Temperature { get; protected set; }
    
    [DataMember]
    protected Dictionary<int, double> _composition = new Dictionary<int, double>();
 
    [DataMember]
    public StringBuilder LogString = new StringBuilder();

    /// <summary>
    /// Gets and sets the relative time. This is mainly used internally for time stepping.
    /// Does not change when adding water
    /// </summary>
    [DataMember]
    public TimeSpan RelativeTimeTag { get; private set; }

    /// <summary>
    /// Does not change when adding water.
    /// </summary>
    [DataMember]
    public TimeSpan WaterAge { get; private set; }

    [DataMember]
    private Dictionary<Chemical, double> _chemicals = new Dictionary<Chemical, double>();


    /// <summary>
    /// Sets whether or not to log.
    /// </summary>
    public bool Log { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructs a water object with the volume
    /// Note volume cannot be negative
    /// </summary>
    /// <param name="volume"></param>
    public WaterPacket(double Volume)
    {
      this.Volume = Volume;
      RelativeTimeTag = TimeSpan.Zero;

      Log = false;

      if (Log)
        LogString.AppendLine("Constructed with the volume: " + Volume);
    }

    /// <summary>
    /// Constructs a water volume with a volume and a temperature
    /// </summary>
    /// <param name="Volume"></param>
    /// <param name="Temperatur"></param>
    public WaterPacket(double Volume, double Temperatur)
      : this(Volume)
    {
      this.Temperature = Temperature;
    }

    /// <summary>
    /// Constructs a water object with a volume and ID
    /// Note volume cannot be negative
    /// </summary>
    /// <param name="volume"></param>
    public WaterPacket(int IDForComposition, double Volume)
      : this(Volume)
    {
      this.IDForComposition = IDForComposition;
      if (Log)
        LogString.AppendLine("Constructed with the ID: " + IDForComposition);
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="WaterToCopy"></param>
    public WaterPacket(WaterPacket WaterToCopy)
    {
      RelativeTimeTag = WaterToCopy.RelativeTimeTag;
      WaterAge = WaterToCopy.WaterAge;
      LogString = new StringBuilder(WaterToCopy.LogString.ToString());

      //Copy the properties
      foreach (var KVP in WaterToCopy.Composition)
        Composition.Add(KVP.Key, KVP.Value);

    }

    #endregion

    #region IWater Members

    /// <summary>
    /// Adds enthalpy in joule. Can be negative when cooling
    /// </summary>
    /// <param name="Enthalpy"></param>
    public virtual void AddEnergy(double Enthalpy)
    {
      //WikiPedia 15 °C and 1 atm
      double cp = 4.1855E6; // J/m3*K

      Temperature += Enthalpy / cp / Volume;
    }

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
    /// Adds W to this water. This has no effect on W. However, it would be non-physical to use W subsequently
    /// </summary>
    /// <param name="W"></param>
    public virtual void Add(IWaterPacket W)
    {
      if (Log)
        LogString.AppendLine("Added mass: " + W.Volume);

      if (Volume == 0)
      {
        WaterAge = W.WaterAge;
      }

      Temperature = Temperature * Volume + W.Volume * W.Temperature;

      double Multiplier = Volume;

      Volume += W.Volume;

      Temperature /= Volume;

      Multiplier /= Volume;



      foreach (var key in Composition.Keys.ToArray())
      {
        _composition[key] *= Multiplier;
      }

      foreach (var KVP in W.Composition)
      {
        if (_composition.ContainsKey(KVP.Key))
          _composition[KVP.Key] += KVP.Value*(1-Multiplier);
        else
          _composition.Add(KVP.Key, KVP.Value * (1 - Multiplier));
      }

      foreach (KeyValuePair<Chemical, double> KVP in ((WaterPacket)W)._chemicals)
      {
        if (_chemicals.ContainsKey(KVP.Key))
          _chemicals[KVP.Key] += KVP.Value;
        else
          _chemicals.Add(KVP.Key, KVP.Value);
      }

    }

    /// <summary>
    /// Adds W to this water over the length of the period.
    /// Use this method if the addition is not instantaneous.
    /// </summary>
    /// <param name="W"></param>
    /// <param name="LengthOfPeriod"></param>
    public virtual void Add(IWaterPacket W, TimeSpan LengthOfPeriod)
    {
      Add(W);
    }

    /// <summary>
    /// Evaporates water.
    /// The water Volume is set to 0, if the parameter volume is larger than original water volume.
    /// </summary>
    /// <param name="volume"></param>
    public virtual void Evaporate(double Volume)
    {
      if (Log)
        LogString.AppendLine("Evaporated mass: " + Volume);

      _volume -= Volume;
      //volume cannot be negative. 
      _volume = Math.Max(_volume, 0);
    }

    /// <summary>
    /// Substracts and returns the substracted water.
    /// volume should be positive. Otherwise a water object with zero volume will be returned
    public virtual IWaterPacket Substract(double Volume)
    {
      if (Log)
        LogString.AppendLine("Substracted mass: " + Volume);

      //Remember the volume
      double v1 = this.Volume;

      Volume = Math.Min(this.Volume,Math.Max(0, Volume));

      IWaterPacket W = DeepClone(Volume);

      this.Volume -= Volume;

      double factor = this.Volume / v1;

      //Now adjust the concentrations
      foreach (Chemical c in _chemicals.Keys.ToArray())
        _chemicals[c] *= factor;

      return W;
    }

    public virtual void MoveInTime(TimeSpan TimeStep, Dictionary<Chemical, double> FirstOrderDegradationRates, double SurfaceArea)
    {
      if (Log)
        LogString.AppendLine("MovedInTime " + TimeStep);

      double GasExchangeConstant = 0.16 / 86400; // [m/s] Gleeson et al p.4

      foreach (var c in Chemicals.ToList())
      {
        double FirstOrderDegradationRate = 0;
        if (c.Key.IsVolatile)
        {
          FirstOrderDegradationRate += -GasExchangeConstant * SurfaceArea / Volume;
        }
        if (FirstOrderDegradationRates.ContainsKey(c.Key))
        {
          FirstOrderDegradationRate += -FirstOrderDegradationRates[c.Key];
        }
        Chemicals[c.Key] = Math.Exp(FirstOrderDegradationRate * TimeStep.TotalSeconds) * c.Value;
      }


      WaterAge += TimeStep;
      RelativeTimeTag += TimeStep;
    }


    /// <summary>
    /// Moves the water in time. The surface area is needed because of gas exchange
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="SurfaceArea"></param>
    public virtual void MoveInTime(TimeSpan TimeStep, double SurfaceArea)
    {
      if (Log)
        LogString.AppendLine("MovedInTime " + TimeStep);

      double GasExchangeConstant = 0.16/86400; // [m/s] Gleeson et al p.4

      foreach (var c in Chemicals.ToList())
      {
        double FirstOrderDegradationRate = 0;
        if (c.Key.IsVolatile)
        {
          FirstOrderDegradationRate += -GasExchangeConstant * SurfaceArea / Volume;
        }
        if (c.Key.IsFirstOrderDegradable)
        {
          FirstOrderDegradationRate += -c.Key.FirstOrderDegradationRate;
        }
        Chemicals[c.Key] = Math.Exp(FirstOrderDegradationRate*TimeStep.TotalSeconds ) * c.Value;
      }

      WaterAge += TimeStep;
      RelativeTimeTag += TimeStep;
    }

    public void ResetTime()
    {
      if (Log)
        LogString.AppendLine("TimeReset ");

      RelativeTimeTag = TimeSpan.Zero;
    }

    public double GetConcentration(ChemicalNames Cname)
    {
      return GetConcentration(ChemicalFactory.Instance.GetChemical(Cname));
    }


    /// <summary>
    /// Gets the concentration in Moles/m3;
    /// </summary>
    /// <param name="ChemicalName"></param>
    /// <returns></returns>
    public double GetConcentration(Chemical Name)
    {
      double m;
      if (_chemicals.TryGetValue(Name, out m) & Volume != 0)
        return m / Volume;
      else
        return 0;
    }

    /// <summary>
    /// Sets the concentration in Moles/m3
    /// </summary>
    public void SetConcentration(ChemicalNames Cname, double Concentration)
    {
      SetConcentration(ChemicalFactory.Instance.GetChemical(Cname), Concentration);
    }

    /// <summary>
    /// Sets the concentration in Moles/m3
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Concentration"></param>
    public void SetConcentration(Chemical Name, double Concentration)
    {
      if (_chemicals.ContainsKey(Name) & Volume != 0)
      {
        _chemicals[Name] = Concentration * Volume;
      }
      else
        _chemicals.Add(Name, Concentration * Volume);
    }


    /// <summary>
    /// Tells the waterpacket where it is
    /// The value is stored
    /// </summary>
    /// <param name="ID"></param>
    public void Tag(int ID)
    {
      if (Log)
        LogString.AppendLine("Tagged by " + ID);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the composition of the water
    /// </summary>
    public Dictionary<int, double> Composition
    {
      get
      {
        return _composition;
      }
    }

    /// <summary>
    /// Sets an ID that can be used for tracking
    /// When setting the ID the composition is cleared
    /// </summary>
    public int IDForComposition
    {
      set
      {
        if (Log)
          LogString.AppendLine("ID for Composition set to: " + value);

        _composition.Clear();
        _composition.Add(value, 1);
      }
    }

    /// <summary>
    /// Gets the volume of the water.
    /// </summary>
    public double Volume
    {
      get
      {
        return _volume;
      }

      protected set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("Volume of water cannot be less than zero");
        _volume = value;
      }
    }

    /// <summary>
    /// Returns a deep clone of this waterpacket with the specified volumen.
    /// This is a non-physical method that will destroy mass balance.
    /// Should only be used for data storage and non-physical boundaries.
    /// </summary>
    /// <param name="Volume"></param>
    /// <returns></returns>
    public virtual IWaterPacket DeepClone()
    {
      return DeepClone(Volume);
    }

    /// <summary>
    /// Returns a deep clone of this waterpacket with the specified volumen.
    /// This is a non-physical method that will destroy mass balance.
    /// Should only be used for data storage and non-physical boundaries.
    /// </summary>
    /// <param name="Volume"></param>
    /// <returns></returns>
    public virtual IWaterPacket DeepClone(double Volume)
    {
      WaterPacket W = new WaterPacket(Volume);
      DeepClone(W, Volume);
      return W;
    }

    protected virtual void DeepClone(IWaterPacket CloneToThis, double Volume)
    {
      WaterPacket W = (WaterPacket)CloneToThis;

      double factor = Volume / this.Volume;

      //Copy the properties
      W.RelativeTimeTag = this.RelativeTimeTag;
      W.WaterAge = WaterAge;
      W.Temperature = Temperature;
      W.LogString = new StringBuilder(LogString.ToString());
      foreach (KeyValuePair<int, double> KVP in _composition)
        W._composition.Add(KVP.Key, KVP.Value);

      //Now clone the chemicals
      foreach (KeyValuePair<Chemical, double> KVP in _chemicals)
        W.AddChemical(KVP.Key, KVP.Value * factor);

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


    public override string ToString()
    {
      return "Volume is:" + _volume;
    }

#endregion

  }
}
