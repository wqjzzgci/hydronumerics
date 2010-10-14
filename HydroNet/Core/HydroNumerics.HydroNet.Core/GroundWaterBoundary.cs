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
  public class GroundWaterBoundary:AbstractBoundary,IGroundwaterBoundary 
  {
    [DataMember]
    public IWaterPacket WaterSample { get; set; }

    [DataMember]
    public IWaterPacket ReceivedWater { get; private set; }

    [DataMember]
    public TimespanSeries WaterFlow { get; private set; }

    [DataMember]
    public IWaterBody Connection{get;set;}
    [DataMember]
    public double HydraulicConductivity { get; set; }
    [DataMember]
    public double Distance { get; set; }

    [DataMember]
    //private GeoExchangeItem _head;
    public double GroundwaterHead{get; set;}

    public GroundWaterBoundary() : base()
    {
      Initialize();
      WaterFlow = new TimespanSeries();
      WaterFlow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      WaterFlow.Name = "Groundwater flow";
    }

    public GroundWaterBoundary(IWaterBody connection, double hydraulicConductivity, double distance, double groundwaterHead, XYPolygon ContactPolygon):this()
    {
      Connection = connection;
      HydraulicConductivity = hydraulicConductivity;
      Distance = distance;
      GroundwaterHead = groundwaterHead;
      ContactGeometry = ContactPolygon;
    }

    #region IWaterSource Members


    public override void Initialize()
    {
        _exchangeItems = new List<GeoExchangeItem>();
        GeoExchangeItem GroundWaterHeadExchangeItem = new GeoExchangeItem();
        GroundWaterHeadExchangeItem.Description = "Ground water head for: " + Name;
        GroundWaterHeadExchangeItem.Geometry = ContactGeometry;
        GroundWaterHeadExchangeItem.ExchangeValue = GroundwaterHead;
        GroundWaterHeadExchangeItem.IsInput = true;
        GroundWaterHeadExchangeItem.IsOutput = false;
//        GroundWaterHeadExchangeItem.Location = "Near " + Connection.Name;
        GroundWaterHeadExchangeItem.Quantity = "Ground water head";
        GroundWaterHeadExchangeItem.timeType = TimeType.TimeStamp;
        GroundWaterHeadExchangeItem.Unit = UnitFactory.Instance.GetUnit(NamedUnits.meter);
               
        ExchangeItems.Add(GroundWaterHeadExchangeItem);

        //TODO: The code below is a dummy implementation, just for demonstration. MUST be implemented correctly later....!!!!
        GeoExchangeItem leakageExchangeItem = new GeoExchangeItem();
 //       leakageExchangeItem.Description = "leakage from: " + Connection.Name;
        leakageExchangeItem.Geometry = ContactGeometry;
        leakageExchangeItem.IsInput = false;
        leakageExchangeItem.IsOutput = true;
  //      leakageExchangeItem.Location = "Near " + Connection.Name;
        leakageExchangeItem.Quantity = "Leakage";
        leakageExchangeItem.timeType = TimeType.TimeSpan;
        leakageExchangeItem.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
        ExchangeItems.Add(leakageExchangeItem);
           


    }
    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      double WaterVolume = ((XYPolygon)ContactGeometry).GetArea() * HydraulicConductivity * (GroundwaterHead - Connection.WaterLevel) / Distance * TimeStep.TotalSeconds;
      WaterFlow.AddSiValue(Start, Start.Add(TimeStep), WaterVolume / TimeStep.TotalSeconds);
      Flow = WaterVolume; //TODO: Check det her. Der er lidt farligt når en state variable som Flow updateres ved at kalde GetSourceWater 

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
      double WaterVolume = ((XYPolygon)ContactGeometry).GetArea() * HydraulicConductivity * (Connection.WaterLevel - GroundwaterHead) / Distance * TimeStep.TotalSeconds;
      return WaterVolume;
    }

    public override void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      WaterFlow.AddSiValue(Start, Start.Add(TimeStep), -Water.Volume/TimeStep.TotalSeconds);

      if (ReceivedWater == null)
        ReceivedWater = Water;
      else
        ReceivedWater.Add(Water);
    }
    /// <summary>
    /// Returns true if water is flowing into the stream.
    /// </summary>
    public bool IsSource(DateTime time)
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

    public DateTime StartTime
    {
      get
      {
        return DateTime.MinValue;
      }
    }

    #endregion
  }
}
