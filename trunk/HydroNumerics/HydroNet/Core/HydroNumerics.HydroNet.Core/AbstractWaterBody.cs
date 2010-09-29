using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public abstract class AbstractWaterBody
  {
    #region Persisted data
    [DataMember]
    protected List<IWaterBody> _downStreamConnections = new List<IWaterBody>();

    [DataMember]
    protected List<ISource> _sources = new List<ISource>();

    [DataMember]
    protected List<ISink> _sinks = new List<ISink>();

    [DataMember]
    protected List<IGroundwaterBoundary> _groundwaterBoundaries = new List<IGroundwaterBoundary>(); 

    [DataMember]
    protected List<ISink> _evapoBoundaries = new List<ISink>();

    [DataMember]
    protected List<GeoExchangeItem> _exchangeItems;


    [DataMember]
    public int ID { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public WaterBodyOutput Output { get; protected set; }

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

    /// <summary>
    /// Gets the collection of exchange items
    /// </summary>
    public Collection<GeoExchangeItem> ExchangeItems { get; protected set; }


    #endregion

    #region Constructors


    public AbstractWaterBody()
    {
      Output = new WaterBodyOutput(ID.ToString());
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

    }



    #endregion

    public virtual void Initialize()
    {
      _exchangeItems = new List<GeoExchangeItem>();
      foreach (var v in Sinks.Select(var => var.ExchangeItems))
        if (v!=null)
          _exchangeItems.AddRange(v);
      foreach (var v in Sources.Select(var => var.ExchangeItems))
        if (v != null) 
          _exchangeItems.AddRange(v);
      foreach (var v in GroundwaterBoundaries.Select(var => var.ExchangeItems))
        if (v != null) 
          _exchangeItems.AddRange(v);
      foreach (var v in EvaporationBoundaries.Select(var => var.ExchangeItems))
        if (v != null) 
          _exchangeItems.AddRange(v);

      ExchangeItems = new Collection<GeoExchangeItem>(_exchangeItems);
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
    /// Distributes water on downstream connections. Also logs chemical concentrations
    /// </summary>
    /// <param name="Water"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    protected void SendWaterDownstream(IWaterPacket Water, DateTime Start, DateTime End)
    {
      if(Water.GetType().Equals(typeof(WaterWithChemicals)))
        foreach (KeyValuePair<Chemical, TimespanSeries> ct in Output.ChemicalsToLog)
        {
          ct.Value.AddSiValue(Start, End, ((WaterWithChemicals)Water).GetConcentration(ct.Key));
        }

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

      Output.ResetToTime(CurrentTime);
    }

    public virtual void AddDownStreamWaterBody(IWaterBody waterbody)
    {
      DownStreamConnections.Add(waterbody);
    }

    public override string ToString()
    {
      return Name;
    }
  }
}

