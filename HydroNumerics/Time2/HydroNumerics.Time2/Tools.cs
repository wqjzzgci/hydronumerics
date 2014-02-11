using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Time2
{
  public class TSTools
  {


        /// <summary>
    /// Returns the appropriate time step for a given time period
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public static TimeStepUnit GetTimeStep(TimeSpanValue value)
    {
      return GetTimeStep(value.StartTime, value.EndTime);
    }

    /// <summary>
    /// Returns the appropriate time step for a given time period
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public static TimeStepUnit GetTimeStep(DateTime Start, DateTime End)
    {
      var timespan = End.Subtract(Start);

      if (timespan.TotalSeconds == 1)
        return TimeStepUnit.Second;
      else if (timespan.TotalMinutes == 1)
        return TimeStepUnit.Minute;
      else if (timespan.TotalHours == 1)
        return TimeStepUnit.Hour;
      else if (timespan.TotalDays == 1)
        return TimeStepUnit.Day;
      else if (28 <= timespan.TotalDays & timespan.TotalDays <= 31)
        return TimeStepUnit.Month;
      else if (364 <= timespan.TotalDays & timespan.TotalDays <= 367)
        return TimeStepUnit.Year;
      else
        return TimeStepUnit.None;
    }

    public static DateTime GetNextTime(DateTime Start, TimeStepUnit timespan)
    {
      switch (timespan)
      {
        case TimeStepUnit.Year:
          return Start.AddYears(1);
        case TimeStepUnit.Month:
          return Start.AddMonths(1);
        case TimeStepUnit.Day:
          return Start.AddDays(1);
        case TimeStepUnit.Hour:
          return Start.AddHours(1);
        case TimeStepUnit.Minute:
          return Start.AddMinutes(1);
        case TimeStepUnit.Second:
          return Start.AddSeconds(1);
        default:
          throw new Exception("TimeStepUnit not set");
      }

    }

    public static IEnumerable<TimeSpanValue> ChangeZoomLevel(TimeSpanSeries Data, TimeStepUnit NewZoomLevel, bool Accumulate)
    {
      List<TimeSpanValue> ToReturn = new List<TimeSpanValue>();

      DateTime start = Data.StartTime;
      DateTime end = Data.EndTime;

      switch (NewZoomLevel)
      {
        case TimeStepUnit.Year:

          break;
        case TimeStepUnit.Month:
          int currentmonth = start.Month;
          int localcount = 0;
          ToReturn.Add(new TimeSpanValue(new DateTime(start.Year, start.Month, 1), new DateTime(start.Year, start.Month, 1).AddMonths(1), 0));
          foreach (var v in Data.Items.Where(dv=>dv.Value!= Data.DeleteValue ))
          {
            if (v.StartTime.Month == currentmonth)
              ToReturn.Last().Value += v.Value;
            else
            {
              currentmonth = v.StartTime.Month;
              if(!Accumulate)
                ToReturn.Last().Value /= localcount;
              localcount = 0;
              ToReturn.Add(new TimeSpanValue(new DateTime(v.StartTime.Year, v.StartTime.Month, 1), new DateTime(v.StartTime.Year, v.StartTime.Month, 1).AddMonths(1), v.Value));
            }
            localcount++;
          }
          if (!Accumulate)
            ToReturn.Last().Value /= localcount;

          break;
        case TimeStepUnit.Day:
          break;
        case TimeStepUnit.Hour:
          break;
        case TimeStepUnit.Minute:
          break;
        case TimeStepUnit.Second:
          break;
        case TimeStepUnit.None:
          break;
        default:
          break;
      }


      return ToReturn;
    }

    public static TimeStampSeries ChangeZoomLevel(TimeStampSeries Data, TimeStepUnit NewZoomLevel, bool Accumulate)
    {
      TimeStampSeries ToReturn = new TimeStampSeries();
      ToReturn.DeleteValue = Data.DeleteValue;
      ToReturn.Name = Data.Name;
      ToReturn.ID = Data.ID;
      ToReturn.TimeStepSize = NewZoomLevel;


      DateTime start = Data.StartTime;
      DateTime end = Data.EndTime;
      int localcount = 0;
      switch (NewZoomLevel)
      {
        case TimeStepUnit.Year:
          {
            int currentyear = start.Year;
            ToReturn.Items.Add(new TimeStampValue(new DateTime(start.Year, 1, 1), 0));
            foreach (var v in Data.Items.Where(dv => dv.Value != Data.DeleteValue))
            {
              if (v.Time.Year == currentyear)
                ToReturn.Items.Last().Value += v.Value;
              else
              {
                currentyear = v.Time.Year;
                if (!Accumulate)
                  ToReturn.Items.Last().Value /= localcount;
                localcount = 0;
                ToReturn.Items.Add(new TimeStampValue(new DateTime(v.Time.Year, 1, 1), v.Value));
              }
              localcount++;
            }
            if (!Accumulate)
              ToReturn.Items.Last().Value /= localcount;
          }
          break;
        case TimeStepUnit.Month:
          int currentmonth = start.Month;
          ToReturn.Items.Add(new TimeStampValue(new DateTime(start.Year, start.Month, 1), 0));
          foreach (var v in Data.Items.Where(dv => dv.Value != Data.DeleteValue))
          {
            if (v.Time.Month == currentmonth)
              ToReturn.Items.Last().Value += v.Value;
            else
            {
              currentmonth = v.Time.Month;
              if (!Accumulate)
                ToReturn.Items.Last().Value /= localcount;
              localcount = 0;
              ToReturn.Items.Add(new TimeStampValue(new DateTime(v.Time.Year, v.Time.Month, 1), v.Value));
            }
            localcount++;
          }
          if (!Accumulate)
            ToReturn.Items.Last().Value /= localcount;

          break;
        case TimeStepUnit.Day:
          break;
        case TimeStepUnit.Hour:
          break;
        case TimeStepUnit.Minute:
          break;
        case TimeStepUnit.Second:
          break;
        case TimeStepUnit.None:
          break;
        default:
          break;
      }


      return ToReturn;
    }


  }
}
