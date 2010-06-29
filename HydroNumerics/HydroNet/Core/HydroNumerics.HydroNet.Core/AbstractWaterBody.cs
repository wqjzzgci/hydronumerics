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
    protected List<IWaterSinkSource> _sinkSources = new List<IWaterSinkSource>();

    [DataMember]
    protected List<IEvaporationBoundary> _evapoBoundaries = new List<IEvaporationBoundary>();

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
    /// Gets the collection of sinks and sources
    /// </summary>
    [DataMember]

    public Collection<IWaterSinkSource> SinkSources { get; protected set; }

    /// <summary>
    /// Gets the collection of downstream connections
    /// </summary>
    [DataMember]
    public Collection<IWaterBody> DownStreamConnections { get; protected set; }

    /// <summary>
    /// Gets the collection og evaporation boundaries
    /// </summary>
    [DataMember]
    public Collection<IEvaporationBoundary> EvaporationBoundaries { get; protected set; }

    #endregion

    #region Constructors


    public AbstractWaterBody()
    {
      Output = new WaterBodyOutput(ID.ToString());
      SinkSources = new Collection<IWaterSinkSource>(_sinkSources);
      DownStreamConnections = new Collection<IWaterBody>(_downStreamConnections);
      EvaporationBoundaries = new Collection<IEvaporationBoundary>(_evapoBoundaries);

    }


    #endregion


    public DateTime EndTime
    {
      get
      {
        DateTime min1 = DateTime.MaxValue;
        if (SinkSources.Count!=0)
          min1 = SinkSources.Min(var => var.EndTime);
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
        _downStreamConnections[0].ReceiveWater(Start, End, Water);
      else if (_downStreamConnections.Count > 1)
      {
        double fraction = Water.Volume/_downStreamConnections.Count;
        foreach (IWaterBody IW in _downStreamConnections)
          IW.ReceiveWater(CurrentTime, End, Water.Substract(fraction));
      }

    }

    public override string ToString()
    {
      return Name;
    }
  }
}

