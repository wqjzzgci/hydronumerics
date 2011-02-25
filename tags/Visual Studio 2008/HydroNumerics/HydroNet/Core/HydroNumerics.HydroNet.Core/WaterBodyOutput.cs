using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class WaterBodyOutput : WaterOutput
  {

    #region Non-persisted properties
    /// <summary>
    /// Gets the outflow to downstream waterbodies
    /// </summary>
    public TimespanSeries Outflow
    {
      get
      {
        return Items[0] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the evaporation from the waterbody
    /// </summary>
    public TimespanSeries Evaporation
    {
      get
      {
        return Items[1] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the direct sinks from the waterbody
    /// </summary>
    public TimespanSeries Sinks
    {
      get
      {
        return Items[2] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the direct sources from the waterbody
    /// </summary>
    public TimespanSeries Sources
    {
      get
      {
        return Items[3] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the inflow from upstream connections
    /// </summary>
    public TimespanSeries Inflow
    {
      get
      {
        return Items[4] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the precipitation to the waterbody
    /// </summary>
    public TimespanSeries Precipitation
    {
      get
      {
        return Items[5] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the groundwater inflow to the water body
    /// </summary>
    public TimespanSeries GroundwaterInflow
    {
      get
      {
        return Items[6] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the outflow from the water body to groundwater boundaries
    /// </summary>
    public TimespanSeries GroundwaterOutflow
    {
      get
      {
        return Items[7] as TimespanSeries;
      }
    }

    /// <summary>
    /// Gets the stored volume in this water body
    /// </summary>
    public TimestampSeries StoredVolume
    {
      get
      {
        return Items[8] as TimestampSeries;
      }
    }

    #endregion

    /// <summary>
    /// Gets the average storage time for the time period. Calculated as mean volume divide by mean (sinks + outflow + evaporation)
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public TimeSpan GetStorageTime(DateTime Start, DateTime End)
    {
      if (!IsEmpty)
      {
        if (EndTime < End || StartTime > Start)
          throw new Exception("Cannot calculate storage time outside of the simulated period");

        //Find the total outflow
        double d = Sinks.GetSiValue(Start, End);
        d += Outflow.GetSiValue(Start, End);
        //Evaporation is negative
        d += Evaporation.GetSiValue(Start, End);
        d += GroundwaterOutflow.GetSiValue(Start, End);

        return TimeSpan.FromSeconds(StoredVolume.GetSiValue(Start, End) / d);
      }
      return TimeSpan.Zero;
    }


    public void Summarize(int MaxCount)
    {
      foreach (BaseTimeSeries bts in Items)
        Summarize(MaxCount, bts);
      foreach (BaseTimeSeries bts in ChemicalsToLog.Values)
        Summarize(MaxCount, bts);
      foreach (BaseTimeSeries bts in CompositionLog.Values)
        Summarize(MaxCount, bts);
    }

    /// <summary>
    /// Returns true if all the timeseries are empty
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return Items.Max(var => var.Values.Count()) == 0;
      }
    }


    private void Summarize(int MaxCount, BaseTimeSeries bts)
    {
        TimespanSeries tspan = bts as TimespanSeries;
        TimestampSeries tstam = bts as TimestampSeries;

        if (tspan != null)
        {
          if (tspan.Items.Count > MaxCount)
          {
            List<TimespanValue> temp = new List<TimespanValue>();

            DateTime Start = tspan.Items.First().StartTime;
            DateTime End = tspan.Items.Last().EndTime;
            double periodDays = End.Subtract(Start).TotalDays / MaxCount;

            for (int i = 0; i < MaxCount; i++)
            {
              TimespanValue TValue = new TimespanValue(Start.AddDays(i * periodDays), Start.AddDays((i + 1) * periodDays), 0);
              TValue.Value = tspan.GetValue(TValue.StartTime, TValue.EndTime);
              temp.Add(TValue);
            }
            tspan.Items.Clear();

            foreach (var v in temp)
              tspan.Items.Add(v);
          }
        }
        if (tstam != null)
        {
          if (tstam.Items.Count > MaxCount)
          {
            List<TimestampValue> temp = new List<TimestampValue>();

            DateTime Start = tstam.Items.First().Time;
            DateTime End = tstam.Items.Last().Time;
            double periodDays = End.Subtract(Start).TotalDays / MaxCount;

            for (int i = 0; i < MaxCount; i++)
            {
              TimestampValue TValue = new TimestampValue(Start.AddDays(i * periodDays), 0);
              TValue.Value = tstam.GetValue(TValue.Time);
              temp.Add(TValue);
            }
            tstam.Items.Clear();

            foreach (var v in temp)
              tstam.Items.Add(v);
          }
        }
      
    }



    #region Constructor
    public WaterBodyOutput(string Name):base()
    {
      TimespanSeries Outflow = new TimespanSeries();
      Outflow.Name = "Outflow";
      Outflow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Outflow);

      TimespanSeries Evaporation = new TimespanSeries();
      Evaporation.Name = "Evaporation";
      Evaporation.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Evaporation);

      TimespanSeries Sinks = new TimespanSeries();
      Sinks.Name = "Sinks";
      Sinks.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Sinks);

      TimespanSeries Sources = new TimespanSeries();
      Sources.Name = "Sources";
      Sources.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Sources);

      TimespanSeries Inflow = new TimespanSeries();
      Inflow.Name = "Inflow";
      Inflow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Inflow);

      TimespanSeries Precip = new TimespanSeries();
      Precip.Name = "Precipitation";
      Precip.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Precip);

      TimespanSeries GWIn = new TimespanSeries();
      GWIn.Name = "Groundwater inflow";
      GWIn.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(GWIn);

      TimespanSeries GWout = new TimespanSeries();
      GWout.Name = "Groundwater outflow";
      GWout.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(GWout);

      TimestampSeries StoredVolume = new TimestampSeries();
      StoredVolume.Name = "Stored volume";
      StoredVolume.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeter);
      Items.Add(StoredVolume);

    }
    #endregion
  }
}
