using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HydroNumerics.MikeSheTools.DFS
{
  public interface IXYTDataSet
  {
    IList<DateTime> TimeSteps { get; }
    DenseMatrix TimeData(int TimeStep);
    DenseMatrix TimeData(DateTime TimeStep);
  }
}
