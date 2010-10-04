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
    public IWaterPacket WaterSample { get; set; }


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
        _exchangeItems = new List<GeoExchangeItem>();
        GeoExchangeItem flowExchangeItem = new GeoExchangeItem();
        flowExchangeItem.Description = "Flow (Boundary name: " + Name + ")";
        flowExchangeItem.Geometry = null;
        flowExchangeItem.ExchangeValue = Flow;
        if (TS != null)
        {
            flowExchangeItem.IsInput = false;
        }
        else
        {
            flowExchangeItem.IsInput = true;
        }
        flowExchangeItem.IsOutput = true;
        flowExchangeItem.Location = Name;
        flowExchangeItem.Quantity = "Flow";
        flowExchangeItem.timeType = TimeType.TimeSpan;
        flowExchangeItem.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
        this.ExchangeItems.Add(flowExchangeItem);
    }

    public SinkSourceBoundary()
    {
      WaterSample = new WaterPacket(1);
      ContactGeometry = XYPolygon.GetSquare(1);
    }


    public SinkSourceBoundary(double FlowRate):this()
    {
        Flow = FlowRate;
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
      if (TS != null)
        Flow = TS.GetSiValue(Start, Start.Add(TimeStep));

      double _routedFlow = ((XYPolygon)ContactGeometry).GetArea() * Flow * TimeStep.TotalSeconds;
      return WaterSample.DeepClone(_routedFlow);
    }

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        Flow = TS.GetSiValue(Start, Start.Add(TimeStep));

      return -((XYPolygon)ContactGeometry).GetArea() * Flow * TimeStep.TotalSeconds;
    }

    public bool Source(DateTime time)
    {
      if (TS != null)
        Flow = TS.GetSiValue(time);
      return Flow >= 0;
    }

    #endregion
  }
}
