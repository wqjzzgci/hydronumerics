using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMap.Geometries;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  /// <summary>
  /// This class can be used to represent a lake. In a lake all incoming water is mixed before the surplus is routed to downstream IWaterbodies. 
  /// If no downstream waterbodies are connected the water just dissappears.
  /// </summary>
  public class Lake:IWaterBody 
  {
    //The list of downstream water bodies
    private List<IWaterBody> DownStreamConnections = new List<IWaterBody>();

    //The sources are put in the three list to make it possible to change the flow direction
    private List<IWaterSinkSource> Sources = new List<IWaterSinkSource>();
    private List<IWaterSinkSource> Sinks = new List<IWaterSinkSource>();
    private List<IWaterSinkSource> SinkSources = new List<IWaterSinkSource>();

    private List<IEvaporationBoundary> EvapoBoundaries = new List<IEvaporationBoundary>();

    //This is used to give the waterbody a volume so transport can be retarded
    private double _volume = 0;

    /// <summary>
    /// Gets the stored water in the current timestep
    /// This property is only to be used for storage. Do not alter the water.
    /// </summary>
    public IWaterPacket CurrentStoredWater {get;set;}

    public DateTime CurrentStartTime { get; set; }

    public TimeSeriesGroup Output { get; protected set; }


    public int ID { get; set; }

    public List<IWaterBody> DownStream
    {
      get { return DownStreamConnections; }
    }

    #region Constructors


    /// <summary>
    /// Use this constructor to create a WaterBody with a volume. The volume will correspond to the volume of the initialwater
    /// </summary>
    /// <param name="InitialWater"></param>
    public Lake(IWaterPacket InitialWater)
    {
      _volume = InitialWater.Volume;
      CurrentStoredWater = InitialWater;
    }

    /// <summary>
    /// Use this constructor to create an empty lake
    /// </summary>
    /// <param name="VolumeOfLakeWater"></param>
    public Lake(double VolumeOfLakeWater)
    {
      _volume = VolumeOfLakeWater;
      CurrentStoredWater = new WaterPacket(0);
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



    /// <summary>
    /// This is the timestepping method
    /// </summary>
    /// <param name="TimeStep"></param>
    public void MoveInTime(TimeSpan TimeStep)
    {
      CurrentStartTime += TimeStep;

      //loop the sources
      foreach (IWaterSinkSource IWS in Sources)
      {
        if (CurrentStoredWater == null)
        {
          CurrentStoredWater = IWS.GetSourceWater(CurrentStartTime, TimeStep);
        }
        else
        {
          IWaterPacket W = IWS.GetSourceWater(CurrentStartTime, TimeStep);
          CurrentStoredWater.Add(W);
        }
      }

      //Loop the Evaporation boundaries
      foreach (IEvaporationBoundary IEB in EvapoBoundaries)
        CurrentStoredWater.Evaporate(IEB.GetEvaporationVolume(CurrentStartTime, TimeStep));

      //loop the sinks
      if (CurrentStoredWater != null && CurrentStoredWater.Volume > 0)
      {
        foreach (IWaterSinkSource IWS in Sinks)
        {
          double sinkvolume = IWS.GetSinkVolume(CurrentStartTime, TimeStep);
          CurrentStoredWater.Substract(sinkvolume);
        }
      }

      IWaterPacket WaterToRoute;

      //Now substract the water that is to be routed
      if (CurrentStoredWater.Volume > _volume) //Only go here if there is a surplus of water.
      {
        WaterToRoute = CurrentStoredWater.Substract(CurrentStoredWater.Volume - _volume);

        //Send water to downstream recipients
        if (DownStreamConnections.Count == 1)
          DownStreamConnections[0].ReceiveWater(TimeStep, WaterToRoute);
        else if (DownStreamConnections.Count > 1)
        {
          foreach (IWaterBody IW in DownStreamConnections)
            IW.ReceiveWater(TimeStep, WaterToRoute.Substract(CurrentStoredWater.Volume / DownStreamConnections.Count));
        }
      }
    }

    /// <summary>
    /// Receives water and adds it to the storage. 
    /// This method is to be used by upstream connections.
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Water"></param>
    public void ReceiveWater(TimeSpan TimeStep, IWaterPacket Water)
    {
        CurrentStoredWater.Add(Water);
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
    private void CheckSourceDirection()
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
