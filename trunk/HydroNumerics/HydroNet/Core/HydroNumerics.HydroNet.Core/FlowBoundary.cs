using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public class FlowBoundary:AbstractBoundary,IWaterSinkSource 
  {
    private double FlowRate;
    
   
    TimeSeries TS = null;

    public FlowBoundary(double FlowRate)
    {
      this.FlowRate = FlowRate;
      WaterSample = new WaterPacket(1);
    }

    public FlowBoundary(TimeSeries ts)
    {
      TS = ts;
      WaterSample = new WaterPacket(1);

    }


    #region IWaterSource Members

    public string ID{get;set;}

    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        FlowRate = TS.GetValue(Start);

      double _routedFlow = FlowRate * TimeStep.TotalSeconds;
      return WaterProvider.GetWater(_routedFlow);
    }

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        FlowRate = TS.GetValue(Start);

      return -FlowRate * TimeStep.TotalSeconds;
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
