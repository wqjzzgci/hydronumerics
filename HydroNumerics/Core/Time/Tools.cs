using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core.Time
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

      if (timespan.TotalSeconds < 60)
        return TimeStepUnit.Second;
      else if (timespan.TotalMinutes < 60)
        return TimeStepUnit.Minute;
      else if (timespan.TotalHours < 24)
        return TimeStepUnit.Hour;
      else if (timespan.TotalDays  <28)
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

    /// <summary>
    /// Aligns two series be returning only the common time values
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="tout1"></param>
    /// <param name="tout2"></param>
    public static void AlignSeries(TimeStampSeries t1, TimeStampSeries t2, out TimeStampValue[] tout1, out TimeStampValue[] tout2)
    {
      int t1count = 0;
      int t2count = 0;

      List<TimeStampValue> newT1values = new List<TimeStampValue>();
      List<TimeStampValue> newT2values = new List<TimeStampValue>();

      if (t1.Count != 0 & t2.Count != 0)
      {
        while (t1count < t1.Count && t1.Items[t1count].Time < t2.StartTime)
          t1count++;

        while (t2count < t2.Count && t2.Items[t2count].Time < t1.StartTime)
          t2count++;

        for (int i = t1count; i < t1.Count; i++)
        {
          while (t2count < t2.Count - 1 & t2.Items[t2count].Time < t1.Items[i].Time)
            t2count++;

          if (t1.Items[i].Time == t2.Items[t2count].Time)
          {

            newT1values.Add(t1.Items[i]);
            newT2values.Add(t2.Items[t2count]);
          }
        }
      }
      tout1 = newT1values.ToArray();
      tout2 = newT2values.ToArray();
    }

    /// <summary>
    /// First aligns the series and then combines
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="Combiner"></param>
    /// <returns></returns>
    public static TimeStampSeries CombineSeries(TimeStampSeries t1, TimeStampSeries t2, Func<double?, double?, double> Combiner)
    {

      TimeStampValue[] ta1;
      TimeStampValue[] ta2;
      AlignSeries(t1, t2, out ta1, out ta2);

      List<TimeStampValue> newvalues = new List<TimeStampValue>();

      for (int i = 0; i < ta1.Count(); i++)
      {
        newvalues.Add(new TimeStampValue(ta1[i].Time, Combiner(ta1[i].Value, ta2[i].Value)));

      }
      return new TimeStampSeries(newvalues);
    }

    /// <summary>
    /// Changes the zoomlevel of a timespanseries
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="NewZoomLevel"></param>
    /// <param name="Accumulate"></param>
    /// <returns></returns>
    public static IEnumerable<TimeSpanValue> ChangeZoomLevel(TimeSpanSeries Data, TimeStepUnit NewZoomLevel, bool Accumulate)
    {
      if (Data.TimeStepSize <= NewZoomLevel)
        return null;

      List<TimeSpanValue> ToReturn = new List<TimeSpanValue>();

      DateTime start = Data.StartTime;
      DateTime end = Data.EndTime;

      TimeSpanValue CurrentValue = new TimeSpanValue(GetTimeOfFirstTimeStep(start, NewZoomLevel), GetTimeOfFirstTimeStep(start, NewZoomLevel).AddTimeStepUnit(NewZoomLevel), 0);
      double currentcount = 0;
      foreach (var v in Data.Items.Where(dv => dv.Value != Data.DeleteValue))
      {
        if (CurrentValue.StartTime <= v.StartTime & v.StartTime<= CurrentValue.EndTime)
        {
          if (CurrentValue.EndTime >= v.EndTime) //We are still within the timespan
          {
            CurrentValue.Value += v.Value;
            currentcount++;
          }
          else //We exceed the timespan
          {
            double outsidefraction =(v.EndTime.Subtract(CurrentValue.EndTime).TotalDays / v.EndTime.Subtract(v.StartTime).TotalDays);
              CurrentValue.Value += v.Value*(1 - outsidefraction);
              currentcount += 1 - outsidefraction;

            if (!Accumulate & currentcount != 0)
              CurrentValue.Value /= currentcount;
            ToReturn.Add(CurrentValue);
            CurrentValue = new TimeSpanValue(CurrentValue.EndTime, CurrentValue.EndTime.AddTimeStepUnit(NewZoomLevel), v.Value*outsidefraction);
            currentcount = outsidefraction;
          }
        }
        else
        {
          if (!Accumulate & currentcount != 0)
            CurrentValue.Value /= currentcount;
          ToReturn.Add(CurrentValue);
          CurrentValue = new TimeSpanValue(GetTimeOfFirstTimeStep(v.StartTime, NewZoomLevel), GetTimeOfFirstTimeStep(v.StartTime, NewZoomLevel).AddTimeStepUnit(NewZoomLevel), v.Value);
          currentcount = 1;
        }
      }
      if (!Accumulate & currentcount != 0)
        CurrentValue.Value /= currentcount;
      ToReturn.Add(CurrentValue);

      return ToReturn;
    }


    public static FixedTimeStepSeries Substract(FixedTimeStepSeries ts1, FixedTimeStepSeries ts2)
    {
      FixedTimeStepSeries ToReturn = new FixedTimeStepSeries();
      ToReturn.TimeStepSize = ts1.TimeStepSize;
      ToReturn.DeleteValue = ts1.DeleteValue;
      ToReturn.StartTime = ts1.StartTime;
      DateTime End = ts1.EndTime;

      if (ts1.StartTime > ts2.StartTime)
        ToReturn.StartTime =ts2.StartTime;
      if(ts1.EndTime > ts2.EndTime)
        End = ts2.EndTime;

      var val1 = ts1.GetValues(ToReturn.StartTime, End);
      var val2 = ts2.GetValues(ToReturn.StartTime, End);

      for (int i = 0;i<val1.Count();i++)
      {
        if(val1[i]==ts1.DeleteValue || val2[i]== ts2.DeleteValue)
          ToReturn.Add(ToReturn.DeleteValue);
        else
          ToReturn.Add(val1[i]-val2[i]);
      }
      return ToReturn;
    }


    /// <summary>
    /// Gets a fixedtimestepseries at another zoom level.
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="NewZoomLevel"></param>
    /// <param name="Accumulate"></param>
    /// <returns></returns>
    public static FixedTimeStepSeries ChangeZoomLevel(FixedTimeStepSeries Data, TimeStepUnit NewZoomLevel, bool Accumulate)
    {

      if (Data.TimeStepSize <= NewZoomLevel)
        return null;

      FixedTimeStepSeries ToReturn = new FixedTimeStepSeries();
      ToReturn.DeleteValue = Data.DeleteValue;
      ToReturn.TimeStepSize = NewZoomLevel;
      ToReturn.StartTime= GetTimeOfFirstTimeStep(Data.StartTime, NewZoomLevel);
      ToReturn.OffSetStartTime = Data.OffSetStartTime;
      ToReturn.OffSetEndTime = Data.OffSetEndTime;


      List<double> newvalues = new List<double>();

      int localcount = 0;
      int counter = 0;
      switch (NewZoomLevel)
      {
        case TimeStepUnit.Year:
          {
            int currentyear = -1;
            foreach (var v in Data.Items)
            {
              if (Data.GetTime(counter).Year == currentyear)
              {
                if (v != Data.DeleteValue)
                  newvalues[newvalues.Count - 1] += v;
              }
              else
              {
                currentyear = Data.GetTime(counter).Year;
                if (!Accumulate & newvalues.Count>0)
                  newvalues[newvalues.Count - 1] /= localcount;
                localcount = 0;
                newvalues.Add(v);
              }
              localcount++;
              counter++;
            }
            if (localcount != 12) //only use complete years
              newvalues.RemoveAt(newvalues.Count - 1);
            else
            {
              if (!Accumulate)
                newvalues[newvalues.Count - 1] /= localcount;
            }
          }
          break;
        case TimeStepUnit.Month:
          int currentmonth = Data.StartTime.Month;
            newvalues.Add(0);
            foreach (var v in Data.Items)
            {
              if (Data.GetTime(counter).Month == currentmonth)
              {
                if (v != Data.DeleteValue)
                  newvalues[newvalues.Count - 1] += v;
              }
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
          int currentday = Data.StartTime.Day;
            newvalues.Add(0);
            foreach (var v in Data.Items)
            {
              if (Data.GetTime(counter).Day == currentday)
              {
                if (v != Data.DeleteValue)
                  newvalues[newvalues.Count - 1] += v;
              }
              else
              {
                currentday = Data.GetTime(counter).Day;
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
        case TimeStepUnit.Hour:
          int currenthour = Data.StartTime.Hour;
            newvalues.Add(0);
            foreach (var v in Data.Items)
            {
              if (Data.GetTime(counter).Hour == currenthour)
              {
                if (v != Data.DeleteValue)
                  newvalues[newvalues.Count - 1] += v;
              }
              else
              {
                currenthour = Data.GetTime(counter).Hour;
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
        case TimeStepUnit.Minute:
          int currentminute = Data.StartTime.Minute;
            newvalues.Add(0);
            foreach (var v in Data.Items.Where(vv => vv != Data.DeleteValue))
            {
              if (Data.GetTime(counter).Minute == currentminute)
              {
                newvalues[newvalues.Count - 1] += v;
              }
              else
              {
                currentminute = Data.GetTime(counter).Minute;
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
        case TimeStepUnit.Second:
          break;
        case TimeStepUnit.None:
          break;
        default:
          break;
      }
      ToReturn.AddRange(newvalues);

      return ToReturn;
    }


    public static DateTime GetTimeOfFirstTimeStep(DateTime Start, TimeStepUnit tsu)
    {
      switch (tsu)
      {
        case TimeStepUnit.Year:
          return new DateTime(Start.Year, 1, 1);
        case TimeStepUnit.Month:
          return new DateTime(Start.Year, Start.Month, 1);
        case TimeStepUnit.Day:
          return new DateTime(Start.Year, Start.Month, Start.Day);
        case TimeStepUnit.Hour:
          return new DateTime(Start.Year, Start.Month, Start.Day, Start.Hour, 0, 0);
        case TimeStepUnit.Minute:
          return new DateTime(Start.Year, Start.Month, Start.Day, Start.Hour, Start.Minute, 0);
        case TimeStepUnit.Second:
          return new DateTime(Start.Year, Start.Month, Start.Day, Start.Hour, Start.Minute, Start.Second);
      }
      return Start;
    }


    /// <summary>
    /// Returns the lowest possible zoomlevel
    /// </summary>
    /// <param name="tsv"></param>
    /// <returns></returns>
    public static TimeStepUnit GetLowestZoomLevel(TimeSpan tsv)
    {
      if (tsv.TotalSeconds <= 60)
        return TimeStepUnit.Minute;
      if (tsv.TotalMinutes <= 60)
        return TimeStepUnit.Hour;
      if (tsv.TotalHours <= 24)
        return TimeStepUnit.Day;
      if (tsv.TotalDays <= 28)
        return TimeStepUnit.Month;
      if (tsv.TotalDays <= 365)
        return TimeStepUnit.Year;
      return TimeStepUnit.None;
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
