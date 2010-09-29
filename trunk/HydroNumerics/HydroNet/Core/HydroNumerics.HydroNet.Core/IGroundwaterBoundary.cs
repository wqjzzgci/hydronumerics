using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Geometry;


namespace HydroNumerics.HydroNet.Core
{
  public interface IGroundwaterBoundary:ISink, ISource 
  {
    bool IsSource(DateTime time);
  }
}
