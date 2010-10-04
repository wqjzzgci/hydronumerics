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
    public WaterBodyOutput(string ID)
    { 
      TimespanSeries Outflow = new TimespanSeries();
      Outflow.Name = ID + ": Outflow";
      Outflow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Outflow);

      TimespanSeries Evaporation = new TimespanSeries();
      Evaporation.Name = ID + ": Evaporation";
      Evaporation.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Evaporation);

      TimespanSeries Sinks = new TimespanSeries();
      Sinks.Name = ID + ": Sinks";
      Sinks.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Sinks);

      TimespanSeries Sources = new TimespanSeries();
      Sources.Name = ID + ": Sources";
      Sources.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Sources);

      TimespanSeries Inflow = new TimespanSeries();
      Inflow.Name = ID + ": Inflow";
      Inflow.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Inflow);

      TimespanSeries Precip = new TimespanSeries();
      Precip.Name = ID + ": Precipitation";
      Precip.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Precip);

      TimespanSeries GWIn = new TimespanSeries();
      GWIn.Name = ID + ": Groundwater inflow";
      GWIn.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(GWIn);

      TimespanSeries GWout = new TimespanSeries();
      GWout.Name = ID + ": Groundwater outflow";
      GWout.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(GWout);

      TimestampSeries StoredVolume = new TimestampSeries();
      StoredVolume.Name = ID + ": Stored volume";
      StoredVolume.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeter);
      Items.Add(StoredVolume);

      ChemicalsToLog = new Dictionary<Chemical,TimespanSeries>();
    }

    #endregion

    [DataMember]
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
