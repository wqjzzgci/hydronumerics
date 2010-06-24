using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public interface IWaterBody
  {
    int ID { get; set; }
    string Name { get; set; }
    double Area { get; }
    double Volume { get; }
    double WaterLevel { get; set; }

    Collection<IWaterBody> DownStreamConnections { get; }
    Collection<IWaterSinkSource> SinkSources { get; }
    Collection<IEvaporationBoundary> EvaporationBoundaries { get; }

    void MoveInTime(TimeSpan TimeStep);
    void ReceiveWater(DateTime Start, DateTime End, IWaterPacket Water);

    IWaterPacket CurrentStoredWater{get;}
    DateTime CurrentTime { get; }

    void SetState(string StateName, DateTime Time, IWaterPacket WaterInStream);
    void KeepCurrentState(string StateName);
    void RestoreState(string StateName);

    /// <summary>
    /// Gets the maximum time this waterbody can run to.
    /// </summary>
    DateTime EndTime { get; }

  }
}
