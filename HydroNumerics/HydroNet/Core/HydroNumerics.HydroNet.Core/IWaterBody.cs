using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  public interface IWaterBody
  {
    int ID { get; set; }
    string Name { get; set; }
    double Area { get; }
    double Volume { get; }
    double WaterLevel { get; set; }
    IGeometry Geometry { get; }

    Collection<ISource> Sources { get; }
    Collection<ISink> Sinks { get; }
    Collection<ISink> EvaporationBoundaries { get; }
    Collection<IGroundwaterBoundary> GroundwaterBoundaries { get; }

    Collection<GeoExchangeItem> ExchangeItems { get; }


    void AddDownStreamWaterBody(IWaterBody waterbody);

    void Initialize();

    void Update(DateTime NewTime);

    
    /// <summary>
    /// Adds a water packet to the waterbody. 
    /// This method is to be used by upstream connections.
    /// 
    /// </summary>
    /// <param name="Start">Start of inflow period</param>
    /// <param name="End">End of inflow period</param>
    /// <param name="Water"></param>
    void AddWaterPacket(DateTime Start, DateTime End, IWaterPacket Water);

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
