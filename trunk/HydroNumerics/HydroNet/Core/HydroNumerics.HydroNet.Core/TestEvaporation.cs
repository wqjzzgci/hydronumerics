using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public class TestEvaporation:FlowBoundary, IEvaporationBoundary 
  {
    
    public TestEvaporation(double EvaporationRate):base(EvaporationRate)
    {
    }

    public TestEvaporation(TimeSeries EvaporationRate)
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
