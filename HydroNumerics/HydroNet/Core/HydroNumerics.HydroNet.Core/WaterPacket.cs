using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class WaterPacket:IWaterPacket 
  {
    private double _volume;
    protected Dictionary<int, double> _composition = new Dictionary<int, double>();
    public StringBuilder LogString = new StringBuilder();

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

      LogString.AppendLine("Constructed with the volume: " + Volume);
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
      LogString.AppendLine("Constructed with the ID: " + IDForComposition);

    }

    #endregion

    #region IWater Members

    /// <summary>
    /// Gets and sets the relative time. This is mainly used internally for time stepping.
    /// Does not change when adding water
    /// </summary>
    public TimeSpan RelativeTimeTag { get; private set; }

    /// <summary>
    /// Does not change when adding water.
    /// </summary>
    public TimeSpan WaterAge { get; private set; }

    /// <summary>
    /// Adds W to this water.
    /// </summary>
    /// <param name="W"></param>
    public virtual void Add(IWaterPacket W)
    {
      LogString.AppendLine("Added mass: " + W.Volume);

      if (Volume == 0)
      {
        WaterAge = W.WaterAge;
      }

      double Multiplier = Volume;

      Volume += W.Volume;

      Multiplier /= Volume;

      foreach (var key in Composition.Keys.ToArray())
      {
        _composition[key] *= Multiplier;
      }

      foreach (KeyValuePair<int, double> KVP in W.Composition)
      {
        if (_composition.ContainsKey(KVP.Key))
          _composition[KVP.Key] += KVP.Value*(1-Multiplier);
        else
          _composition.Add(KVP.Key, KVP.Value * (1 - Multiplier));
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
    public void Evaporate(double Volume)
    {
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
      LogString.AppendLine("Substracted mass: " + Volume);

      Volume = Math.Max(0, Volume);

      //If the volume to substract is larger than water volume, it will substract all water.
      if (Volume >= this.Volume)
      {
        Volume = this.Volume;
        this.Volume = 0;
      }
      else
        this.Volume -= Volume;

      WaterPacket W = new WaterPacket(Volume);
      //Copy the properties
      W.RelativeTimeTag = this.RelativeTimeTag;
      W.WaterAge = WaterAge;
      foreach (KeyValuePair<int, double> KVP in _composition)
        W._composition.Add(KVP.Key, KVP.Value);

      return W;

    }

    public void MoveInTime(TimeSpan TimeStep)
    {
      LogString.AppendLine("MovedInTime " + TimeStep);

      WaterAge += TimeStep;
      RelativeTimeTag += TimeStep;
    }

    public void ResetTime()
    {
      LogString.AppendLine("TimeReset ");

      RelativeTimeTag = TimeSpan.Zero;
    }

    /// <summary>
    /// Tells the waterpacket where it is
    /// The value is stored
    /// </summary>
    /// <param name="ID"></param>
    public void Tag(int ID)
    {
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

    public override string ToString()
    {
      return "Volume is:" + _volume;
    }

#endregion



  }
}
