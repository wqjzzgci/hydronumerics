using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class EvaporationRateBoundary:FlowBoundary, IEvaporationBoundary 
  {
    
    public EvaporationRateBoundary(double EvaporationRate):base(EvaporationRate)
    {
    }

    public EvaporationRateBoundary(TimeSeries EvaporationRate)
      : base(EvaporationRate)
    {
    }


    #region IEvaporationBoundary Members

    public double GetEvaporationVolume(DateTime Start, TimeSpan TimeStep)
    {
      return -base.GetSinkVolume(Start, TimeStep);
    }

    #endregion
  }
}
