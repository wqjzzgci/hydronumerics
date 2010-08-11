using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;


namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public abstract class AbstractBoundary
  {
    [DataMember]
    private XYPolygon _contactArea;
    
    [DataMember]
    private double _area;

    [DataMember]
    public TimeSeriesGroup Output { get; protected set; }

    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Gets and sets the Water that flows out of this boundary. Volume does not matter.
    /// </summary>
    [DataMember]
    public IWaterPacket WaterSample { get; set; }

    [DataMember]
    protected TimespanSeries ts;

    [DataMember]
    protected List<GeoExchangeItem> _exchangeItems = new List<GeoExchangeItem>();

    [DataMember]
    //protected GeoExchangeItem _flow;
    public double Flow { get; set; }


    public AbstractBoundary()
    {
      Output = new TimeSeriesGroup();
      ts = new TimespanSeries();
      
      ts.Name = "Flow";
      Output.Items.Add(ts);
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
    public XYPolygon ContactArea //TODO: Rename to ContactPolygon (JBG)
    {
      get { return _contactArea; }
      set
      {
        _contactArea = value;
        Area = _contactArea.GetArea();
      }
    }

    public List<GeoExchangeItem> ExchangeItems
    {
      get
      {
        return _exchangeItems;
      }
    }


    public virtual void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      ts.AddSiValue(Start,Start.Add(TimeStep), -Water.Volume);
      Flow = -Water.Volume/TimeStep.TotalSeconds;
    }


  }
}
