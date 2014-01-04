using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public class DaisyTimeSeries
  {

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public float[] DataSeries { get; private set; }


    public DaisyTimeSeries(DateTime Start, float[] DataSeries)
    {
      this.Start = Start;
      this.End = Start.AddDays(DataSeries.Count()-2);
      this.DataSeries = DataSeries.Take(DataSeries.Count() - 3).ToArray();
      //What to do with the sum and the negative data
      float sum = DataSeries[DataSeries.Count() - 2];
      float NegativeValues = DataSeries[DataSeries.Count() - 1];
    }

    public bool TryCombine(DaisyTimeSeries OtherTs)
    {
      if (Start == OtherTs.End)
      {
        Start = OtherTs.Start;
        var temp = OtherTs.DataSeries.ToList();
        temp.AddRange(DataSeries);
        DataSeries = temp.ToArray();
        return true;
      }
      else if (OtherTs.Start == End)
      {
        End = OtherTs.End;
        var temp = DataSeries.ToList();
        temp.AddRange(OtherTs.DataSeries);
        DataSeries = temp.ToArray();
        return true;
      }
      else return false;
    }
  }
}
