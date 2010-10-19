using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class SinkSourceBoundary:AbstractBoundary,ISource, ISink 
  {
    [DataMember]
    private BaseTimeSeries TS = null;

    [DataMember]
    private double _currentFlowRate;

    /// <summary>
    /// Use this to override a fixed flowrate or a time series
    /// </summary>
    public double? OverrideFlowRate { get; set; }

    /// <summary>
    /// Gets and sets the type of water this boundary will deliver
    /// </summary>
    [DataMember]
    public IWaterPacket WaterSample { get; set; }

    /// <summary>
    /// Gets the latest time this boundary can provide data
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        if (TS == null)
          return DateTime.MaxValue;
        else
          return TS.EndTime;
      }
    }

    /// <summary>
    /// Gets the earliest time this boundary can provide data
    /// </summary>
    public DateTime StartTime
    {
      get
      {
        if (TS == null)
          return DateTime.MinValue;
        else
          return TS.StartTime;
      }
    }


    public void Initialize()
    {
    }

    public SinkSourceBoundary()
    {
      WaterSample = new WaterPacket(1);
      ContactGeometry = XYPolygon.GetSquare(1);
    }

    public SinkSourceBoundary(double FlowRate):this()
    {
      _currentFlowRate = FlowRate;
    }

    public SinkSourceBoundary(BaseTimeSeries ts) : this()
    {
      TS = ts;
    }

    public TimespanSeries TimeValues
    {
      get
      {
        return (TimespanSeries)TS;
      }
    }
    
    #region IWaterSource Members


    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      if (OverrideFlowRate.HasValue)
        _currentFlowRate = OverrideFlowRate.Value;
      else if (TS != null)
        _currentFlowRate = TS.GetSiValue(Start, Start.Add(TimeStep));

      double _routedFlow =  _currentFlowRate * TimeStep.TotalSeconds;

      //Could be a point
      XYPolygon geom = ContactGeometry as XYPolygon;
      if (geom != null)
        _routedFlow *= geom.GetArea();

      return WaterSample.DeepClone(_routedFlow);
    }

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      if (OverrideFlowRate.HasValue)
        _currentFlowRate = OverrideFlowRate.Value;
      else if (TS != null)
        _currentFlowRate = TS.GetSiValue(Start, Start.Add(TimeStep));

      return -((XYPolygon)ContactGeometry).GetArea() * _currentFlowRate * TimeStep.TotalSeconds;
    }

    public void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {}


    public bool Source(DateTime time)
    {
      if (TS != null)
        _currentFlowRate = TS.GetSiValue(time);
      return _currentFlowRate >= 0;
    }

    #endregion
  }
}
