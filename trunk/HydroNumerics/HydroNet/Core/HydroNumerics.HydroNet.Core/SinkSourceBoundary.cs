using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class SinkSourceBoundary:AbstractBoundary,ISource, ISink 
  {
    [DataMember]
    private BaseTimeSeries TS = null;

    [DataMember]
    private double _currentFlowRate;

    [DataMember(Order=1)]
    private GeoExchangeItem _flowExchangeItem;

    [DataMember]
    private bool _useExchangeItem = false;

    /// <summary>
    /// Gets and sets the type of water this boundary will deliver
    /// </summary>
    [DataMember]
    public IWaterPacket WaterSample { get; set; }

    /// <summary>
    /// Gets the latest time this boundary can provide data
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        if (TS == null)
          return DateTime.MaxValue;
        else
          return TS.EndTime;
      }
    }

    /// <summary>
    /// Gets the earliest time this boundary can provide data
    /// </summary>
    public DateTime StartTime
    {
      get
      {
        if (TS == null)
          return DateTime.MinValue;
        else
          return TS.StartTime;
      }
    }


    public void Initialize()
    {
    }

    public SinkSourceBoundary()
    {
      WaterSample = new WaterPacket(1);
      ContactGeometry = XYPolygon.GetSquare(1);
      BuildExchangeItems();
    }

    private void BuildExchangeItems()
    {
      _flowExchangeItem = new GeoExchangeItem();
      _flowExchangeItem.Geometry = ContactGeometry;
      _flowExchangeItem.IsInput = true;
      _flowExchangeItem.Quantity = "Flow";
      _flowExchangeItem.timeType = TimeType.TimeSpan;
      _flowExchangeItem.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      ExchangeItems.Add(_flowExchangeItem);

      _flowExchangeItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(flowExchangeItem_PropertyChanged);
    }

    [OnDeserialized]
    private void ReconnectEvents(StreamingContext context)
    {
      _flowExchangeItem.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(flowExchangeItem_PropertyChanged);
      Name = Name;
    }


    public override string Name
    {
      get
      {
        return base.Name;
      }
      set
      {
        base.Name = value;

        if (_flowExchangeItem != null)//During deserialization _flowexchangeitem is null. Changing the order has not been succesfull
        {
          _flowExchangeItem.Description = value;
          _flowExchangeItem.Location = value;
        }
      }
    }

    void flowExchangeItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      _useExchangeItem = true;
    }


    public SinkSourceBoundary(double FlowRate):this()
    {
      _currentFlowRate = FlowRate;
    }

    public SinkSourceBoundary(BaseTimeSeries ts) : this()
    {
      TS = ts;
    }

    public TimespanSeries TimeValues
    {
      get
      {
        return (TimespanSeries)TS;
      }
    }
    
    #region IWaterSource Members


    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      if (_useExchangeItem)
        _currentFlowRate = _flowExchangeItem.ExchangeValue;

      else if (TS != null)
          _currentFlowRate = TS.GetSiValue(Start, Start.Add(TimeStep));

      double _routedFlow =  _currentFlowRate * TimeStep.TotalSeconds;

      //Could be a point
      XYPolygon geom = ContactGeometry as XYPolygon;
      if (geom != null)
        _routedFlow *= geom.GetArea();

      return WaterSample.DeepClone(_routedFlow);
    }

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      if (TS != null)
        _currentFlowRate = TS.GetSiValue(Start, Start.Add(TimeStep));

      return -((XYPolygon)ContactGeometry).GetArea() * _currentFlowRate * TimeStep.TotalSeconds;
    }

    public void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {}


    public bool Source(DateTime time)
    {
      if (TS != null)
        _currentFlowRate = TS.GetSiValue(time);
      return _currentFlowRate >= 0;
    }

    #endregion
  }
}
