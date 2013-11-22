using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public class Particle
  {
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }
    public int ID { get; set; }
    public double TravelTime { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

  }
}
