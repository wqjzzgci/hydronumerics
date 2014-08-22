using System;
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
      ValueGetter = new Func<TimeSpanValue, double>(T => T.Value);
    }

    public TimeSpanSeries(IEnumerable<TimeSpanValue> Values):base(Values)
    {
      ValueGetter = new Func<TimeSpanValue, double>(T => T.Value);

      if (Items.Count > 0)
        TimeStepSize = TSTools.GetTimeStep(Items[0].StartTime, Items[0].EndTime);
    }

    #endregion


    public void GapFill(InterpolationMethods Method)
    {
      if (this.TimeStepSize == TimeStepUnit.None)
        throw new Exception("Cannot GapFill when the timestep unit is not set");

      List<int> Xvalues = new List<int>();
      List<double> Yvalues = new List<double>();
      Xvalues.Add(0);
      Yvalues.Add(Items.First().Value);
      int counter = 1;

      for (int i = 1; i < Items.Count; i++)
      {
        if (Items[i - 1].EndTime == Items[i].StartTime)
        {
          Yvalues.Add(Items[i].Value);
          Xvalues.Add(counter);
        }
        counter++;
      }

      if (Method == InterpolationMethods.DeleteValue)
      {
        for (int i = 1; i < Yvalues.Count; i++)
        {
          for (int j = Xvalues[i - 1]; j < Xvalues[i]; j++)
            Items.Insert(j, new TimeSpanValue(Items[j - 1].EndTime, TSTools.GetNextTime(Items[j - 1].EndTime, this.TimeStepSize), DeleteValue));
        }
      }
      else
        throw new Exception("Not implemented yet");
    }



    #region Properties




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
