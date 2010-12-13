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
  public class SourceBoundary:AbstractBoundary,ISource 
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

    public SourceBoundary()
    {
      WaterSample = new WaterPacket(1);
      ContactGeometry = XYPolygon.GetSquare(1);
    }

    public SourceBoundary(double FlowRate):this()
    {
      _currentFlowRate = FlowRate;
    }

    public SourceBoundary(BaseTimeSeries ts) : this()
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

    public TimestampSeries TimeValues2
    {
      get
      {
        return (TimestampSeries)TS;
      }
    }

    #region IWaterSource Members

    public void AddChemicalConcentrationSeries(Chemical C, TimestampSeries Concentration)
    {
      _concentrations.Add(C, Concentration);
    }

    private Dictionary<Chemical, TimestampSeries> _concentrations = new Dictionary<Chemical, TimestampSeries>();


    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      foreach (var kvp in _concentrations)
        ((WaterPacket)WaterSample).SetConcentration(kvp.Key,kvp.Value.GetSiValue(Start, Start.Add(TimeStep)));

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

    #endregion
  }
}
