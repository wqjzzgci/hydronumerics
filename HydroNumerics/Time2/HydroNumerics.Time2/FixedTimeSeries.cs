using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time2
{
  [DataContract]
  public class FixedTimeSeries : BaseTimeSeries
  {

    public List<float> values;

    #region Constructors
    public FixedTimeSeries()
    {
      TimeStepSize = TimeStepUnit.None;
    }

    public FixedTimeSeries(DateTime Start, TimeSpan TimeStep, IEnumerable<float> Values)
    {
      values = new List<float>(Values);
      StartTime = Start;
      this.TimeStep = TimeStep;
    }

    public void MoveToMonthly()
    {
      if (TimeStepSize != TimeStepUnit.Month)
      {
        var _MonthlyData = new TimeSpanSeries(TSTools.ChangeZoomLevel(new TimeSpanSeries(this.ToTimeSpanseries), TimeStepUnit.Month, true));
        TimeStepSize = TimeStepUnit.Month;
        values = new List<float>(_MonthlyData.Items.Select(v => (float)v.Value));
      }
    }

    /// <summary>
    /// Gets the value at a point in time
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public List<float> GetValues(DateTime First, DateTime Last)
    {
      List<float> toreturn = new List<float>();

      int start =0;
      int last = 0;
      if (this.TimeStepSize == TimeStepUnit.Month)
      {
        start = (First.Year - StartTime.Year) * 12 + First.Month - StartTime.Month;
        last = (Last.Year - StartTime.Year) * 12 + Last.Month - StartTime.Month;
      }
      else
      {
        start = (int)(First.Subtract(StartTime).Ticks / TimeStep.Ticks);
        last = (int)(Last.Subtract(StartTime).Ticks / TimeStep.Ticks);
      }

      for (int i = start; i <= last; i++)
      {
        toreturn.Add(values[i]);
      }
      return toreturn;
    }



    public void AddRange(DateTime Start, IEnumerable<float> Values)
    {
      if (EndTime == Start)
      {
        values.AddRange(Values);
      }
      else
      {
        var temp = Values.ToList();

        while (Start.AddTicks(TimeStep.Ticks * temp.Count) < StartTime)
          temp.Add((float)DeleteValue);
        temp.AddRange(values);
        values = temp;
        StartTime = Start;
      }
      NotifyPropertyChanged("EndTime");
      NotifyPropertyChanged("Sum");
      NotifyPropertyChanged("Average");
      NotifyPropertyChanged("Min");
      NotifyPropertyChanged("Max");
    }
    

    #endregion




    #region Properties

    /// <summary>
    /// Get the fixed time series as a TimeSpanseries
    /// </summary>
    public IEnumerable<TimeSpanValue> ToTimeSpanseries
    {
      get
      {
        for (int i = 0; i < values.Count; i++)
        {
          yield return new TimeSpanValue(StartTime.AddSeconds(i * TimeStep.TotalSeconds), StartTime.AddSeconds((i + 1) * TimeStep.TotalSeconds), values[i]);
        }
      }
    }

    private TimeSpan _TimeStep;
    public TimeSpan TimeStep
    {
      get { return _TimeStep; }
      set
      {
        if (_TimeStep != value)
        {
          _TimeStep = value;
          NotifyPropertyChanged("TimeStep");
        }
      }
    } 



    private DateTime _StartTime;
    [DataMember]
    public DateTime StartTime
    {
      get { return _StartTime; }
      private set
      {
        if (_StartTime != value)
        {
          _StartTime = value;
          NotifyPropertyChanged("StartTime");
        }
      }
    }

    /// <summary>
    /// Gets the End time
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        return StartTime.AddTicks(TimeStep.Ticks * values.Count);
      }
    }
    
    
    /// <summary>
    /// Gets the sum og the values
    /// </summary>
    public double Sum
    {
      get
      {
        return values.Sum();
      }
    }

    /// <summary>
    /// Gets the average of the values
    /// </summary>
    public double Average
    {
      get
      {
        return values.Average();
      }
    }

    /// <summary>
    /// Gets the maximum of the values
    /// </summary>
    public double Max
    {
      get
      {
        return values.Max();
      }
    }

    /// <summary>
    /// Gets the minimum of the values
    /// </summary>
    public double Min
    {
      get
      {
        return values.Min();
      }
    }

    #endregion
  }
}
