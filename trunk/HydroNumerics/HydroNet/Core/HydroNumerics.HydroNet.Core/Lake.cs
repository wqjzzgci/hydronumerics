using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  /// <summary>
  /// This class can be used to represent a lake. In a lake all incoming water is mixed before the surplus is routed to downstream IWaterbodies. 
  /// If no downstream waterbodies are connected the water just dissappears.
  /// </summary>
  [DataContract]
  public class Lake:AbstractWaterBody,IWaterBody
  {
    #region persisted data
    /// <summary>
    /// Gets the stored water in the current timestep
    /// This property is only to be used for storage. Do not alter the water.
    /// </summary>
    [DataMember]
    public IWaterPacket CurrentStoredWater {get; set;}

    [DataMember]
    public XYPolygon SurfaceArea { get; set; }

    //Dictionary to store the states
    [DataMember]
    private Dictionary<string, Tuple<DateTime, IWaterPacket>> _states = new Dictionary<string, Tuple<DateTime, IWaterPacket>>();

    [DataMember]
    public TimestampSeries StoredVolume { get; protected set; }

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
    /// Gets the area of the lake. 
    /// </summary>
    public double Area
    {
      get
      {
        return SurfaceArea.GetArea();
      }
    }

    /// <summary>
    /// Gets the volume of the lake
    /// </summary>
    public double Volume
    {
      get
      {
        return Area * Depth;
      }
    }

    public TimeSpan StorageTime { get; private set; }


    #region Constructors


    /// <summary>
    /// Use this constructor to create an empty lake
    /// </summary>
    /// <param name="VolumeOfLakeWater"></param>
    public Lake(double VolumeOfLakeWater):base()
    {
      SurfaceArea = XYPolygon.GetSquare(VolumeOfLakeWater);
      Depth = 1;

      //Only used in unit test. Otherwise it should be set through the state
      CurrentStoredWater = new WaterPacket(0);
      Initialize();
    }

    public Lake(XYPolygon surface)
      : base()
    {
      SurfaceArea = surface;
      Depth = 1;
      Initialize();
    }

#endregion

    private void Initialize()
    {
      StoredVolume = new TimestampSeries();
      StoredVolume.Name = ID + ": Volume";
      StoredVolume.Unit = new HydroNumerics.Core.Unit("m3", 1, 0);
      Output.Items.Add(StoredVolume);
    }

    /// <summary>
    /// Sets the state. Also stores the state. If the state name already exists it will be overwritten
    /// </summary>
    /// <param name="StateName"></param>
    /// <param name="Time"></param>
    /// <param name="WaterInStream"></param>
    public void SetState(string StateName, DateTime Time, IWaterPacket WaterInStream)
    {
      var state = new Tuple<DateTime,IWaterPacket>(Time,WaterInStream.DeepClone());

      if (_states.ContainsKey(StateName))
        _states[StateName] = state;
      else
       _states.Add(StateName, state);
  
      RestoreState(StateName);
    }

    /// <summary>
    /// Saves the current state for future use
    /// </summary>
    /// <param name="StateName"></param>
    public void KeepCurrentState(string StateName)
    {
      _states.Add(StateName, new Tuple<DateTime, IWaterPacket>(CurrentTime, CurrentStoredWater.DeepClone()));
    }


    /// <summary>
    /// Restores to the state stored under the StateName
    /// </summary>
    /// <param name="StateName"></param>
    public void RestoreState(string StateName)
    {
      CurrentTime = _states[StateName].First;
      CurrentStoredWater = _states[StateName].Second.DeepClone();
      Output.ResetToTime(CurrentTime);
    }


    /// <summary>
    /// This is the timestepping method
    /// </summary>
    /// <param name="TimeStep"></param>
    public void MoveInTime(TimeSpan TimeStep)
    {
      DateTime EndTime = CurrentTime.Add(TimeStep);
      double vol = CurrentStoredWater.Volume;
      //loop the sources
      foreach (IWaterSinkSource IWS in SinkSources.Where(var => var.Source(CurrentTime)))
      {
        if (CurrentStoredWater == null)
        {
          CurrentStoredWater = IWS.GetSourceWater(CurrentTime, TimeStep);
        }
        else
        {
          IWaterPacket W = IWS.GetSourceWater(CurrentTime, TimeStep);
          CurrentStoredWater.Add(W);
        }
      }

      Output.Sources.AddSiValue(CurrentTime, EndTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

      //Loop the Evaporation boundaries
      foreach (IEvaporationBoundary IEB in _evapoBoundaries)
        CurrentStoredWater.Evaporate(IEB.GetEvaporationVolume(CurrentTime, TimeStep));

      Output.Evaporation.AddSiValue(CurrentTime, EndTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

      //loop the sinks
      if (CurrentStoredWater != null && CurrentStoredWater.Volume > 0)
      {
        foreach (IWaterSinkSource IWS in SinkSources.Where(var => !var.Source(CurrentTime)))
        {
          double sinkvolume = IWS.GetSinkVolume(CurrentTime, TimeStep);
          IWS.ReceiveSinkWater(CurrentTime,TimeStep, CurrentStoredWater.Substract(sinkvolume));
        }
      }
      Output.Sinks.AddSiValue(CurrentTime, EndTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds);

      IWaterPacket WaterToRoute;

      //Now substract the water that is to be routed
      if (CurrentStoredWater.Volume > Volume) //Only go here if there is a surplus of water.
      {
        WaterToRoute = CurrentStoredWater.Substract(CurrentStoredWater.Volume - Volume);
        //Write routed water. The value is the average value for the timestep
        Output.Outflow.AddSiValue(CurrentTime, EndTime, WaterToRoute.Volume / TimeStep.TotalSeconds);

        SendWaterDownstream(WaterToRoute, CurrentTime, EndTime);
      }
      else
        Output.Outflow.AddSiValue(CurrentTime, EndTime, 0);

      //Write current volume to output. The calculated volume is at the end of the timestep
      StoredVolume.AddSiValue(EndTime, CurrentStoredWater.Volume);

      if (CurrentStoredWater.GetType().Equals(typeof(IsotopeWater)))
        foreach (KeyValuePair<Chemical, TimespanSeries> ct in Output.ChemicalsToLog)
        {
          ct.Value.AddSiValue(CurrentTime, EndTime, ((WaterWithChemicals)CurrentStoredWater).GetConcentration(ct.Key));
        }

      CurrentTime = EndTime;
    }

    /// <summary>
    /// Gets the average storage time for the time period. Calculated as mean volume divide by mean (sinks + outflow + evaporation)
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public TimeSpan GetStorageTime(DateTime Start, DateTime End)
    {
      if (Output.Sinks.EndTime < EndTime || Output.Sinks.StartTime > Start)
        throw new Exception("Cannot calculate storage time outside of the simulated period");

      double d = Output.Sinks.GetSiValue(Start, End) + Output.Outflow.GetSiValue(Start, End) + Output.Evaporation.GetSiValue(Start, End);
      return TimeSpan.FromSeconds(StoredVolume.GetSiValue(Start, End) / d);

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
