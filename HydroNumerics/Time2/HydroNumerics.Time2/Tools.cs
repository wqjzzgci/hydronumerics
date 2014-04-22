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
    /// Returns the mean error
    /// </summary>
    /// <param name="Observations"></param>
    /// <param name="Simulations"></param>
    /// <param name="DeleteValue"></param>
    /// <returns></returns>
    public static double ME(IList<double> Observations, IList<double> Simulations, double DeleteValue)
    {
      int counter = 0;
      double ME = 0;
      for (int i = 0; i < Math.Min(Observations.Count, Simulations.Count); i++)
      {
        if (Observations[i] != DeleteValue & Simulations[i] != DeleteValue)
        {
          ME += Observations[i] - Simulations[i];
          counter++;
        }
      }
      return ME / counter;
    }

    /// <summary>
    /// Calculates the R-squared value. Coefficient of determination. http://en.wikipedia.org/wiki/Coefficient_of_determination
    /// </summary>
    /// <param name="ts1"></param>
    /// <param name="ts2"></param>
    /// <returns></returns>
    public static double R2(double[] ts1, double[] ts2)
    {
      double r2 = 0;
      double divisor = 0;
      double OMean = ts1.Average();

      for (int i = 0; i < ts1.Count(); i++)
      {
        r2 += Math.Pow(ts1[i] - ts2[i], 2);
        divisor += Math.Pow(ts1[i] - OMean, 2);
      }
      return 1.0 - r2 / divisor;
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


    public static void LimitTimeSeries(TimeStampSeries Data, DateTime Start, DateTime End)
    {
      for (int i = Data.Items.Count-1; i >= 0; i--)
      {
        if (Data.Items[i].Time < Start || Data.Items[i].Time > End)
        {
          Data.Items.RemoveAt(i);
        }
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

    public static FixedTimeStepSeries ChangeZoomLevel(FixedTimeStepSeries Data, TimeStepUnit NewZoomLevel, bool Accumulate)
    {
      FixedTimeStepSeries ToReturn = new FixedTimeStepSeries();
      ToReturn.DeleteValue = Data.DeleteValue;
      ToReturn.TimeStepSize = NewZoomLevel;

      List<double> newvalues = new List<double>();

      int localcount = 0;
      int counter = 0;
      switch (NewZoomLevel)
      {
        case TimeStepUnit.Year:
          {
            int currentyear = Data.StartTime.Year;
            newvalues.Add(0);
            foreach (var v in Data.values)
            {
              if (Data.GetTime(counter).Year == currentyear)
                newvalues[newvalues.Count-1] += v;
              else
              {
                currentyear++;
                if (!Accumulate)
                  newvalues[newvalues.Count - 1] /= localcount;
                localcount = 0;
                newvalues.Add(v);
              }
              localcount++;
              counter++;
            }
            if (!Accumulate)
              newvalues[newvalues.Count - 1] /= localcount;
          }
          break;
        case TimeStepUnit.Month:
          int currentmonth = Data.StartTime.Month;
            newvalues.Add(0);
            foreach (var v in Data.values)
            {
              if (Data.GetTime(counter).Month == currentmonth)
                newvalues[newvalues.Count - 1] += v;
              else
            {
              currentmonth = Data.GetTime(counter).Month;
              if (!Accumulate)
                newvalues[newvalues.Count - 1] /= localcount;
              localcount = 0;
              newvalues.Add(v);
            }
            localcount++;
            counter++;
            }
          if (!Accumulate)
            newvalues[newvalues.Count - 1] /= localcount;

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
      ToReturn.AddRange(Data.StartTime, newvalues);
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
