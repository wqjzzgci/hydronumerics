﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Core.Time
{
  [DataContract]
  public class TimeSpanSeries : BaseTimeSeries<TimeSpanValue>
  {


    #region Constructors
    public TimeSpanSeries():base()
    {
    }

    public TimeSpanSeries(IEnumerable<TimeSpanValue> Values)
      : base(Values)
    {

      if (Items.Count > 0)
        TimeStepSize = TSTools.GetTimeStep(Items[0].StartTime, Items[0].EndTime);
    }

    public  TimeSpanSeries(TimeStampSeries ts):this()
    {
      this.DeleteValue = ts.DeleteValue;
      TimeStepSize = ts.TimeStepSize;

      if (ts.Count > 0)
      {
        List<TimeSpanValue> templist = new List<TimeSpanValue>();
        templist.Add(new TimeSpanValue(ts.Items[0].Time.Subtract(ts.Items[1].Time.Subtract(ts.Items[0].Time)), ts.Items[0].Time, ts.Items[0].Value));

        for (int i = 1; i < ts.Count; i++)
        {
          templist.Add(new TimeSpanValue(ts.Items[i - 1].Time, ts.Items[i].Time, ts.Items[i].Value));
        }
        AddRange(templist);
      }
      if (Items.Count > 0)
        TimeStepSize = TSTools.GetTimeStep(Items[0].StartTime, Items[0].EndTime);

    }

    public TimeSpanSeries(TimeStampSeries ts, TimeSpan TimeStep)
      : this()
    {
      this.DeleteValue = ts.DeleteValue;
      TimeStepSize = ts.TimeStepSize;
      List<TimeSpanValue> templist = new List<TimeSpanValue>();

      for (int i = 0; i < ts.Count; i++)
      {
        templist.Add(new TimeSpanValue(ts.Items[i].Time.Subtract(TimeStep), ts.Items[i].Time, ts.Items[i].Value));
      }
      AddRange(templist);
      if(ts.Count!=0)
      TimeStepSize = TSTools.GetTimeStep(StartTime, StartTime.Add(TimeStep));

    }



    #endregion



    public void GapFill(InterpolationMethods Method, TimeSpan Timestep)
    {

      switch (Method)
      {
        case InterpolationMethods.Linear:
          break;
        case InterpolationMethods.CubicSpline:
          break;
        case InterpolationMethods.Nearest:
          break;
        case InterpolationMethods.DeleteValue:
          for (int i = Items.Count - 1; i > 0; i--)
          {
            while (Items[i - 1].EndTime != Items[i].StartTime)
            {
              double newvalue = DeleteValue;
              Items.Insert(i, new TimeSpanValue(Items[i].StartTime.Subtract(Timestep), Items[i].StartTime, newvalue));
            }
          }
          break;
        default:
          break;
      }
    }




    #region Properties


    public TimeStampSeries AsTimeStampSeries
    {
      get
      {
        List<TimeStampValue> ts = new List<TimeStampValue>();
        for (int i = 0; i < Count; i++)
        {
          ts.Add(new TimeStampValue(Items[i].StartTime, Items[i].Value));
          ts.Add(new TimeStampValue(Items[i].EndTime.AddTicks(-1), Items[i].Value));
          //if (i < Count - 1 && Items[i].EndTime != Items[i + 1].StartTime)
          //{
          //  ts.Add(new TimeStampValue(Items[i].EndTime, 0));
          //  ts.Add(new TimeStampValue(Items[i + 1].StartTime.AddTicks(-1), 0));
          //}
        }
        //if (ts.Count > 0)
        //{
        //  ts.Insert(0, new TimeStampValue(Items.First().StartTime.AddTicks(-1), 0));
        //  ts.Last().Time = ts.Last().Time.AddTicks(-1);
        //  ts.Add(new TimeStampValue(Items.Last().EndTime.AddTicks(-1), 0));
        //}
          return new TimeStampSeries(ts);
      }
    }


    /// <summary>
    /// Gets the start time of the first value
    /// </summary>
    public DateTime StartTime
    {
      get
      {
        return Items.First().StartTime;
      }
    }

    /// <summary>
    /// Gets the end time of the last value
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        return Items.Last().EndTime;
      }
    }


    #endregion
  }
}