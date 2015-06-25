using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;

namespace HydroNumerics.Nitrate.Model
{
  public class Particle:IXYPoint
  {
    public int ID { get; set; }
    public double TravelTime { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double XStart { get; set; }
    public double YStart { get; set; }
    public double ZStart { get; set; }
    public int Registration { get; set; }
    public SinkType SinkType { get; set; }

    public double HorizontalTravelDistance
    {
      get
      {
        return new XYPoint(this.XStart, this.YStart).GetDistance(this);
      }
    }

    public override bool Equals(object obj)
    {
      return ID.Equals(((Particle)obj).ID);
    }

    public override int GetHashCode()
    {
      return ID.GetHashCode();
    }

  }
}
