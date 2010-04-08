using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using SharpMap.Geometries;
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
    public double Head { get; set; }

    private double WaterVolume;


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
      Head = head;
    }


    #region IWaterSource Members


    public IWaterPacket GetSourceWater(DateTime Start, TimeSpan TimeStep)
    {
      WaterVolume = Area * HydraulicConductivity * (Head - Connection.WaterLevel) / Distance * TimeStep.TotalSeconds;
      Output.TimeSeriesList.First().AddTimeValueRecord(new TimeValue(Start, WaterVolume));

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
