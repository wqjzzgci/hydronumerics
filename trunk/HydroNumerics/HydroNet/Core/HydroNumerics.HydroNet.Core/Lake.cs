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
  public class Lake:AbstractWaterBody,IWaterBody 
  {
    /// <summary>
    /// Gets the stored water in the current timestep
    /// This property is only to be used for storage. Do not alter the water.
    /// </summary>
    public IWaterPacket CurrentStoredWater {get; set;}


    public Polygon SurfaceArea { get; set; }
    public double Area { get; set; }
    public TimeSeries Volume{get;protected set;}

    #region Constructors


    /// <summary>
    /// Use this constructor to create a WaterBody with a volume. The volume will correspond to the volume of the initialwater
    /// </summary>
    /// <param name="InitialWater"></param>
    public Lake(IWaterPacket initialWater):base(initialWater)
    {
      CurrentStoredWater = initialWater;
      Initialize();
    }

    /// <summary>
    /// Use this constructor to create an empty lake
    /// </summary>
    /// <param name="VolumeOfLakeWater"></param>
    public Lake(double VolumeOfLakeWater):base(VolumeOfLakeWater)
    {
      CurrentStoredWater = new WaterPacket(0);
      Initialize();
    }

    private void Initialize()
    {
      Volume = new TimeSeries();
      Volume.Name = ID + ": Volume";
      Volume.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("m3", 1, 0);
      Volume.TimeSeriesType = TimeSeriesType.TimeStampBased;
      Output.TimeSeriesList.Add(Volume);

    }

    #endregion

    /// <summary>
    /// Gets the geometry
    /// </summary>
    public IGeometry Geometry
    {
      get
      {
        return SurfaceArea;
      }
    }

    /// <summary>
    /// This is the timestepping method
    /// </summary>
    /// <param name="TimeStep"></param>
    public void MoveInTime(TimeSpan TimeStep)
    {

      double vol = CurrentStoredWater.Volume;
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

      Output.Sources.AddTimeValueRecord(new TimeValue(CurrentStartTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds));
      vol = CurrentStoredWater.Volume;

      //Loop the Evaporation boundaries
      foreach (IEvaporationBoundary IEB in EvapoBoundaries)
        CurrentStoredWater.Evaporate(IEB.GetEvaporationVolume(CurrentStartTime, TimeStep));

      Output.Evaporation.AddTimeValueRecord(new TimeValue(CurrentStartTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds));
      vol = CurrentStoredWater.Volume;

      //loop the sinks
      if (CurrentStoredWater != null && CurrentStoredWater.Volume > 0)
      {
        foreach (IWaterSinkSource IWS in Sinks)
        {
          double sinkvolume = IWS.GetSinkVolume(CurrentStartTime, TimeStep);
          IWS.ReceiveSinkWater(CurrentStartTime,TimeStep, CurrentStoredWater.Substract(sinkvolume));
        }
      }
      Output.Sinks.AddTimeValueRecord(new TimeValue(CurrentStartTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds));

      IWaterPacket WaterToRoute;

      DateTime EndTime = CurrentStartTime.Add(TimeStep);

      //Now substract the water that is to be routed
      if (CurrentStoredWater.Volume > _volume) //Only go here if there is a surplus of water.
      {
        WaterToRoute = CurrentStoredWater.Substract(CurrentStoredWater.Volume - _volume);
        //Write routed water. The value is the average value for the timestep
        Output.Outflow.AddTimeValueRecord(new TimeValue(CurrentStartTime, WaterToRoute.Volume / TimeStep.TotalSeconds));

        //Send water to downstream recipients
        if (DownStreamConnections.Count == 1)
          DownStreamConnections[0].ReceiveWater(CurrentStartTime, EndTime, WaterToRoute);
        else if (DownStreamConnections.Count > 1)
        {
          foreach (IWaterBody IW in DownStreamConnections)
            IW.ReceiveWater(CurrentStartTime, EndTime, WaterToRoute.Substract(CurrentStoredWater.Volume / DownStreamConnections.Count));
        }
      }
      else
        Output.Outflow.AddTimeValueRecord(new TimeValue(CurrentStartTime, 0));

      //Write current volume to output. The calculated volume is at the end of the timestep
      Volume.AddTimeValueRecord(new TimeValue(EndTime, CurrentStoredWater.Volume));

      CurrentStartTime =EndTime;

    }

    public void Reset()
    {
      foreach (TimeSeries T in Output.TimeSeriesList)
        T.TimeValues.Clear();

      CurrentStoredWater = InitialWater.DeepClone();
    }

    /// <summary>
    /// Receives water and adds it to the storage. 
    /// This method is to be used by upstream connections.
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Water"></param>
    public void ReceiveWater(DateTime Start, DateTime End, IWaterPacket Water)
    {
      Water.Tag(ID);
      CurrentStoredWater.Add(Water);
    }

  }
}
