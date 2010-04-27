using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public interface IHasTSOuput
  {
    WaterBodyOutput Output { get; }
  }
}
