using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Time2
{
  public class FixedTimeStepSeries:BaseTimeSeries
  {

    public int Count
    {
      get { return Values.Count; }
    }
    


    public void AddRange(IEnumerable<double> values)
    {
      Values.AddRange(values);

      NotifyPropertyChanged("Count");
    }

    public void AddRange(DateTime Start, IEnumerable<double> values)
    {
      if (StartTime == DateTime.MinValue)
      {
        StartTime = Start;
        Values.AddRange(values);
      }
      int nextindex = GetIndex(Start);

      if (nextindex > Values.Count)
      {
        for (int i = Values.Count; i < nextindex; i++)
          Values.Add(DeleteValue);
        Values.AddRange(values);
      }
      else if (nextindex < 0)
      {
        var previous = Values.ToArray();
        var previoustime = StartTime;
        Values.Clear();
        StartTime = DateTime.MinValue;
        AddRange(Start, values);
        AddRange(previoustime, previous);
      }
      else
      {
        foreach(var v in values)
        {
          if(nextindex < Values.Count)
            Values[nextindex]= v;
          else
            Values.Add(v);
          nextindex++;
        }
      }
      NotifyPropertyChanged("Count");
    }


    public IEnumerable<double> values
    {
      get
      {
        return Values.AsEnumerable<double>() ;
      }
    }

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

    public DateTime EndTime
    {
      get
      {
        return GetTime(Count - 1);
      }
    }

    public double Sum
    {
      get
      {
        return Values.Sum();
      }
    }

    public double Average
    {
      get
      {
        return Values.Average();
      }
    }

    public double Max
    {
      get
      {
        return Values.Max();
      }
    }

    public double Min
    {
      get
      {
        return Values.Min();
      }
    }




    private int GetIndex(DateTime Time)
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

    public double GetValue(DateTime Time)
    {
      int index = GetIndex(Time);
      if (index < 0 || index > Values.Count - 1)
        return DeleteValue;
      else
        return Values[index];
    }

    private List<double> Values = new List<double>();

    private DateTime _StartTime = DateTime.MinValue;
    public DateTime StartTime
    {
      get { return _StartTime; }
      set
      {
        if (_StartTime != value)
        {
          _StartTime = value;
          NotifyPropertyChanged("StartTime");
        }
      }
    }

    private int _TimeStepMultiplier=1;
    public int TimeStepMultiplier
    {
      get { return _TimeStepMultiplier; }
      set
      {
        if (_TimeStepMultiplier != value)
        {
          _TimeStepMultiplier = value;
          NotifyPropertyChanged("TimeStepMultiplier");
        }
      }
    }
    


  }
}
