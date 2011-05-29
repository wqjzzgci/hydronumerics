using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HydroNumerics.MikeSheTools.DFS
{
  public interface IXYDataSet
  {
    DenseMatrix Data { get; }
    double GetData(double UTMX, double UTMY);
  }
}
