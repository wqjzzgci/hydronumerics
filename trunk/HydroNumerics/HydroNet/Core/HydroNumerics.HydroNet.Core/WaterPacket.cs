﻿using System;
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

    #endregion

    #region IWater Members


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

      Volume = Math.Min(this.Volume,Math.Max(0, Volume));

      IWaterPacket W = DeepClone(Volume);

      this.Volume -= Volume;

      return W;
    }

    public void MoveInTime(TimeSpan TimeStep)
    {
      if (Log)
        LogString.AppendLine("MovedInTime " + TimeStep);

      WaterAge += TimeStep;
      RelativeTimeTag += TimeStep;
    }

    public void ResetTime()
    {
      if (Log)
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
      DeepClone(W);
      return W;
    }

    protected virtual void DeepClone(IWaterPacket CloneToThis)
    {
      WaterPacket W = (WaterPacket)CloneToThis;
      //Copy the properties
      W.RelativeTimeTag = this.RelativeTimeTag;
      W.WaterAge = WaterAge;
      W.LogString = new StringBuilder(LogString.ToString());
      foreach (KeyValuePair<int, double> KVP in _composition)
        W._composition.Add(KVP.Key, KVP.Value);
    }

    public override string ToString()
    {
      return "Volume is:" + _volume;
    }

#endregion

  }
}
