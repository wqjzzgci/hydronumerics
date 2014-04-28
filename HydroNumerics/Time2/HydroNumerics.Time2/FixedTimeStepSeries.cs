using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HydroNumerics.Time2
{
  public class FixedTimeStepSeries:BaseTimeSeries
  {
    private List<double> Values = new List<double>();


    /// <summary>
    /// Gets the number of values
    /// </summary>
    public int Count
    {
      get { return Values.Count; }
    }

    /// <summary>
    /// Adds a value to the end of the series
    /// </summary>
    /// <param name="Value"></param>
    public void Add(double Value)
    {
      Values.Add(Value);
    }

    /// <summary>
    /// Adds a range of values to the end of the series
    /// </summary>
    /// <param name="values"></param>
    public void AddRange(IEnumerable<double> values)
    {
      Values.AddRange(values);

      NotifyPropertyChanged("Count");
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


    /// <summary>
    /// Gets the values
    /// </summary>
    public IList<double> values
    {
      get
      {
        return Values;
      }
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
    /// Gets the end time
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        return GetTime(Count - 1);
      }
    }

    /// <summary>
    /// Gets the sum
    /// </summary>
    public double Sum
    {
      get
      {
        return Values.Where(v=>v!=DeleteValue).Sum();
      }
    }

    /// <summary>
    /// Gets the average
    /// </summary>
    public double Average
    {
      get
      {
        return Values.Where(v=>v!=DeleteValue).Average();
      }
    }

    /// <summary>
    /// Gets the maximum value
    /// </summary>
    public double Max
    {
      get
      {
        return Values.Where(v=>v!=DeleteValue).Max();
      }
    }

    /// <summary>
    /// Gets the minimum value
    /// </summary>
    public double Min
    {
      get
      {
        return Values.Where(v=>v!=DeleteValue).Min();
      }
    }

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
    /// Returns the bR-squared value
    /// </summary>
    /// <param name="Other"></param>
    /// <returns></returns>
    public double? bR2(FixedTimeStepSeries Other)
    {
      double[] val1;
      double[] val2;
      AlignRemoveDeletevalues(Other, out val1, out val2);
      int c = val1.Count();
      if (c > 0)
      {
        var xdata = new double[] { 10, 20, 30 };
        var ydata = new double[] { 15, 20, 25 };

        var coeff = Fit.Line(val1, val2);
        return coeff[1]*TSTools.R2(val1, val2);
      }
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
        return val1.Average()/val2.Average();
      return null;
    }


    private void AlignRemoveDeletevalues(FixedTimeStepSeries Other, out double[] val1, out double[] val2)
    {
      List<double> var1 = new List<double>();
      List<double> var2 = new List<double>();
      if (Other.TimeStepSize == TimeStepSize & Other.TimeStepMultiplier == TimeStepMultiplier)
      {
        if (Other.StartTime == StartTime)
        {
          for (int i = 0; i < Math.Min(this.Count, Other.Count); i++)
          {
            if (this.Values[i] != DeleteValue & Other.values[i] != Other.DeleteValue)
            {
              var1.Add(Values[i]);
              var2.Add(Other.values[i]);
            }
          }
        }
      }

      val1 = var1.ToArray();
      val2 = var2.ToArray();
    }



    /// <summary>
    /// Gets the index for a certain time
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets the values closest to time Time. Returns Deletevalue if time is before or after timeseries
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public double GetValue(DateTime Time)
    {
      int index = GetIndex(Time);
      if (index < 0 || index > Values.Count - 1)
        return DeleteValue;
      else
        return Values[index];
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
          NotifyPropertyChanged("StartTime");
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
          NotifyPropertyChanged("TimeStepMultiplier");
        }
      }
    }
    


  }
}
