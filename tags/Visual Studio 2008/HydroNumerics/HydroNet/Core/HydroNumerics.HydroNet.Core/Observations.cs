using System;
using System.Runtime.Serialization;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class Observations:TimeSeriesGroup
  {
    [DataMember]
    public Dictionary<Chemical, TimestampSeries> ChemicalConcentrations { get; private set; }

    [DataMember]
    public TimestampSeries Discharge { get; private set; }

    [DataMember]
    public TimestampSeries WaterLevel { get; private set; }

    public Observations(string ID)
      : base()
    {
      Discharge = new TimestampSeries();
      Discharge.Name = ID + ": Discharge";
      Discharge.Unit = UnitFactory.Instance.GetUnit(NamedUnits.cubicmeterpersecond);
      Items.Add(Discharge);

      WaterLevel = new TimestampSeries();
      WaterLevel.Name = ID + ": WaterLevel";
      WaterLevel.Unit = UnitFactory.Instance.GetUnit(NamedUnits.meter);
      Items.Add(WaterLevel);

      ChemicalConcentrations = new Dictionary<Chemical, TimestampSeries>();
    }


    /// <summary>
    /// Creates a time series for the chemical
    /// </summary>
    /// <param name="Chem"></param>
    /// <returns></returns>
    public TimestampSeries AddChemicalTimeSeries(Chemical Chem)
    {
      TimestampSeries ts = new TimestampSeries();
      ts.Name = Chem.Name;
      ts.Unit = UnitFactory.Instance.GetUnit(NamedUnits.molespercubicmeter);
      Items.Add(ts);
      ChemicalConcentrations.Add(Chem, ts);
      return ts;
    }

  }
}
