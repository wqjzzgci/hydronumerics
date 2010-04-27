using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;


using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class WaterBodyOutput
  {

    #region Persisted data
    [DataMember]
    private TimeSeriesGroup tsg = new TimeSeriesGroup();

    #endregion

    #region Non-persisted properties
    public TimeSeries Outflow 
    { 
      get
      {
        return tsg.TimeSeriesList[0];
      }
    }

    public TimeSeries Evaporation
    {
      get
      {
        return tsg.TimeSeriesList[1];
      }
    }
    public TimeSeries Sinks
    {
      get
      {
        return tsg.TimeSeriesList[2];
      }
    }
    public TimeSeries Sources
    {
      get
      {
        return tsg.TimeSeriesList[3];
      }
    }

    public IList<TimeSeries> TimeSeriesList
    {
      get
      {
        return tsg.TimeSeriesList;
      }
    }

    public TimeSeriesGroup Group
    {
      get
      {
        return tsg;
      }
    }


    #endregion

    #region Constructor
    public WaterBodyOutput(string ID)
    {
      TimeSeries Outflow = new TimeSeries();
      Outflow.Name = ID + ": Outflow";
      Outflow.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Outflow.Unit = new HydroNumerics.Core.Unit("m3/s", 1, 0);
      tsg.TimeSeriesList.Add(Outflow);

      TimeSeries Evaporation = new TimeSeries();
      Evaporation.Name = ID + ": Evaporation";
      Evaporation.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Evaporation.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Evaporation);

      TimeSeries Sinks = new TimeSeries();
      Sinks.Name = ID + ": Sinks";
      Sinks.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Sinks.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Sinks);

      TimeSeries Sources = new TimeSeries();
      Sources.Name = ID + ": Sources";
      Sources.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      Sources.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Sources);

      ChemicalsToLog = new Dictionary<Chemical,TimeSeries>();
    }

    #endregion

    /// <summary>
    /// This method deletes all entries after the time
    /// </summary>
    /// <param name="Time"></param>
    public void ResetToTime(DateTime Time)
    {
      foreach (TimeSeries T in tsg.TimeSeriesList)
      {
        int i = T.TimeValues.Count - 1;
        while (i > 0 && T.TimeValues[i].Time >= Time )
        {
          T.TimeValues.RemoveAt(i);
          i--;
        }
      }
    }

    public Dictionary<Chemical,TimeSeries> ChemicalsToLog { get; set; }

    public void LogChemicalConcentration(Chemical Chem)
    {
      TimeSeries ts = new TimeSeries();
      ts.Name = Chem.Name;
      ts.TimeSeriesType = TimeSeriesType.TimeSpanBased;
      ts.Unit = new HydroNumerics.Core.Unit("mol/m3", 1, 0);
      tsg.TimeSeriesList.Add(ts);
      ChemicalsToLog.Add(Chem,ts);
    }


    public void Save(string FileName)
    {
      tsg.Save(FileName);
    }
  }
}
