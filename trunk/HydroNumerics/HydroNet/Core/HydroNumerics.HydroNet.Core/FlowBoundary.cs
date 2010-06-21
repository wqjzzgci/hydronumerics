using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class FlowBoundary:AbstractBoundary,IWaterSinkSource 
  {
    [DataMember]
    BaseTimeSeries TS = null;


    public FlowBoundary()
    {
      WaterSample = new WaterPacket(1);
      Area = 1;
    }


    public FlowBoundary(double FlowRate):this()
    {
      _flow.ExchangeValue = FlowRate;
      _flow.IsInput = true;
    }

    public FlowBoundary(BaseTimeSeries ts)
      : this()
    {
      TS = ts;
      _flow.IsInput = false;
    }


    #region IWaterSource Members


    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        _flow.ExchangeValue = TS.GetValue(Start, Start.Add(TimeStep));

      double _routedFlow = Area * _flow.ExchangeValue * TimeStep.TotalSeconds;
      return WaterSample.DeepClone(_routedFlow);
    }

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        _flow.ExchangeValue = TS.GetValue(Start, Start.Add(TimeStep));

      return -Area * _flow.ExchangeValue * TimeStep.TotalSeconds;
    }

    public bool Source(DateTime time)
    {
      if (TS != null)
        _flow.ExchangeValue = TS.GetValue(time);
      return _flow.ExchangeValue >= 0;
    }

    #endregion
  }
}
