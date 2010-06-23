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
  public class WaterBodyOutput:TimeSeriesGroup 
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


    #endregion

    #region Constructor
    public WaterBodyOutput(string ID)
    {
      TimespanSeries Outflow = new TimespanSeries();
      Outflow.Name = ID + ": Outflow";
      Outflow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Outflow);

      TimespanSeries Evaporation = new TimespanSeries();
      Evaporation.Name = ID + ": Evaporation";
      Evaporation.Unit = Outflow.Unit;
      Items.Add(Evaporation);

      TimespanSeries Sinks = new TimespanSeries();
      Sinks.Name = ID + ": Sinks";
      Sinks.Unit = Outflow.Unit;
      Items.Add(Sinks);

      TimespanSeries Sources = new TimespanSeries();
      Sources.Name = ID + ": Sources";
      Sources.Unit = Outflow.Unit;
      Items.Add(Sources);

      ChemicalsToLog = new Dictionary<Chemical,TimespanSeries>();
    }

    #endregion


    public Dictionary<Chemical,TimespanSeries> ChemicalsToLog { get; set; }

    public void LogChemicalConcentration(Chemical Chem)
    {
      TimespanSeries ts = new TimespanSeries();
      ts.Name = Chem.Name;
      ts.Unit = new HydroNumerics.Core.Unit("mol/m3", 1, 0);
      Items.Add(ts);
      ChemicalsToLog.Add(Chem,ts);
    }

  }
}
