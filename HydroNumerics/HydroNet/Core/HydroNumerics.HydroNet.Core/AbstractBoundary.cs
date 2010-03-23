using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpMap.Geometries;

namespace HydroNumerics.HydroNet.Core
{
  public abstract class AbstractBoundary
  {
    private Polygon _contactArea;
    private double _area;

    public string ID { get; set; }


    public double Area 
    { 
      get
      {
        return _area;
      }
      set
      {
        if (value < 0)
          throw new Exception("Area cannot be negative");
        _area = value;

      }
    }


    /// <summary>
    /// Gets and sets the Contact area for the groundwater interaction
    /// </summary>
    public Polygon ContactArea
    {
      get { return _contactArea; }
      set
      {
        _contactArea = value;
        Area = _contactArea.Area;
      }
    }

    /// <summary>
    /// Gets and sets the Water that flows out of this boundary. Volume does not matter.
    /// </summary>
    public IWaterPacket WaterSample
    {
      get
      {
        return WaterProvider.Sample;
      }
      set
      {
        WaterProvider.Sample = value;
      }
    }


    protected InfiniteSource WaterProvider = new InfiniteSource();

  }
}
