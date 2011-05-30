using System;
using System.Collections.Generic;
using HydroNumerics.Time.Core;

namespace HydroNumerics.Wells
{
  public interface IIntake:IComparable<IIntake>
  {
    int IDNumber { get; set; }
    int? Layer { get; set;}
    TimestampSeries HeadObservations { get; }
    TimespanSeries Extractions { get; }
    List<Screen> Screens { get; }
    string ToString();
    IWell well { get; }
    double? Depth { get; set; }
  }
}
