using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using SharpMap.Geometries;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public abstract class AbstractWaterBody
  {
    [DataMember]
    protected List<IWaterBody> _downStreamConnections = new List<IWaterBody>();

    [DataMember]
    protected List<IWaterSinkSource> _sinkSources = new List<IWaterSinkSource>();

    [DataMember]
    protected List<IEvaporationBoundary> _evapoBoundaries = new List<IEvaporationBoundary>();

    [DataMember]
    public int ID { get; set; }

    [DataMember]
    public double Volume { get; protected set; }

    [DataMember]
    public WaterBodyOutput Output { get; protected set; }

    /// <summary>
    /// Gets and sets the Water level
    /// </summary>
    [DataMember]
    public double WaterLevel { get; set; }

    
    public DateTime CurrentStartTime { get; protected set; }

    /// <summary>
    /// Gets the collection of sinks and sources
    /// </summary>
    public Collection<IWaterSinkSource> SinkSources { get; protected set; }

    /// <summary>
    /// Gets the collection of downstream connections
    /// </summary>
    public Collection<IWaterBody> DownStreamConnections { get; protected set; }

    /// <summary>
    /// Gets the collection og evaporation boundaries
    /// </summary>
    public Collection<IEvaporationBoundary> EvaporationBoundaries { get; protected set; }

    #region Constructors


    public AbstractWaterBody()
    {
      Volume = 0;
      Output = new WaterBodyOutput(ID.ToString());
      SinkSources = new Collection<IWaterSinkSource>(_sinkSources);
      DownStreamConnections = new Collection<IWaterBody>(_downStreamConnections);
      EvaporationBoundaries = new Collection<IEvaporationBoundary>(_evapoBoundaries);
      
    }

    /// <summary>
    /// Use this constructor to create an empty lake
    /// </summary>
    /// <param name="VolumeOfLakeWater"></param>
    public AbstractWaterBody(double VolumeOfLakeWater):this()
    {
      Volume = VolumeOfLakeWater;
    }


    #endregion

   


  

  }
}

