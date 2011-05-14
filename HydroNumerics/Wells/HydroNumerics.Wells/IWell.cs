using System;
using System.Collections.Generic;

using HydroNumerics.Geometry;

namespace HydroNumerics.Wells
{
  public interface IWell:IXYPoint
  {
    string Description { get; set; }
    string ID { get; set; }
    double Terrain { get; set; }
    string ToString();
    double? Depth { get; set; }
    bool UsedForExtraction { get; set; }
    IEnumerable<IIntake> Intakes { get; }
    IIntake AddNewIntake(int IDNumber);
  }
}
