using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public interface IEvaporationBoundary
  {
    string Name { get; set; }
    double GetEvaporationVolume(DateTime Start, TimeSpan TimeStep);
    DateTime EndTime { get; }
  }
}
