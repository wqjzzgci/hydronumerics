using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using SharpMap.Geometries;

namespace HydroNumerics.HydroNet.Core
{
  public abstract class AbstractWaterBody
  {
    //The list of downstream water bodies
    protected List<IWaterBody> DownStreamConnections = new List<IWaterBody>();

    //The sources are put in the three list to make it possible to change the flow direction
    protected List<IWaterSinkSource> Sources = new List<IWaterSinkSource>();
    protected List<IWaterSinkSource> Sinks = new List<IWaterSinkSource>();
    protected List<IWaterSinkSource> SinkSources = new List<IWaterSinkSource>();

    protected List<IEvaporationBoundary> EvapoBoundaries = new List<IEvaporationBoundary>();

    protected IWaterPacket InitialWater;

    public double Volume { get; set; }

    public DateTime CurrentStartTime { get; set; }

    public WaterBodyOutput Output { get; protected set; }

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
    public AbstractWaterBody(double VolumeOfLakeWater):this()
    {
      Volume = VolumeOfLakeWater;
    }

    public AbstractWaterBody(IWaterPacket initialWater)
      : this(initialWater.Volume)
    {
      this.InitialWater = initialWater.DeepClone();
    }

    public AbstractWaterBody()
    {
      Volume = 0;
      Output = new WaterBodyOutput(ID.ToString());
    }



    #endregion

    #region IWaterbody Members

    /// <summary>
    /// Gets and sets the Water level
    /// </summary>
    public double WaterLevel{get; set;}
   
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

