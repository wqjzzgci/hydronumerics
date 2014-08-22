using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.Core.Time;
using HydroNumerics.MikeSheTools.Mike11;

namespace HydroNumerics.MikeSheTools.Core
{
  public class DetailedMike11
  {
    public XYPoint Location { get; set; }
    public TimeStampSeries Observation { get; set; }
    public TimeStampSeries Simulation { get; set; }
    public M11Branch Branch { get; set; }
    public string TopoID { get; set; }
    public double Chainage { get; set; }
    public string Name { get; set; }
  }
}
