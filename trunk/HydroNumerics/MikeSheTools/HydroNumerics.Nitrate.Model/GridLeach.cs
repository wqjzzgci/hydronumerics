using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  /// <summary>
  /// This class holds the leaching from daisy i one GridBlock
  /// </summary>
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

    /// <summary>
    /// Adds another year of data
    /// </summary>
    /// <param name="start"></param>
    /// <param name="data"></param>
    public void AddYear(DateTime start, float[] data)
    {
      YearlyData yd = new YearlyData(start, data);
      Data.Add(yd);

      //Make sure the yearly data are properly sorted.
//      Data.Sort(new Comparison<YearlyData>((f1, f2) => f1.Start.CompareTo(f2.Start)));
    }

    /// <summary>
    /// Reduce all data to monthly timesteps. To reduce memory use
    /// </summary>
    public void ReduceToMonhlyTimeSteps()
    {
        Parallel.ForEach(Data, new ParallelOptions() { MaxDegreeOfParallelism = 7 },
        (p) =>
        {
          p.ReduceToMonthlyTimeSteps();
        });
    }

    /// <summary>
    /// Gets the value at a point in time
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public float GetValue(DateTime Time)
    {
      int year = Time.Year - Start.Year;
      if (year < 0) return 0f; //Very careful here. This should be removed

      return Data[year].GetValue(Time);
    }

    /// <summary>
    /// Gets the start of the timeseries
    /// </summary>
    public DateTime Start
    {
      get
      {
        return Data.First().Start;
      }
    }

    /// <summary>
    /// Gets the end of the timeseries
    /// </summary>
    public DateTime End
    {
      get
      {
        return Data.Last().End;
      }
    }
  }
}
