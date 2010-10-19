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

    /// <summary>
    /// Gets the accumulated water that has flown into this boundary. Is not reset
    /// </summary>
    [DataMember]
    public IWaterPacket ReceivedWater { get; private set; }

    public double CurrentFlowRate { get; private set; }

    [DataMember]
    public TimespanSeries WaterFlow { get; private set; }

    [DataMember]
    public IWaterBody Connection{get;set;}
    [DataMember]
    public double HydraulicConductivity { get; set; }
    [DataMember]
    public double Distance { get; set; }

    /// <summary>
    /// Gets and sets the groundwater head
    /// </summary>
    [DataMember]
    public double GroundwaterHead { get; set; }

    public GroundWaterBoundary() : base()
    {
      WaterFlow = new TimespanSeries();
      WaterFlow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      WaterFlow.Name = "Groundwater flow";
      Output.Items.Add(WaterFlow);
      WaterSample = new WaterPacket(1);

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

    public void Initialize()
    {
    }

    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      double WaterVolume = ((XYPolygon)ContactGeometry).GetArea() * HydraulicConductivity * (GroundwaterHead - Connection.WaterLevel) / Distance * TimeStep.TotalSeconds;

      CurrentFlowRate = WaterVolume / TimeStep.TotalSeconds;
      WaterFlow.AddSiValue(Start, Start.Add(TimeStep), CurrentFlowRate);

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

    public void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      CurrentFlowRate = -Water.Volume / TimeStep.TotalSeconds;

      WaterFlow.AddSiValue(Start, Start.Add(TimeStep), CurrentFlowRate);

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
