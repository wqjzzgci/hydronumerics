using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;


namespace HydroNumerics.Nitrate.Model
{
  public class SmallLakesSink : BaseModel, ISink
  {
    private class SoilType
    {
      public double IntensiveRate { get; set; }
      public double IntermediateRate { get; set; }
      public double ExtensiveRate { get; set; }
      public string Name { get; set; }
    }

    private Dictionary<string, SoilType> soils = new Dictionary<string, SoilType>();
    private Dictionary<int, double> YearFactors;
    private Dictionary<int, double> Reduction;

    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Update)
      {
        foreach (var s in Configuration.Element("SoilTypes").Elements("SoilType"))
        {
          SoilType st = new SoilType();
          st.ExtensiveRate = s.SafeParseDouble("ExtensiveRate")??0;
          st.IntermediateRate = s.SafeParseDouble("IntermediateRate") ?? 0;
          st.IntensiveRate = s.SafeParseDouble("IntensiveRate") ?? 0;
          st.Name = s.SafeParseString("Name");
          soils.Add(s.SafeParseString("Name").ToLower(),st);
        }

        YearFactors = new Dictionary<int, double>();
        foreach (var s in Configuration.Element("YearFactors").Elements("YearFactors"))
        {
          YearFactors.Add(s.SafeParseInt("Year").Value, s.SafeParseDouble("Factor").Value);
        }
      }
    }






    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      Reduction = new Dictionary<int, double>();
      foreach (var c in Catchments)
      {

        double reduction = 0;

        foreach (var l in c.Lakes)
        {
          double rate=0;
          switch (l.DegreeOfCultivation)
          {
            case CultivationClass.Low:
              rate= soils[l.SoilType.ToLower()].ExtensiveRate;
              break;
            case CultivationClass.Intermediate:
              rate= soils[l.SoilType.ToLower()].IntermediateRate;
              break;
            case CultivationClass.High:
              rate= soils[l.SoilType.ToLower()].IntensiveRate;
              break;
            default:
              break;
          }
          reduction += l.Geometry.GetArea()/10000.0 * rate;
        }
        Reduction.Add(c.ID, reduction);
      }
    }

    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      double rate;
      if(Reduction.TryGetValue(c.ID, out rate) & c.M11Flow!=null)
      {
        double upstreammonthly = c.UpstreamConnections.Where(ca => ca.M11Flow != null).Sum(ca => ca.M11Flow.GetTs(Time2.TimeStepUnit.Month).GetValue(CurrentTime));
        double upstreamyearly = c.UpstreamConnections.Where(ca => ca.M11Flow != null).Sum(ca => ca.M11Flow.GetTs(Time2.TimeStepUnit.Year).GetValue(CurrentTime));
        double MonthlyFlow = c.M11Flow.GetTs(Time2.TimeStepUnit.Month).GetValue(CurrentTime) -upstreammonthly;
        double YearlyFlow = c.M11Flow.GetTs(Time2.TimeStepUnit.Year).GetValue(CurrentTime) -upstreamyearly;
        double NormalizedMonthlyFlow = MonthlyFlow / YearlyFlow;

        return rate * NormalizedMonthlyFlow / (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0);
      }
      else
        return 0;


    }

  }
}
