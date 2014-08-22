using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HydroNumerics.Core.Time
{
  public class TimeSeriesReducer
  {

    public static List<TimeStampValue> ReduceByRounding(List<TimeStampValue> TimeSeries, int NumberOfDecimals)
    {
      List<TimeStampValue> NewList = new List<TimeStampValue>();
      NewList.Add(new TimeStampValue(TimeSeries[0].Time, Math.Round(TimeSeries[0].Value, NumberOfDecimals)));

      int i = 1;

      while (i < TimeSeries.Count-1)
      {
        while (NewList.Last().Value == Math.Round(TimeSeries[i].Value, NumberOfDecimals) & i < TimeSeries.Count-1)
          i++;
        if (i < TimeSeries.Count-1)
          NewList.Add(new TimeStampValue(TimeSeries[i-1].Time, Math.Round(TimeSeries[i-1].Value, NumberOfDecimals)));
        NewList.Add(new TimeStampValue(TimeSeries[i].Time, Math.Round(TimeSeries[i].Value, NumberOfDecimals)));
      }
      
      return NewList;
    }
  }
}
