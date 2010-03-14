using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HydroNumerics.MikeSheTools.DFS
{
  public interface IXYZDataSet
  {
    IMatrix3d Data { get; }
  }
}
