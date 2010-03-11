using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class TestEvaporation:IEvaporationBoundary 
  {
    private double _volume;
    public TestEvaporation(double Volume)
    {
      _volume = Volume;
    }

    #region IEvaporationBoundary Members

    public string ID {get;set;}

    public double GetEvaporationVolume(DateTime Start, TimeSpan TimeStep)
    {
      return _volume;
    }

    #endregion
  }
}
