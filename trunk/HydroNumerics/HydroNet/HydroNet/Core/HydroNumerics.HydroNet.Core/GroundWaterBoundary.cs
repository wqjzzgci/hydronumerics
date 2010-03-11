using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class GroundWaterBoundary:AbstractBoundary,IWaterSinkSource
  {
    public IWaterBody Connection{get;set;}
    public double HydraulicConductivity { get; set; }
    public double Area { get; set; }
    public double Distance { get; set; }
    public double Head { get; set; }

    private double WaterVolume;

    public GroundWaterBoundary(IWaterBody connection, double hydraulicConductivity, double area, double distance, double head)
    {
      Connection = connection;
      HydraulicConductivity = hydraulicConductivity;
      Area = area;
      Distance = distance;
      Head = head;
    }


    #region IWaterSource Members

    public string ID {get; set;}

    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      WaterVolume = Area * HydraulicConductivity * (Head - Connection.WaterLevel) / Distance * TimeStep.TotalSeconds;
      return WaterProvider.GetWater(WaterVolume);
    }

    /// <summary>
    /// Positive
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      return Area * HydraulicConductivity * (Connection.WaterLevel - Head) / Distance * TimeStep.TotalSeconds;
    }

    /// <summary>
    /// Returns true if water is flowing into the stream.
    /// </summary>
    public bool Source(DateTime time)
    {
      return Connection.WaterLevel < Head;
    }


    #endregion
  }
}
