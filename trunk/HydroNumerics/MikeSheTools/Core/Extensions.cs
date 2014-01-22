using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Core
{
  public static class Extensions
  {

    public static Time2.TimeStampSeries GetTimeSpanSeries(this DFS.DFS0 data, int item)
    {
      List<Time2.TimeStampValue> values = new List<Time2.TimeStampValue>();

      for (int i = 0; i < data.NumberOfTimeSteps; i++)
      {
        values.Add(new Time2.TimeStampValue(data.TimeSteps[i], data.GetData(i, item)));
      }
      Time2.TimeStampSeries ToReturn = new Time2.TimeStampSeries(values);
      ToReturn.DeleteValue = data.DeleteValue;
      ToReturn.Name = data.Items[item-1].Name;
      ToReturn.ID = item;
      return ToReturn;
    }
  }
}
