using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;

namespace HydroNumerics.Nitrate.Model
{
  public class Particle:IXYPoint
  {
    public int StartXGrid { get; set; }
    public int StartYGrid { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }
    public int ID { get; set; }
    public double TravelTime { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double XStart { get; set; }
    public double YStart { get; set; }

  }
}
