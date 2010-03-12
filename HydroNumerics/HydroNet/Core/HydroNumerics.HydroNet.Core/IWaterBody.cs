using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMap.Geometries;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public interface IWaterBody
  {
    double WaterLevel{get;}

    //Jag: Should it be possible to remove?
    void AddDownstreamConnection(IWaterBody Element);
    void AddWaterSinkSource(IWaterSinkSource Source);
    void AddEvaporationBoundary(IEvaporationBoundary Evapo);
    void MoveInTime(TimeSpan TimeStep);
    void ReceiveWater(TimeSpan TimeStep, IWaterPacket Water);
    IWaterPacket CurrentStoredWater{get;}
    IGeometry Geometry { get; set; }
    List<IWaterBody> DownStream { get; }
    int ID { get; set; }
    DateTime CurrentStartTime { get; set; }
    TimeSeriesGroup Output { get; }
  }
}
