using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public abstract class AbstractWaterBody:IDObject
  {
    #region Persisted data
    [DataMember]
    protected List<IWaterBody> _downStreamConnections = new List<IWaterBody>();

    [DataMember]
    protected List<ISource> _sources = new List<ISource>();

    [DataMember]
    protected List<ISource> _precipitationBoundaries = new List<ISource>();

    [DataMember]
    protected List<ISink> _sinks = new List<ISink>();

    [DataMember]
    protected List<IGroundwaterBoundary> _groundwaterBoundaries = new List<IGroundwaterBoundary>(); 

    [DataMember]
    protected List<ISink> _evapoBoundaries = new List<ISink>();

    [DataMember]
    public WaterBodyOutput Output { get; protected set; }

    [DataMember]
    public Observations RealData { 
      get
      {
        if (_realData == null)
          _realData = new Observations(Name);
        return _realData;
      }
      private set
      {
        _realData = value;
      }
    }
    private Observations _realData;

    [DataMember]
    public DateTime CurrentTime { get; protected set; }

    /// <summary>
    /// Gets and sets the depth of the waterbody
    /// </summary
    [DataMember]
    public double Depth { get; set; }

    /// <summary>
    /// Gets and sets the Water level
    /// </summary>
    [DataMember]
    public double WaterLevel { get; set; }

    #endregion

    #region Non-persisted Properties

    /// <summary>
    /// Gets the collection of sources
    /// </summary>
    public Collection<ISource> Sources { get; protected set; }

    /// <summary>
    /// Gets the collection of sources
    /// </summary>
    public Collection<ISink> Sinks { get; protected set; }

    /// <summary>
    /// Gets the collection of precipitation boundaries
    /// </summary>
    public Collection<ISource> Precipitation { get; protected set; }

    /// <summary>
    /// Gets the collection og evaporation boundaries
    /// </summary>
    public Collection<ISink> EvaporationBoundaries { get; protected set; }

    /// <summary>
    /// Gets the collection of Groundwater boundaries
    /// </summary>
    public Collection<IGroundwaterBoundary> GroundwaterBoundaries { get; protected set; }

    /// <summary>
    /// Gets the collection of downstream connections
    /// </summary>
    public Collection<IWaterBody> DownStreamConnections { get; protected set; }


    #endregion

    #region Constructors


    public AbstractWaterBody(string Name)
    {
      this.Name = Name;
      Output = new WaterBodyOutput(Name);
      BindCollections();
    }

    /// <summary>
    /// This mettod is called whenever the object is deserialized
    /// </summary>
    /// <param name="context"></param>
    [OnDeserialized]
    private void ReconnectEvents(StreamingContext context)
    {
      BindCollections();
    }

    /// <summary>
    /// Bind the public collections to the internal lists
    /// </summary>
    private void BindCollections()
    {
      Sources = new Collection<ISource>(_sources);
      Sinks = new Collection<ISink>(_sinks);
      DownStreamConnections = new Collection<IWaterBody>(_downStreamConnections);
      EvaporationBoundaries = new Collection<ISink>(_evapoBoundaries);
      GroundwaterBoundaries = new Collection<IGroundwaterBoundary>(_groundwaterBoundaries);
      Precipitation = new Collection<ISource>(_precipitationBoundaries);

    }



    #endregion

    public virtual void Initialize()
    {
    }



    public DateTime EndTime
    {
      get
      {
        DateTime min1 = DateTime.MaxValue;
        if (Sources.Count != 0)
          min1 = Sources.Min(var => var.EndTime);
        DateTime min2 = DateTime.MaxValue;
        if (EvaporationBoundaries.Count!=0)
          min2 = EvaporationBoundaries.Min(var => var.EndTime);
        if (min1 < min2)
          return min1;
        else
          return min2;
      }
    }

    /// <summary>
    /// Distributes water on downstream connections.
    /// </summary>
    /// <param name="Water"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    protected void SendWaterDownstream(IWaterPacket Water, DateTime Start, DateTime End)
    {
      //Send water to downstream recipients
      if (_downStreamConnections.Count == 1)
        _downStreamConnections[0].AddWaterPacket(Start, End, Water);
      else if (_downStreamConnections.Count > 1)
      {
        double fraction = Water.Volume/_downStreamConnections.Count;
        foreach (IWaterBody IW in _downStreamConnections)
          IW.AddWaterPacket(CurrentTime, End, Water.Substract(fraction));
      }

    }

    public virtual void ResetToTime(DateTime Time)
    {
      foreach (var SI in Sources)
        SI.ResetOutputTo(Time);
      foreach (var SI in Sinks)
        SI.ResetOutputTo(Time);
      foreach (var SI in GroundwaterBoundaries)
        SI.ResetOutputTo(Time);
      foreach (var SI in EvaporationBoundaries)
        SI.ResetOutputTo(Time);
      foreach (var SI in Precipitation)
        SI.ResetOutputTo(Time);

      Output.ResetToTime(CurrentTime);
    }

    public virtual void AddDownStreamWaterBody(IWaterBody waterbody)
    {
      DownStreamConnections.Add(waterbody);
    }

    /// <summary>
    /// Gets the average storage time for the time period. Calculated as mean volume divide by mean (sinks + outflow + evaporation)
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public TimeSpan GetStorageTime(DateTime Start, DateTime End)
    {
      if (!Output.IsEmpty)
      {
        if (Output.EndTime < End || Output.StartTime > Start)
          throw new Exception("Cannot calculate storage time outside of the simulated period");

        //Find the total outflow
        double d = Output.Sinks.GetSiValue(Start, End);
        d += Output.Outflow.GetSiValue(Start, End);
        //Evaporation is negative
        d += Output.Evaporation.GetSiValue(Start, End);
        d += Output.GroundwaterOutflow.GetSiValue(Start, End);

        return TimeSpan.FromSeconds(Output.StoredVolume.GetSiValue(Start, End) / d);
      }
      return TimeSpan.Zero;
    }


    public override string ToString()
    {
      return Name;
    }
  }
}

