using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public class WaterBodyOutput:TimeSeriesGroup 
  {
    [XmlIgnore]
    public TimeSeries Outflow { get; private set; }
    [XmlIgnore]
    public TimeSeries Evaporation { get; private set; }
    [XmlIgnore]
    public TimeSeries Sinks { get; private set; }
    [XmlIgnore]
    public TimeSeries Sources { get; private set; }


    public WaterBodyOutput()
    {
    }

    public WaterBodyOutput(string ID)
    {
      Outflow = new TimeSeries();
      Outflow.Name = ID + ": Outflow";
      Outflow.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Outflow.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("m3/s", 1, 0);
      this.TimeSeriesList.Add(Outflow);

      Evaporation = new TimeSeries();
      Evaporation.Name = ID + ": Evaporation";
      Evaporation.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Evaporation.Unit = Outflow.Unit;
      TimeSeriesList.Add(Evaporation);

      Sinks = new TimeSeries();
      Sinks.Name = ID + ": Sinks";
      Sinks.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Sinks.Unit = Outflow.Unit;
      TimeSeriesList.Add(Sinks);

      Sources = new TimeSeries();
      Sources.Name = ID + ": Sources";
      Sources.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Sources.Unit = Outflow.Unit;
      TimeSeriesList.Add(Sources);
    }
  }
}
