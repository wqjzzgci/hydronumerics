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
    /// Use this constructor to create an empty lake. Should only be used for unit testing
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
      ResetToTime(CurrentTime);
    }


    /// <summary>
    /// This is the timestepping method
    /// </summary>
    /// <param name="TimeStep"></param>
    public void Update(DateTime NewTime)
    {
      TimeSpan TimeStep = NewTime.Subtract(CurrentTime);
      double vol = CurrentStoredWater.Volume;

      //loop the sources
      foreach (ISource IWS in Sources)
          CurrentStoredWater.Add(IWS.GetSourceWater(CurrentTime, TimeStep));
      //Update output
      Output.Sources.AddSiValue(CurrentTime, NewTime, (CurrentStoredWater.Volume - vol) / TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

      //Loop the groundwater sources
      foreach(var gwb in GroundwaterBoundaries.Where(var=>var.IsSource(CurrentTime)))
        CurrentStoredWater.Add(gwb.GetSourceWater(CurrentTime, TimeStep));
      //Update output
      Output.GroundwaterInflow.AddSiValue(CurrentTime, NewTime, (CurrentStoredWater.Volume - vol) / TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

      //Loop the precipitation
      foreach (ISource IWS in Precipitation)
        CurrentStoredWater.Add(IWS.GetSourceWater(CurrentTime, TimeStep));
      //Update output
      Output.Precipitation.AddSiValue(CurrentTime, NewTime, (CurrentStoredWater.Volume - vol) / TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

      //Loop the Evaporation boundaries
      foreach (var IEB in _evapoBoundaries)
        CurrentStoredWater.Evaporate(IEB.GetSinkVolume(CurrentTime, TimeStep));
      //Update output
      Output.Evaporation.AddSiValue(CurrentTime, NewTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

      //loop the sinks
      if (CurrentStoredWater.Volume > 0)
      {
        foreach (var IWS in Sinks)
        {
          double sinkvolume = IWS.GetSinkVolume(CurrentTime, TimeStep);
          IWS.ReceiveSinkWater(CurrentTime,TimeStep, CurrentStoredWater.Substract(sinkvolume));
        }
      }
      //Update output
      Output.Sinks.AddSiValue(CurrentTime, NewTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

        //Loop the groundwater sinks
      if (CurrentStoredWater.Volume > 0)
      {
        foreach (var gwb in GroundwaterBoundaries.Where(var => !var.IsSource(CurrentTime)))
        {
          double sinkvolume = gwb.GetSinkVolume(CurrentTime, TimeStep);
          gwb.ReceiveSinkWater(CurrentTime, TimeStep, CurrentStoredWater.Substract(sinkvolume));
        }
      }
      //Update output
      Output.GroundwaterOutflow.AddSiValue(CurrentTime, NewTime, (CurrentStoredWater.Volume - vol)/TimeStep.TotalSeconds);
      vol = CurrentStoredWater.Volume;

      IWaterPacket WaterToRoute;

      //Now substract the water that is to be routed
      if (CurrentStoredWater.Volume > Volume) //Only go here if there is a surplus of water.
      {
        WaterToRoute = CurrentStoredWater.Substract(CurrentStoredWater.Volume - Volume);
        //Write routed water. The value is the average value for the timestep
        Output.Outflow.AddSiValue(CurrentTime, NewTime, WaterToRoute.Volume / TimeStep.TotalSeconds);

        SendWaterDownstream(WaterToRoute, CurrentTime, NewTime);
      }
      else
        Output.Outflow.AddSiValue(CurrentTime, NewTime, 0);

      //Write current volume to output. The calculated volume is at the end of the timestep
      Output.StoredVolume.AddSiValue(NewTime, CurrentStoredWater.Volume);

      if (CurrentStoredWater.GetType().Equals(typeof(IsotopeWater)))
        foreach (KeyValuePair<Chemical, TimespanSeries> ct in Output.ChemicalsToLog)
        {
          ct.Value.AddSiValue(CurrentTime, NewTime, ((WaterWithChemicals)CurrentStoredWater).GetConcentration(ct.Key));
        }

      CurrentTime = NewTime;
    }


    /// <summary>
    /// Adds a water packet to the lake. 
    /// This method is to be used by upstream connections.
    /// 
    /// </summary>
    /// <param name="Start">Start of inflow period</param>
    /// <param name="End">End of inflow period</param>
    /// <param name="Water"></param>
    public void AddWaterPacket(DateTime Start, DateTime End, IWaterPacket Water)
    {
      Water.Tag(ID);
      CurrentStoredWater.Add(Water);

      Output.Inflow.AddSiValue(Start, End, Water.Volume / (End - Start).TotalSeconds);
    }

  }
}
