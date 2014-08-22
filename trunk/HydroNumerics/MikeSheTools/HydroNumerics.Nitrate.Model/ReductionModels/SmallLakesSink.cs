using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Core.Time;


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
        foreach (var s in Configuration.Element("YearFactors").Elements("YearFactor"))
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
        Reduction.Add(c.ID, reduction/12.0); //Unit: form n/year to N/month
      }
    }

    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      double red = 0;
      double rate;
      if (Reduction.TryGetValue(c.ID, out rate) & c.M11Flow != null)
      {
        double NormalizedMonthlyFlow = 0;
        double yearlyinflow = c.NetInflow.GetTs(TimeStepUnit.Year).GetValue(CurrentTime);
        double monthlyflow=c.NetInflow.GetTs(TimeStepUnit.Month).GetValue(CurrentTime);
        if (yearlyinflow>0 & monthlyflow>0)
          NormalizedMonthlyFlow =  monthlyflow/yearlyinflow;
        red = Math.Min(rate * NormalizedMonthlyFlow * YearFactors[CurrentTime.Year], CurrentMass) / (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0);
      }
      return red * MultiplicationPar + AdditionPar;
    }
  }
}
