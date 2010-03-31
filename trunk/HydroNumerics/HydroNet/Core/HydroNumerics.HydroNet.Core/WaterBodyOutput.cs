using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public class WaterBodyOutput 
  {
    public TimeSeries Outflow { get; private set; }
    public TimeSeries Evaporation { get; private set; }
    public TimeSeries Sinks { get; private set; }
    public TimeSeries Sources { get; private set; }
    private TimeSeriesGroup tsg = new TimeSeriesGroup();


    public WaterBodyOutput(string ID)
    {
      Outflow = new TimeSeries();
      Outflow.Name = ID + ": Outflow";
      Outflow.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Outflow.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("m3/s", 1, 0);
      tsg.TimeSeriesList.Add(Outflow);

      Evaporation = new TimeSeries();
      Evaporation.Name = ID + ": Evaporation";
      Evaporation.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Evaporation.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Evaporation);

      Sinks = new TimeSeries();
      Sinks.Name = ID + ": Sinks";
      Sinks.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Sinks.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Sinks);

      Sources = new TimeSeries();
      Sources.Name = ID + ": Sources";
      Sources.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Sources.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Sources);
    }

    public IList<TimeSeries> TimeSeriesList
    {
      get
      {
        return tsg.TimeSeriesList;
      }
    }

    public void Save(string FileName)
    {
      tsg.Save(FileName);
    }
  }
}
