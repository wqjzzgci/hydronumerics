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
    public TimespanSeries Outflow
    {
      get
      {
        return Items[0] as TimespanSeries;
      }
    }

    public TimespanSeries Evaporation
    {
      get
      {
        return Items[1] as TimespanSeries;
      }
    }

    public TimespanSeries Sinks
    {
      get
      {
        return Items[2] as TimespanSeries;
      }
    }

    public TimespanSeries Sources
    {
      get
      {
        return Items[3] as TimespanSeries;
      }
    }

    public TimespanSeries Inflow
    {
      get
      {
        return Items[4] as TimespanSeries;
      }
    }

    public TimespanSeries Precipitation
    {
      get
      {
        return Items[5] as TimespanSeries;
      }
    }

    public TimespanSeries GroundwaterInflow
    {
      get
      {
        return Items[6] as TimespanSeries;
      }
    }

    public TimespanSeries GroundwaterOutflow
    {
      get
      {
        return Items[7] as TimespanSeries;
      }
    }

    public TimestampSeries StoredVolume
    {
      get
      {
        return Items[8] as TimestampSeries;
      }
    }


    #endregion

    public void Summarize(int MaxCount)
    {
      foreach (BaseTimeSeries bts in Items)
        Summarize(MaxCount, bts);
      foreach (BaseTimeSeries bts in ChemicalsToLog.Values)
        Summarize(MaxCount, bts);
      foreach (BaseTimeSeries bts in CompositionLog.Values)
        Summarize(MaxCount, bts);
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
