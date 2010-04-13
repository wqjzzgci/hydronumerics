using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using SharpMap.Geometries;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  /// <summary>
  /// This class can be used to represent a lake. In a lake all incoming water is mixed before the surplus is routed to downstream IWaterbodies. 
  /// If no downstream waterbodies are connected the water just dissappears.
  /// </summary>
  [DataContract]
  public class Lake:AbstractWaterBody,IWaterBody 
  {
    /// <summary>
    /// Gets the stored water in the current timestep
    /// This property is only to be used for storage. Do not alter the water.
    /// </summary>
    public IWaterPacket CurrentStoredWater {get; set;}

    public Polygon SurfaceArea { get; set; }

    [DataMember]
    public double Area { get; set; }

    private Dictionary<string, Tuple<DateTime, IWaterPacket>> _states = new Dictionary<string, Tuple<DateTime, IWaterPacket>>();

    public TimeSeries StoredVolume{get;protected set;}


    #region Constructors


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
      StoredVolume = new TimeSeries();
      StoredVolume.Name = ID + ": Volume";
      StoredVolume.Unit = new HydroNumerics.Core.Unit("m3", 1, 0);
      StoredVolume.TimeSeriesType = TimeSeriesType.TimeStampBased;
      Output.TimeSeriesList.Add(StoredVolume);
    }

    /// <summary>
    /// Sets the state. Also stores the state
    /// </summary>
    /// <param name="StateName"></param>
    /// <param name="Time"></param>
    /// <param name="WaterInStream"></param>
    public void SetState(string StateName, DateTime Time, IWaterPacket WaterInStream)
    {
     _states.Add(StateName, new Tuple<DateTime,IWaterPacket>(Time,WaterInStream.DeepClone()));
      RestoreState(StateName);
    }

    /// <summary>
    /// Saves the current state for future use
    /// </summary>
    /// <param name="StateName"></param>
    public void KeepCurrentState(string StateName)
    {
      _states.Add(StateName, new Tuple<DateTime, IWaterPacket>(CurrentStartTime, CurrentStoredWater.DeepClone()));
    }

    /// <summary>
    /// Restores to the state stored under the StateName
    /// </summary>
    /// <param name="StateName"></param>
    public void RestoreState(string StateName)
    {
      CurrentStartTime = _states[StateName].First;
      CurrentStoredWater = _states[StateName].Second.DeepClone();
      Output.ResetToTime(CurrentStartTime);
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
      foreach (IWaterSinkSource IWS in SinkSources.Where(var => var.Source(CurrentStartTime)))
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
      foreach (IEvaporationBoundary IEB in _evapoBoundaries)
        CurrentStoredWater.Evaporate(IEB.GetEvaporationVolume(CurrentStartTime, TimeStep));

      Output.Evaporation.AddTimeValueRecord(new TimeValue(CurrentStartTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds));
      vol = CurrentStoredWater.Volume;

      //loop the sinks
      if (CurrentStoredWater != null && CurrentStoredWater.Volume > 0)
      {
        foreach (IWaterSinkSource IWS in SinkSources.Where(var => !var.Source(CurrentStartTime)))
        {
          double sinkvolume = IWS.GetSinkVolume(CurrentStartTime, TimeStep);
          IWS.ReceiveSinkWater(CurrentStartTime,TimeStep, CurrentStoredWater.Substract(sinkvolume));
        }
      }
      Output.Sinks.AddTimeValueRecord(new TimeValue(CurrentStartTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds));

      IWaterPacket WaterToRoute;

      DateTime EndTime = CurrentStartTime.Add(TimeStep);

      //Now substract the water that is to be routed
      if (CurrentStoredWater.Volume > Volume) //Only go here if there is a surplus of water.
      {
        WaterToRoute = CurrentStoredWater.Substract(CurrentStoredWater.Volume - Volume);
        //Write routed water. The value is the average value for the timestep
        Output.Outflow.AddTimeValueRecord(new TimeValue(CurrentStartTime, WaterToRoute.Volume / TimeStep.TotalSeconds));

        //Send water to downstream recipients
        if (_downStreamConnections.Count == 1)
          _downStreamConnections[0].ReceiveWater(CurrentStartTime, EndTime, WaterToRoute);
        else if (_downStreamConnections.Count > 1)
        {
          foreach (IWaterBody IW in _downStreamConnections)
            IW.ReceiveWater(CurrentStartTime, EndTime, WaterToRoute.Substract(CurrentStoredWater.Volume / _downStreamConnections.Count));
        }
      }
      else
        Output.Outflow.AddTimeValueRecord(new TimeValue(CurrentStartTime, 0));

      //Write current volume to output. The calculated volume is at the end of the timestep
      StoredVolume.AddTimeValueRecord(new TimeValue(EndTime, CurrentStoredWater.Volume));

      CurrentStartTime =EndTime;

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
