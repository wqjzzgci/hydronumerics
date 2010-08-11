using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

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
    //private GeoExchangeItem _head;
    private double GroundwaterHead{get; set;}

    public GroundWaterBoundary() : base()
    {
    }

    public GroundWaterBoundary(IWaterBody connection, double hydraulicConductivity, double area, double distance, double groundwaterHead)
    {
      Connection = connection;
      HydraulicConductivity = hydraulicConductivity;
      Area = area;
      Distance = distance;
      GroundwaterHead = groundwaterHead;
    }

    #region IWaterSource Members


    public void Initialize()
    {
        _exchangeItems = new List<GeoExchangeItem>();
        GeoExchangeItem GroundWaterHeadExchangeItem = new GeoExchangeItem();
        GroundWaterHeadExchangeItem.Description = "Ground water head for: " + Name;
        GroundWaterHeadExchangeItem.Geometry = ContactArea;
        GroundWaterHeadExchangeItem.ExchangeValue = GroundwaterHead;
        GroundWaterHeadExchangeItem.IsInput = true;
        GroundWaterHeadExchangeItem.IsOutput = true;
        GroundWaterHeadExchangeItem.Location = "Near " + Connection.Name;
        GroundWaterHeadExchangeItem.Quantity = "Ground water head";
        GroundWaterHeadExchangeItem.timeType = TimeType.TimeStamp;
        GroundWaterHeadExchangeItem.Unit = UnitFactory.Instance.GetUnit(NamedUnits.meter);

        ExchangeItems.Add(GroundWaterHeadExchangeItem);

    }
    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      double WaterVolume = Area * HydraulicConductivity * (GroundwaterHead - Connection.WaterLevel) / Distance * TimeStep.TotalSeconds;
      ts.AddSiValue(Start, Start.Add(TimeStep), WaterVolume);
      Flow = WaterVolume;

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
      double WaterVolume = Area * HydraulicConductivity * (Connection.WaterLevel - GroundwaterHead) / Distance * TimeStep.TotalSeconds;
      return WaterVolume;
    }

    /// <summary>
    /// Returns true if water is flowing into the stream.
    /// </summary>
    public bool Source(DateTime time)
    {
      return Connection.WaterLevel < GroundwaterHead;
    }


    public DateTime EndTime
    {
      get
      {
        return DateTime.MaxValue;
      }
    }

    #endregion
  }
}
