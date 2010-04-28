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
    public TimespanSeries Outflow 
    { 
      get
      {
        return tsg.TimeSeriesList[0] as TimespanSeries;
      }
    }

    public TimespanSeries Evaporation
    {
      get
      {
        return tsg.TimeSeriesList[1] as TimespanSeries;
      }
    }

    public TimespanSeries Sinks
    {
      get
      {
        return tsg.TimeSeriesList[2] as TimespanSeries;
      }
    }

    public TimespanSeries Sources
    {
      get
      {
        return tsg.TimeSeriesList[3] as TimespanSeries;
      }
    }

    public IList<BaseTimeSeries> TimeSeriesList
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
      TimespanSeries Outflow = new TimespanSeries();
      Outflow.Name = ID + ": Outflow";
      Outflow.Unit = new HydroNumerics.Core.Unit("m3/s", 1, 0);
      tsg.TimeSeriesList.Add(Outflow);

      TimespanSeries Evaporation = new TimespanSeries();
      Evaporation.Name = ID + ": Evaporation";
      Evaporation.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Evaporation);

      TimespanSeries Sinks = new TimespanSeries();
      Sinks.Name = ID + ": Sinks";
      Sinks.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Sinks);

      TimespanSeries Sources = new TimespanSeries();
      Sources.Name = ID + ": Sources";
      Sources.Unit = Outflow.Unit;
      tsg.TimeSeriesList.Add(Sources);

      ChemicalsToLog = new Dictionary<Chemical,TimespanSeries>();
    }

    #endregion

    /// <summary>
    /// This method deletes all entries after the time
    /// </summary>
    /// <param name="Time"></param>
    public void ResetToTime(DateTime Time)
    {
      foreach (BaseTimeSeries T in tsg.TimeSeriesList)
      {
        //int i = T.TimeValues.Count - 1;
        //while (i > 0 && T.TimeValues[i].Time >= Time )
        //{
        //  T.TimeValues.RemoveAt(i);
        //  i--;
        //}
      }
    }

    public Dictionary<Chemical,TimespanSeries> ChemicalsToLog { get; set; }

    public void LogChemicalConcentration(Chemical Chem)
    {
      TimespanSeries ts = new TimespanSeries();
      ts.Name = Chem.Name;
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
