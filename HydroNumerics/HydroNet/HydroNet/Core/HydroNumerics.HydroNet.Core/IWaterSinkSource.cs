using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public interface IWaterSinkSource
  {
    string ID { get; set; }
    IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep);
    double GetSinkVolume(DateTime Start, TimeSpan TimeStep);
    bool Source(DateTime Start); 
  }
}
