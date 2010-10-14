using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public interface ISource:IBoundary 
  {
    IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep);
    IWaterPacket WaterSample { get; set; }
  }
}
