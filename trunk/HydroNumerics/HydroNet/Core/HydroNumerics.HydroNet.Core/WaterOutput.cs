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
  public class WaterOutput : TimeSeriesGroup
  {

    #region Constructor
    public WaterOutput()
    {
      ChemicalsToLog = new Dictionary<Chemical, TimestampSeries>();
      LogAllChemicals = false;

      CompositionLog = new Dictionary<int, TimestampSeries>();
      LogComposition = false;
    }

    #endregion

    /// <summary>
    /// Returns true if all the timeseries are empty
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return Items.Max(var => var.Values.Count()) > 1;
      }
    }

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
      WaterPacket wc = Water as WaterPacket;

      //Log chemicals if the water is based on WaterPacket
      if (wc!=null)
      {
        if (LogAllChemicals)
          foreach (var c in wc.Chemicals.Keys)
          {
            TimestampSeries ts;
            if (!ChemicalsToLog.TryGetValue(c, out ts))
            {
              ts = CreateChemicalSeries(c);
            }
            ts.AddSiValue(End, wc.GetConcentration(c));
          }
        else
        {
          foreach (KeyValuePair<Chemical, TimestampSeries> ct in ChemicalsToLog)
          {
            ct.Value.AddSiValue(End, ((WaterPacket)Water).GetConcentration(ct.Key));
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
