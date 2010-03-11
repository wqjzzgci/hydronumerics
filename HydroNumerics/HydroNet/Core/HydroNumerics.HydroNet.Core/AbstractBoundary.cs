using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public abstract class AbstractBoundary
  {
    /// <summary>
    /// Gets and sets the Water that flows out of this boundary. Volume does not matter.
    /// </summary>
    public IWaterPacket WaterSample
    {
      get
      {
        return WaterProvider.Sample;
      }
      set
      {
        WaterProvider.Sample = value;
      }
    }


    protected InfiniteSource WaterProvider = new InfiniteSource();

  }
}
