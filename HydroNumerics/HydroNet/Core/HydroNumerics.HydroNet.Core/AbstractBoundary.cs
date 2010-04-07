using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

using SharpMap.Geometries;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public abstract class AbstractBoundary
  {
    private Polygon _contactArea;
    [DataMember]
    private double _area;
    [DataMember]
    public TimeSeriesGroup Output { get; protected set; }

    [DataMember]
    public string ID { get; set; }

    [DataMember]
    protected InfiniteSource WaterProvider = new InfiniteSource();

    public AbstractBoundary()
    {
      Output = new TimeSeriesGroup();
      TimeSeries ts= new TimeSeries();
      ts.Name = "Flow";
      Output.TimeSeriesList.Add(ts);
    }


    /// <summary>
    /// Gets and sets the area
    /// </summary>
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
    /// Gets and sets the Contact area for the boundary
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


    public virtual void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      Output.TimeSeriesList.First().AddTimeValueRecord(new TimeValue(Start, -Water.Volume));
    }


  }
}
