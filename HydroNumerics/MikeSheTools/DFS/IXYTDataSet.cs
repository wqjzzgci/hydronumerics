using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace HydroNumerics.MikeSheTools.DFS
{
  public interface IXYTDataSet
  {
    List<DateTime> TimeSteps { get; }
    Matrix TimeData(int TimeStep);
    Matrix TimeData(DateTime TimeStep);
  }
}
