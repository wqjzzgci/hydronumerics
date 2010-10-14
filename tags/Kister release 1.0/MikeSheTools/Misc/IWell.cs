using System;
using System.Collections.Generic;

namespace HydroNumerics.MikeSheTools.Misc
{
  public interface IWell
  {
    string Description { get; set; }
    string ID { get; set; }
    double Terrain { get; set; }
    string ToString();
    double X { get; set; }
    double Y { get; set; }
    bool UsedForExtraction { get; set; }
    IEnumerable<IIntake> Intakes { get; }
    IIntake AddNewIntake(int IDNumber);
  }
}
