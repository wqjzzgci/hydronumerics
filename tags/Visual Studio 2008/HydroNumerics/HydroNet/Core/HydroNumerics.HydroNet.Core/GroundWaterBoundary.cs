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
  public enum GWType
  {
    [EnumMember]
    Darcy,
    [EnumMember]
    Flow
  }

  [DataContract(IsReference=true)]
  public class GroundWaterBoundary:AbstractBoundary,IGroundwaterBoundary 
  {
    /// <summary>
    /// Gets and sets an enum determining how the flow should be calculated
    /// </summary>
    [DataMember]
    public GWType FlowType {get;set;}

    [DataMember]
    public IWaterPacket WaterSample { get; set; }

    /// <summary>
    /// Gets the accumulated water that has flown into this boundary. Is not reset
    /// </summary>
    [DataMember]
    public IWaterPacket ReceivedWater { get; private set; }

    /// <summary>
    /// Gets the Current flow rate
    /// </summary>
    public double CurrentFlowRate { get; private set; }

    /// <summary>
    /// Returns true if this is a Darcy-type groundwater connection
    /// </summary>
    public bool IsDarcy
    {
      get
      {
        return FlowType == GWType.Darcy;
      }
      set
      {
        if (value)
          FlowType = GWType.Darcy;
        else
          FlowType = GWType.Flow;
      }
    }

    [DataMember]
    private TimespanSeries _waterFlow;

    public TimespanSeries WaterFlow
    {
      get
      {
        if (_waterFlow == null)
          CreateWaterFlowSeries();
        return _waterFlow;
      }
      set
      {
        _waterFlow = value;
      }
    }

    [DataMember]
    public IWaterBody Connection{get;set;}

    /// <summary>
    /// Gets and sets the hydraulic conductivity
    /// </summary>
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
      WaterSample = new WaterPacket(1);
      FlowType = GWType.Darcy;
    }

    private void CreateWaterFlowSeries()
    {
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

    public void Initialize()
    {
    }

    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      switch (FlowType)
      {
        case GWType.Darcy:
          CurrentFlowRate = ((XYPolygon)ContactGeometry).GetArea() * HydraulicConductivity * (GroundwaterHead - Connection.WaterLevel) / Distance;
          WaterFlow.AddSiValue(Start, Start.Add(TimeStep), CurrentFlowRate);
          break;
        case GWType.Flow:
          CurrentFlowRate = WaterFlow.GetSiValue(Start, Start.Add(TimeStep));
          break;
        default:
          break;
      }

      return WaterSample.DeepClone(CurrentFlowRate * TimeStep.TotalSeconds);
    }

    /// <summary>
    /// Positive
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      double WaterVolume=0;
      switch (FlowType)
      {
        case GWType.Darcy:
          WaterVolume = ((XYPolygon)ContactGeometry).GetArea() * HydraulicConductivity * (Connection.WaterLevel - GroundwaterHead) / Distance * TimeStep.TotalSeconds;
          break;
        case GWType.Flow:
          WaterVolume = -WaterFlow.GetSiValue(Start, Start.Add(TimeStep)) * TimeStep.TotalSeconds;
          break;
      }
      return WaterVolume;
    }

    public void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      CurrentFlowRate = -Water.Volume / TimeStep.TotalSeconds;

      if (FlowType== GWType.Darcy)
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
      switch (FlowType)
      {
        case GWType.Darcy:
          return Connection.WaterLevel < GroundwaterHead;
        case GWType.Flow:
          return WaterFlow.GetSiValue(time) > 0;
      }
      return false;
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
