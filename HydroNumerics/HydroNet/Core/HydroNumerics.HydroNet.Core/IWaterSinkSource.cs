using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.HydroNet.Core
{
  public interface IWaterSinkSource
  {
    string Name { get; set; }
    void Initialize();
    IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep);
    double GetSinkVolume(DateTime Start, TimeSpan TimeStep);
    void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water);
    bool Source(DateTime Start);
    List<GeoExchangeItem> ExchangeItems { get; }
    DateTime EndTime { get; }
 
  }
}
