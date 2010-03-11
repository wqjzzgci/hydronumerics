using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class WaterQuantity
  {

    public WaterQuantity(double Mass)
    {
      this.Mass = Mass;
    }

    private double _mass;

    /// <summary>
    /// Gets or sets the Mass
    /// </summary>
    public double Mass
    {
      get { return _mass; }
      set 
      { 
        _mass = Math.Max(0,value);
      _volume = _mass / _density;
      }
    }
    private double _volume;
    private double _density =  1000000;//g/m3;

    /// <summary>
    /// Gets or sets the volume (m3)
    /// </summary>
    public double Volume
    {
      get { return _volume; }
      set 
      { 
        _volume = Math.Max(0,value);
        _mass = _volume * _density;
      }
    }

  }
}
