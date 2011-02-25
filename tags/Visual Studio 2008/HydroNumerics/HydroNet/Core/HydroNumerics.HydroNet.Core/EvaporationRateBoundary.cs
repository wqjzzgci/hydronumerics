using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class EvaporationRateBoundary:AbstractBoundary, ISink  
  {

    [DataMember]
    TimespanSeries _evaporationRate = null;
    
    [DataMember]
    double _currentEvaporation;

    private EvaporationRateBoundary()
    {
      ContactGeometry = XYPolygon.GetSquare(1);
    }
    
    public EvaporationRateBoundary(double EvaporationRate):this()
    {
      _currentEvaporation = EvaporationRate;
    }

    public EvaporationRateBoundary(TimespanSeries EvaporationRate)
      : this()
    {
      _evaporationRate = EvaporationRate;
    }


    #region ISink Members

    public double GetSinkVolume(DateTime Start, TimeSpan TimeStep)
    {
      if (_evaporationRate != null)
        _currentEvaporation = _evaporationRate.GetSiValue(Start, Start.Add(TimeStep));
      return _currentEvaporation * ((XYPolygon)ContactGeometry).GetArea() * TimeStep.TotalSeconds;;
    }

    #endregion

    #region IBoundary Members


    public void Initialize()
    {
    }

    public DateTime EndTime
    {
      get
      {
        if (_evaporationRate == null)
          return DateTime.MaxValue;
        else
          return _evaporationRate.EndTime;
      }
    }

    public DateTime StartTime
    {
      get
      {
        if (_evaporationRate == null)
          return DateTime.MinValue;
        else
          return _evaporationRate.StartTime;
      }
    }

    public TimespanSeries TimeValues
    {
      get
      {
        return _evaporationRate;
      }
    }

    public void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {}

    #endregion
  }
}
