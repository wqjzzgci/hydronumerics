using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public interface ISink:IBoundary
  {
    double GetSinkVolume(DateTime Start, TimeSpan TimeStep);
    void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water);

  }
}
