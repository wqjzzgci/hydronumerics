using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using SharpMap.Geometries;

namespace HydroNumerics.HydroNet.Core
{
  public abstract class BaseWaterBody:IWaterBody
  {
    //The list of downstream water bodies
    protected List<IWaterBody> DownStreamConnections = new List<IWaterBody>();

    //The sources are put in the three list to make it possible to change the flow direction
    protected List<IWaterSinkSource> Sources = new List<IWaterSinkSource>();
    protected List<IWaterSinkSource> Sinks = new List<IWaterSinkSource>();
    protected List<IWaterSinkSource> SinkSources = new List<IWaterSinkSource>();

    protected List<IEvaporationBoundary> EvapoBoundaries = new List<IEvaporationBoundary>();

    //This is used to give the waterbody a volume so transport can be retarded
    protected double _volume = 0;

    public DateTime CurrentStartTime { get; set; }

    public TimeSeriesGroup Output { get; protected set; }


    public int ID { get; set; }

    public List<IWaterBody> DownStream
    {
      get { return DownStreamConnections; }
    }

    #region Constructors


    /// <summary>
    /// Use this constructor to create an empty lake
    /// </summary>
    /// <param name="VolumeOfLakeWater"></param>
    public BaseWaterBody(double VolumeOfLakeWater)
    {
      _volume = VolumeOfLakeWater;
      Output = new TimeSeriesGroup();
      TimeSeries ts = new TimeSeries();
      ts.ID = ID + ": Outflow";
      ts.TimeSeriesType = TimeSeriesType.TimeStampBased;
      Output.TimeSeriesList.Add(ts);
    }



    #endregion

    #region IWaterbody Members

    /// <summary>
    /// Gets and sets the Geometry
    /// </summary>
    public IGeometry Geometry { get; set; }

    /// <summary>
    /// Gets and sets the Water level
    /// </summary>
    public double WaterLevel{get; set;}


    public virtual IWaterPacket CurrentStoredWater { get; set; }
    
    /// <summary>
    /// This is the timestepping method
    /// </summary>
    /// <param name="TimeStep"></param>
    public virtual void MoveInTime(TimeSpan TimeStep)
    {
    }

    /// <summary>
    /// Receives water and adds it to the storage. 
    /// This method is to be used by upstream connections.
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Water"></param>
    public virtual void ReceiveWater(DateTime Start, DateTime End, IWaterPacket Water)
    {
    }


    /// <summary>
    /// Adds a connection
    /// </summary>
    /// <param name="Element"></param>
    /// <param name="Upstream"></param>
    public void AddDownstreamConnection(IWaterBody Element)
    {
        DownStreamConnections.Add(Element);
    }

    /// <summary>
    /// Adds an evaporation boundary
    /// </summary>
    /// <param name="Evapo"></param>
    public void AddEvaporationBoundary(IEvaporationBoundary Evapo)
    {
      EvapoBoundaries.Add(Evapo);
    }

    /// <summary>
    /// Adds a source or a sink
    /// </summary>
    /// <param name="Source"></param>
    public void AddWaterSinkSource(IWaterSinkSource Source)
    {
      //Add to the list of sources
      SinkSources.Add(Source);
      //Add to either the list of sinks or the list of sources
      if (Source.Source(CurrentStartTime))
        Sources.Add(Source);
      else
        Sinks.Add(Source);
    }
    #endregion

    #region Private methods

    /// <summary>
    /// Distributes the sources and sinks depending on flow direction
    /// </summary>
    protected void CheckSourceDirection()
    {
      Sources.Clear();
      Sinks.Clear();
      foreach (IWaterSinkSource IWS in SinkSources)
      {
        if (IWS.Source(CurrentStartTime))
          Sources.Add(IWS);
        else
          Sinks.Add(IWS);
      }
    }

    #endregion

  }
}

