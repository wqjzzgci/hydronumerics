using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using SharpMap.Geometries;
using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class GroundWaterBoundary:AbstractBoundary,IWaterSinkSource
  {
    [DataMember]
    public IWaterBody Connection{get;set;}
    [DataMember]
    public double HydraulicConductivity { get; set; }
    [DataMember]
    public double Distance { get; set; }

    [DataMember]
    private ExchangeItem _head;


    public GroundWaterBoundary()
      : base()
    {
    }

    public GroundWaterBoundary(IWaterBody connection, double hydraulicConductivity, double area, double distance, double head)
    {
      Connection = connection;
      HydraulicConductivity = hydraulicConductivity;
      Area = area;
      Distance = distance;
      

      _head = new ExchangeItem(this.Name + "GWB","Head", UnitFactory.Instance.GetUnit(NamedUnits.meter));
      _head.ExchangeValue = head;
      _head.IsInput = true;
      _head.IsOutput = false;
      _exchangeItems.Add(_head);

      _flow.Location = this.Name + "GWB";
    }


    #region IWaterSource Members


    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      double WaterVolume = Area * HydraulicConductivity * (Head - Connection.WaterLevel) / Distance * TimeStep.TotalSeconds;
      ts.AddSiValue(Start, Start.Add(TimeStep), WaterVolume);
      _flow.ExchangeValue = WaterVolume;

      return WaterSample.DeepClone(WaterVolume);
    }

    /// <summary>
    /// Positive
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
     

      double WaterVolume= Area * HydraulicConductivity * (Connection.WaterLevel - Head) / Distance * TimeStep.TotalSeconds;

      return WaterVolume;
    }

    /// <summary>
    /// Returns true if water is flowing into the stream.
    /// </summary>
    public bool Source(DateTime time)
    {
      return Connection.WaterLevel < Head;
    }


    /// <summary>
    /// Get and sets the head
    /// </summary>
    public double Head
    {
      get
      {
        return _head.ExchangeValue;
      }
      set
      {
        _head.ExchangeValue = value;
      }
    }

    #endregion
  }
}
