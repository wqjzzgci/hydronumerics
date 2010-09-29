using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  public interface IBoundary
  {
    int ID { get; set; }
    string Name { get; set; }
    void ResetOutputTo(DateTime Time);
    void Initialize();
    List<GeoExchangeItem> ExchangeItems { get; }
    DateTime EndTime { get; }
    DateTime StartTime { get; }
    IGeometry ContactGeometry { get; set; }
  }
}
