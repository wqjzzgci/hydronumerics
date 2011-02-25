using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace HydroNumerics.MikeSheTools.DFS
{
  public interface IXYDataSet
  {
    Matrix Data { get; }
    double GetData(double UTMX, double UTMY);
  }
}
