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
    private double FlowRate;
    [DataMember]
    BaseTimeSeries TS = null;


    public FlowBoundary()
    {
      WaterSample = new WaterPacket(1);
      Area = 1;
    }


    public FlowBoundary(double FlowRate):this()
    {
      this.FlowRate = FlowRate;
    }

    public FlowBoundary(BaseTimeSeries ts)
      : this()
    {
      TS = ts;
    }


    #region IWaterSource Members


    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        FlowRate = TS.GetValue(Start,Start.Add(TimeStep ));

      double _routedFlow = Area * FlowRate * TimeStep.TotalSeconds;
      return WaterSample.DeepClone(_routedFlow);
    }

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        FlowRate = TS.GetValue(Start, Start.Add(TimeStep));

      return - Area * FlowRate * TimeStep.TotalSeconds;
    }

    public bool Source(DateTime time)
    {
      if (TS != null)
        FlowRate = TS.GetValue(time);
      return FlowRate >= 0;
    }

    #endregion
  }
}
