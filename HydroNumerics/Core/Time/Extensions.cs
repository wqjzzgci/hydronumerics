using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroNumerics.Core.Time
{
  public static class Extensions
  {
    public static DateTime AddTimeStepUnit(this DateTime dt, TimeStepUnit tstep)
    {
      switch (tstep)
      {
        case TimeStepUnit.Year:
          return dt.AddYears(1);
        case TimeStepUnit.Month:
          return dt.AddMonths(1);
        case TimeStepUnit.Day:
          return dt.AddDays(1);
        case TimeStepUnit.Hour:
          return dt.AddHours(1);
        case TimeStepUnit.Minute:
          return dt.AddMinutes(1);
        case TimeStepUnit.Second:
          return dt.AddSeconds(1);
      }
      return dt;
    }

    public static DateTime SubstractTimeStepUnit(this DateTime dt, TimeStepUnit tstep)
    {
      switch (tstep)
      {
        case TimeStepUnit.Year:
          return dt.AddYears(-1);
        case TimeStepUnit.Month:
          return dt.AddMonths(-1);
        case TimeStepUnit.Day:
          return dt.AddDays(-1);
        case TimeStepUnit.Hour:
          return dt.AddHours(-1);
        case TimeStepUnit.Minute:
          return dt.AddMinutes(-1);
        case TimeStepUnit.Second:
          return dt.AddSeconds(-1);
      }
      return dt;
    }

  }
}
