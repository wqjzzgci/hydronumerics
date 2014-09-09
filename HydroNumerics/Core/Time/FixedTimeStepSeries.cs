using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core.Time
{
  public class FixedTimeStepSeries:BaseTimeSeries<double>
  {


    public FixedTimeStepSeries():base(new Func<double, double>(d => d))
    {
    }

    /// <summary>
    /// Creates a fixed time step series with the lowest possible timestep 
    /// </summary>
    /// <param name="ts"></param>
    public FixedTimeStepSeries(TimeSpanSeries ts):this()
    {
      var maxtimestep = ts.Items.Max(tse => tse.Duration);
      this.TimeStepSize = TSTools.GetLowestZoomLevel(maxtimestep);
      this.StartTime = TSTools.GetTimeOfFirstTimeStep(ts.StartTime, TimeStepSize);
      this.OffSetStartTime = ts.StartTime;
      OffSetEndTime = ts.EndTime;
      DateTime NextEnd = ts.StartTime;

      foreach(var v in ts.Items)
      {
        if (v.EndTime <= NextEnd)
        {
          if (Items.Count == 0)
            Items.Add(0);
          Items[Items.Count - 1] += v.Value;
        }
        else
        {
          Items.Add(DeleteValue);
          while (v.EndTime > (NextEnd = NextEnd.AddTimeStepUnit(TimeStepSize)))
            Items.Add(DeleteValue);
          Items[Count - 1] = v.Value;
        }
      }
    }


    /// <summary>
    /// Adds a value to the end of the series
    /// </summary>
    /// <param name="Value"></param>
    public void Add(double Value)
    {
      Items.Add(Value);
    }


    /// <summary>
    /// Adds a range of data to the series. If necessary delete values will be added. Existing values at the same time step will be overwritten.
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="values"></param>
    public void AddRange(DateTime Start, IEnumerable<double> values)
    {
      if (StartTime == DateTime.MinValue)
      {
        StartTime = Start;
        AddRange(values);
      }
      else
      {
        int nextindex = GetIndex(Start);

        if (nextindex > Count)
        {
          for (int i = Count; i < nextindex; i++)
            Items.Add(DeleteValue);
          AddRange(values);
        }
        else if (nextindex < 0)
        {
          var previous = Values.ToArray();
          var previoustime = StartTime;
          Items.Clear();
          StartTime = DateTime.MinValue;
          AddRange(Start, values);
          AddRange(previoustime, previous);
        }
        else
        {
          foreach (var v in values)
          {
            if (nextindex < Count)
              Items[nextindex] = v;
            else
              Items.Add(v);
            nextindex++;
          }
        }
      }
      RaisePropertyChanged("Count");
    }



    /// <summary>
    /// Gets the time at a certain index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public DateTime GetTime(int index)
    {
      switch (TimeStepSize)
      {
        case TimeStepUnit.Year:
          return StartTime.AddYears(TimeStepMultiplier * index);
        case TimeStepUnit.Month:
          return StartTime.AddMonths(TimeStepMultiplier * index);
        case TimeStepUnit.Day:
          return StartTime.AddDays(TimeStepMultiplier * index);
        case TimeStepUnit.Hour:
          return StartTime.AddHours(TimeStepMultiplier * index);
        case TimeStepUnit.Minute:
          return StartTime.AddMinutes(TimeStepMultiplier * index);
        case TimeStepUnit.Second:
          return StartTime.AddSeconds(TimeStepMultiplier * index);
        case TimeStepUnit.None:
        default:
          return DateTime.MinValue;
      }
    }


    /// <summary>
    /// Gets the time series as TimeStampValues
    /// </summary>
    public IEnumerable<TimeStampValue> TimeStampValues
    {
      get
      {
        for (int i = 0; i < Count; i++)
          yield return new TimeStampValue(GetTime(i), Items[i]);
      }
    }

    /// <summary>
    /// Enumerates the timestep
    /// </summary>
    public IEnumerable<DateTime> TimeSteps
    {
      get
      {
        for (int i = 0; i < Count; i++)
          yield return GetTime(i);
      }
    }

    /// <summary>
    /// Gets the time series at TimeSpanValues. Removes delete values
    /// </summary>
    public IEnumerable<TimeSpanValue> TimeSpanValues
    {
      get
      {
        if (Items[0] != DeleteValue)
        {
          if(Count==1)
            yield return new TimeSpanValue(OffSetStartTime, OffSetEndTime, Items[0]);
          else
            yield return new TimeSpanValue(OffSetStartTime, GetTime(0), Items[0]);
        }
        for (int i = 1; i < Count - 1; i++)
        {
          if (Items[i] != DeleteValue)
            yield return new TimeSpanValue(GetTime(i - 1), GetTime(i), Items[i]);
        }
        if (Items.Count()>1 && Items[Count - 1] != DeleteValue)
          yield return new TimeSpanValue(GetTime(Count - 2), OffSetEndTime, Items[Count - 1]);
      }
    }

#region Stats

    public double? ME(FixedTimeStepSeries Other)
    {
      double[] val1;
      double[] val2;
      AlignRemoveDeletevalues(Other, out val1, out val2);
      int c = val1.Count();
      if (c > 0)
      {
        double me = 0;
        for (int i = 0; i < c; i++)
        {
          me += val1[i] - val2[i];
        }
        return me/c;
      }
      return null;
    }

    public int CommonCount(FixedTimeStepSeries Other)
    {
      double[] val1;
      double[] val2;
      AlignRemoveDeletevalues(Other, out val1, out val2);
      return val1.Count();

    }

    public double? MAE(FixedTimeStepSeries Other)
    {
      double[] val1;
      double[] val2;
      AlignRemoveDeletevalues(Other, out val1, out val2);
      int c = val1.Count();
      if (c > 0)
      {
        double mae = 0;
        for (int i = 0; i < c; i++)
        {
          mae += Math.Abs(val1[i] - val2[i]);
        }
        return mae / c;
      }
      return null;
    }

    /// <summary>
    /// Returns the RMSE value
    /// </summary>
    /// <param name="Other"></param>
    /// <returns></returns>
    public double? RMSE(FixedTimeStepSeries Other)
    {
      double[] val1;
      double[] val2;
      AlignRemoveDeletevalues(Other, out val1, out val2);
      int c = val1.Count();
      if (c > 0)
      {
        double me = 0;
        for (int i = 0; i < c; i++)
        {
          me += Math.Pow(val1[i] - val2[i],2);
        }
        return Math.Pow(me / c,0.5);

      }
      return null;
    }

    /// <summary>
    /// Returns the R-squared value
    /// </summary>
    /// <param name="Other"></param>
    /// <returns></returns>
    public double? R2(FixedTimeStepSeries Other)
    {
      double[] val1;
      double[] val2;
      AlignRemoveDeletevalues(Other, out val1, out val2);
      int c = val1.Count();
      if (c > 0)
        return TSTools.R2(val1, val2);
      return null;
    }


    /// <summary>
    /// Returns the FBAL-value
    /// </summary>
    /// <param name="Other"></param>
    /// <returns></returns>
    public double? FBAL(FixedTimeStepSeries Other)
    {
      double[] val1;
      double[] val2;
      AlignRemoveDeletevalues(Other, out val1, out val2);
      int c = val1.Count();
      if (c > 0)
        return (val1.Average() - val2.Average()) / val1.Average();
      return null;
    }


    public void AlignRemoveDeletevalues(FixedTimeStepSeries Other, out double[] val1, out double[] val2)
    {
      List<double> var1 = new List<double>();
      List<double> var2 = new List<double>();
      if (Other.TimeStepSize == TimeStepSize & Other.TimeStepMultiplier == TimeStepMultiplier)
      {
        if (Other.StartTime == StartTime)
        {
          for (int i = 0; i < Math.Min(this.Count, Other.Count); i++)
          {
            if (Items[i] != DeleteValue & Other.Items[i] != Other.DeleteValue)
            {
              var1.Add(Items[i]);
              var2.Add(Other.Items[i]);
            }
          }
        }
      }

      val1 = var1.ToArray();
      val2 = var2.ToArray();
    }
#endregion


    /// <summary>
    /// Gets the index for a certain time. Note that indeces can be negative and also exceed the length of the timeseries
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public int GetIndex(DateTime Time)
    {

      //Should take account of the timestepmultiplier!!!!
      switch (TimeStepSize)
      {
        case TimeStepUnit.Year:
          return Time.Year - StartTime.Year;
        case TimeStepUnit.Month:
          return Time.Month - StartTime.Month + (Time.Year - StartTime.Year) * 12;
        case TimeStepUnit.Day:
          return (int)Math.Floor(Time.Subtract(StartTime).TotalDays);
        case TimeStepUnit.Hour:
          return (int)Math.Floor(Time.Subtract(StartTime).TotalHours);
        case TimeStepUnit.Minute:
          return (int)Math.Floor(Time.Subtract(StartTime).TotalMinutes);
        case TimeStepUnit.Second:
          return (int)Math.Floor(Time.Subtract(StartTime).TotalSeconds);
        case TimeStepUnit.None:
        default:
          return -1;
      }
    }

    /// <summary>
    /// Gets the values closest to time Time. Returns Deletevalue if time is before or after timeseries
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public double GetValue(DateTime Time)
    {
      int index = GetIndex(Time);
      if (index < 0 || index > Count - 1)
        return DeleteValue;
      else
        return Items[index];
    }


    /// <summary>
    /// Returns the values between the starttime and endtime. Both inclusive.
    /// If the times exceed the timeseries the array is filled with deletevalues
    /// </summary>
    /// <param name="StartTime"></param>
    /// <param name="EndTime"></param>
    /// <returns></returns>
    public double[] GetValues(DateTime StartTime, DateTime EndTime)
    {

      int startindex = GetIndex(StartTime);
      int endindex = GetIndex(EndTime);

      double[] ToReturn = new double[endindex - startindex+1];

      int local = 0;
      for (int i = startindex; i <= endindex; i++)
      {
        if (i < 0 || i > Count - 1)
          ToReturn[local] = DeleteValue;
        else
          ToReturn[local] = Items[i];
        local++;
      }

      return ToReturn;
    }



    private DateTime _StartTime = DateTime.MinValue;
    /// <summary>
    /// Gets or sets the starttime
    /// </summary>
    public DateTime StartTime
    {
      get { return _StartTime; }
      set
      {
        if (_StartTime != value)
        {
          _StartTime = value;
          RaisePropertyChanged("StartTime");
        }
      }
    }

    /// <summary>
    /// Gets the end time
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        return GetTime(Count - 1);
      }
    }

    private DateTime _OffSetStartTime;
    public DateTime OffSetStartTime
    {
      get { return _OffSetStartTime; }
      set
      {
        if (_OffSetStartTime != value)
        {
          _OffSetStartTime = value;
          RaisePropertyChanged("OffSetStartTime");
        }
      }
    }

    private DateTime _OffSetEndTime;
    public DateTime OffSetEndTime
    {
      get { return _OffSetEndTime; }
      set
      {
        if (_OffSetEndTime != value)
        {
          _OffSetEndTime = value;
          RaisePropertyChanged("OffSetEndTime");
        }
      }
    }
    
    

    private int _TimeStepMultiplier=1;
    /// <summary>
    /// Get or sets the timestep multiplier
    /// </summary>
    public int TimeStepMultiplier
    {
      get { return _TimeStepMultiplier; }
      set
      {
        if (_TimeStepMultiplier != value)
        {
          _TimeStepMultiplier = value;
          RaisePropertyChanged("TimeStepMultiplier");
        }
      }
    }

    


  }
}
