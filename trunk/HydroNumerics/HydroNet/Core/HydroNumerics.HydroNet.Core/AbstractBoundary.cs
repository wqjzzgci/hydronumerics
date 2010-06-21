using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
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
    public string Name { get; set; }

    /// <summary>
    /// Gets and sets the Water that flows out of this boundary. Volume does not matter.
    /// </summary>
    [DataMember]
    public IWaterPacket WaterSample { get; set; }

    [DataMember]
    protected TimespanSeries ts;

    [DataMember]
    protected List<ExchangeItem> _exchangeItems = new List<ExchangeItem>();

    [DataMember]
    protected ExchangeItem _flow;


    public AbstractBoundary()
    {
      Output = new TimeSeriesGroup();
      ts = new TimespanSeries();
      
      ts.Name = "Flow";
      Output.Items.Add(ts);

      _flow = new ExchangeItem("?", "Flow", UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond));
      _flow.Quantity = "Flow";
      _flow.IsOutput = true;
      _flow.IsInput = false;

      _exchangeItems.Add(_flow);
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

    public List<ExchangeItem> ExchangeItems
    {
      get
      {
        return _exchangeItems;
      }
    }


    public virtual void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      ts.AddSiValue(Start,Start.Add(TimeStep), -Water.Volume);
      _flow.ExchangeValue = -Water.Volume/TimeStep.TotalSeconds;
    }


  }
}
