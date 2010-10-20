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
  public class WaterBodyOutput : TimeSeriesGroup
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

    #region Constructor
    public WaterBodyOutput(string Name)
    {
      TimespanSeries Outflow = new TimespanSeries();
      Outflow.Name = Name + ": Outflow";
      Outflow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Outflow);

      TimespanSeries Evaporation = new TimespanSeries();
      Evaporation.Name = Name + ": Evaporation";
      Evaporation.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Evaporation);

      TimespanSeries Sinks = new TimespanSeries();
      Sinks.Name = Name + ": Sinks";
      Sinks.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Sinks);

      TimespanSeries Sources = new TimespanSeries();
      Sources.Name = Name + ": Sources";
      Sources.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Sources);

      TimespanSeries Inflow = new TimespanSeries();
      Inflow.Name = Name + ": Inflow";
      Inflow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Inflow);

      TimespanSeries Precip = new TimespanSeries();
      Precip.Name = Name + ": Precipitation";
      Precip.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Precip);

      TimespanSeries GWIn = new TimespanSeries();
      GWIn.Name = Name + ": Groundwater inflow";
      GWIn.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(GWIn);

      TimespanSeries GWout = new TimespanSeries();
      GWout.Name = Name + ": Groundwater outflow";
      GWout.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(GWout);

      TimestampSeries StoredVolume = new TimestampSeries();
      StoredVolume.Name = Name + ": Stored volume";
      StoredVolume.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeter);
      Items.Add(StoredVolume);

      ChemicalsToLog = new Dictionary<Chemical, TimestampSeries>();
      LogAllChemicals = false;

      CompositionLog = new Dictionary<int, TimestampSeries>();
      LogComposition = false;
    }

    #endregion

    /// <summary>
    /// Gets the logged chemicals. Empty if nothing logged.
    /// </summary>
    [DataMember]
    public Dictionary<Chemical, TimestampSeries> ChemicalsToLog { get; private set; }

    /// <summary>
    /// Gets the logged composition. Empty if nothing logged.
    /// </summary>
    [DataMember]
    public Dictionary<int, TimestampSeries> CompositionLog { get; private set; }

    /// <summary>
    /// Tells the output to log all concentrations of all chemicals
    /// </summary>
    [DataMember]
    public bool LogAllChemicals { get; set; }

    /// <summary>
    /// Tells the output to log the composition of the water
    /// </summary>
    [DataMember]
    public bool LogComposition { get; set; }

    /// <summary>
    /// Logs a particular chemical. Only has an effect when not all chemicals are being logged.
    /// </summary>
    /// <param name="Chem"></param>
    public void LogChemicalConcentration(Chemical Chem)
    {
      CreateChemicalSeries(Chem);
    }


    /// <summary>
    /// Creates a time series for logging composition
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    private TimestampSeries CreateCompositionTimeSeries(int ID)
    {
      TimestampSeries ts = new TimestampSeries();
      ts.Name = "Composition: " + ID;
      ts.Unit = new HydroNumerics.Core.Unit("-", 1, 0);
      Items.Add(ts);
      CompositionLog.Add(ID, ts);
      return ts;
    }

    /// <summary>
    /// Creates a time series for the chemical
    /// </summary>
    /// <param name="Chem"></param>
    /// <returns></returns>
    private TimestampSeries CreateChemicalSeries(Chemical Chem)
    {
      TimestampSeries ts = new TimestampSeries();
      ts.Name = Chem.Name;
      ts.Unit = UnitFactory.Instance.GetUnit(NamedUnits.molespercubicmeter);
      Items.Add(ts);
      ChemicalsToLog.Add(Chem, ts);
      return ts;
    }

    /// <summary>
    /// Logs chemical
    /// </summary>
    /// <param name="Water"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public void Log(IWaterPacket Water, DateTime Start, DateTime End)
    {
      WaterWithChemicals wc = Water as WaterWithChemicals;

      //Log chemicals if the water is based on WaterWithChemicals
      if (wc!=null)
      {
        if (LogAllChemicals)
          foreach (var c in wc.Chemicals.Keys)
          {
            TimestampSeries ts;
            if (!ChemicalsToLog.TryGetValue(c, out ts))
              ts = CreateChemicalSeries(c);
            ts.AddSiValue(End, wc.GetConcentration(c));
          }
        else
        {
          foreach (KeyValuePair<Chemical, TimestampSeries> ct in ChemicalsToLog)
          {
            ct.Value.AddSiValue(End, ((WaterWithChemicals)Water).GetConcentration(ct.Key));
          }
        }
      }

      //Log the water composition
      if (LogComposition)
      {
        foreach (var id in Water.Composition)
        {
          TimestampSeries ts;
          if (!CompositionLog.TryGetValue(id.Key, out ts))
            ts = CreateCompositionTimeSeries(id.Key);
          ts.AddValue(End, id.Value);
        }
      }
    }

  }
}
