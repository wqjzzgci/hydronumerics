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

    SortedList<int, SortedList<int, float>> MonthlyValues = new SortedList<int,SortedList<int,float>>();

   

    
    #region Constructors
    public FixedTimeSeries()
    {
      TimeStepSize = TimeStepUnit.Month;
    }
    

    /// <summary>
    /// Gets the value at a point in time
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public List<float> GetValues(DateTime First, DateTime Last)
    {
      List<float> toreturn = new List<float>();

      for (int i = First.Year; i <= Last.Year; i++)
      {
        int startmonth = 1;
        if (i == First.Year)
          startmonth = First.Month;

        int lastmonth = 12;
        if (i == Last.Year)
          lastmonth = Last.Month;

        SortedList<int, float> currentyear;

        if (!MonthlyValues.TryGetValue(i, out currentyear))
        {
          if(i<MonthlyValues.Keys.First())
            currentyear = MonthlyValues.Values.Where(v => v.Count == 12).First(); //Recycle first complete year
          else
            currentyear = MonthlyValues.Values.Where(v => v.Count == 12).Last(); //Recycle last complete year
        }
        for (int j = startmonth; j <= lastmonth; j++)
        {
          if (currentyear.ContainsKey(j))
            toreturn.Add(currentyear[j]);
          else
            toreturn.Add((float)DeleteValue);
        }
      }
      return toreturn;
    }



    public void AddRange(DateTime Start, TimeSpan TimeStep, List<float> Values)
    {
      if(TimeStep == TimeSpan.FromDays(1))
      {
        float monthlyvalue=0;
        int daycounter =0;
        int currentyear= Start.Year;
        int currentmonth=Start.Month;

        for (int i =0;i<Values.Count();i++)
        {
          if (daycounter == DateTime.DaysInMonth(currentyear, currentmonth))
          {
            if (!MonthlyValues.ContainsKey(currentyear))
              MonthlyValues.Add(currentyear,new SortedList<int,float>());
            MonthlyValues[currentyear].Add(currentmonth, monthlyvalue / (86400f * DateTime.DaysInMonth(currentyear, currentmonth)));
            monthlyvalue =0;
            daycounter = 0;
            currentmonth++;
            if (currentmonth > 12)
            {
              currentmonth = 1;
              currentyear++;
            }
          }
          daycounter++;
          monthlyvalue += Values[i];
        }
        if (!MonthlyValues.ContainsKey(currentyear))
          MonthlyValues.Add(currentyear, new SortedList<int, float>());
        MonthlyValues[currentyear].Add(currentmonth, monthlyvalue / (86400f*DateTime.DaysInMonth(currentyear, currentmonth)));

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
        foreach (var kvpyear in MonthlyValues)
        {
          foreach (var kvpmonth in kvpyear.Value)
          {
            DateTime start = new DateTime(kvpyear.Key, kvpmonth.Key, 1);
            yield return new TimeSpanValue(start, start.AddMonths(1), kvpmonth.Value);
          }
        }
      }
    }


    public DateTime StartTime
    {
      get { return new DateTime(MonthlyValues.First().Key, MonthlyValues.First().Value.First().Key,1) ; }
    }

    /// <summary>
    /// Gets the End time
    /// </summary>
    public DateTime EndTime
    {
      get { return new DateTime(MonthlyValues.Last().Key, MonthlyValues.Last().Value.Last().Key,1) ; }
    }
    
    
    /// <summary>
    /// Gets the sum og the values
    /// </summary>
    public double Sum
    {
      get
      {
        return MonthlyValues.SelectMany(kvp=>kvp.Value.Select(m=>m.Value)).Sum();
      }
    }

    /// <summary>
    /// Gets the average of the values
    /// </summary>
    public double Average
    {
      get
      {
        return MonthlyValues.SelectMany(kvp => kvp.Value.Select(m => m.Value)).Average();
      }
    }

    /// <summary>
    /// Gets the maximum of the values
    /// </summary>
    public double Max
    {
      get
      {
        return MonthlyValues.SelectMany(kvp => kvp.Value.Select(m => m.Value)).Max();
      }
    }

    /// <summary>
    /// Gets the minimum of the values
    /// </summary>
    public double Min
    {
      get
      {
        return MonthlyValues.SelectMany(kvp => kvp.Value.Select(m => m.Value)).Min();
      }
    }

    #endregion
  }
}
