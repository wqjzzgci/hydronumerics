using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public class GridLeach
  {

    public GridLeach(int GridId)
    {
      this.GridID = GridId;
      Data = new List<YearlyData>();
    }

    public int GridID { get; private set; }
    public int DMIGridID { get; set; }
    public int LandUseID { get; set; }
    public int SoilID { get; set; }

    public List<YearlyData> Data { get; private set; }

    public void AddYear(DateTime start, float[] data)
    {
      YearlyData yd = new YearlyData(start, data);
      Data.Add(yd);

      //Make sure the yearly data are properly sorted.
      Data.Sort(new Comparison<YearlyData>((f1, f2) => f1.Start.CompareTo(f2.Start)));
    }

    public void ReduceToMonhlyTimeSteps()
    {
      foreach (var d in Data)
        d.ReduceToMonthlyTimeSteps();
    }


    public float GetValue(DateTime Time)
    {
      int year = Time.Year - Start.Year;
      return Data[year].GetValue(Time);
    }

    public DateTime Start
    {
      get
      {
        return Data.First().Start;
      }
    }
    public DateTime End
    {
      get
      {
        return Data.Last().End;
      }
    }

  }
}
