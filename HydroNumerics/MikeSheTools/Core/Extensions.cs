using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core.Time;

namespace HydroNumerics.MikeSheTools.Core
{
  public static class Extensions
  {

    public static TimeStampSeries GetTimeSpanSeries(this DFS.DFS0 data, int item)
    {
      List<TimeStampValue> values = new List<TimeStampValue>();

      for (int i = 0; i < data.NumberOfTimeSteps; i++)
      {
        values.Add(new TimeStampValue(data.TimeSteps[i], data.GetData(i, item)));
      }
      TimeStampSeries ToReturn = new TimeStampSeries(values);
      ToReturn.DeleteValue = data.DeleteValue;
      ToReturn.Name = data.Items[item-1].Name;
      ToReturn.ID = item;
      return ToReturn;
    }
  }
}
