using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HydroNumerics.MikeSheTools.DFS
{
  public interface IXYZTDataSet
  {
    DateTime[] TimeSteps { get; }
    IMatrix3d TimeData(int TimeStep);
    IMatrix3d TimeData(DateTime TimeStep);
    int GetTimeStep(DateTime TimeStep);
  }
}
