using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Core.Time
{
  public class TimeStampSeries:BaseTimeSeries<TimeStampValue>
  {


    private Dictionary<DateTime,int> dateIndex;

    private Dictionary<DateTime, int> DateIndex
    {
      get
      {
        if (dateIndex == null)
        {
          dateIndex = new Dictionary<DateTime, int>();
          for (int i = 0; i < Items.Count; i++)
          {
            if (!dateIndex.ContainsKey(Items[i].Time.Date))
              dateIndex.Add(Items[i].Time.Date, i);
          }
        }
        return dateIndex;
      }
    }
    

    public TimeStampSeries():base(new Func<TimeStampValue, double>(T => T.Value))
    {
      TimeStepSize = TimeStepUnit.None;
    }

    public TimeStampSeries(IEnumerable<TimeStampValue> Values)
      : base(Values, new Func<TimeStampValue, double>(T => T.Value))
    {
      if (Items.Count > 1 & TimeStepSize == TimeStepUnit.None)
        TimeStepSize = TSTools.GetTimeStep(Items[0].Time, Items[1].Time);
    }


    public void GapFill(InterpolationMethods Method)
    {
      if (this.TimeStepSize == TimeStepUnit.None)
        throw new Exception("Cannot GapFill when the timestep unit is not set");

      List<int> Xvalues = new List<int>();
      List<double> Yvalues = new List<double>();
      Xvalues.Add(0);
      Yvalues.Add(Items.First().Value);
      int counter = 0;

      for (int i = 1; i < Items.Count; i++)
      {
        DateTime next =Items[i - 1].Time;
        while ( (next= TSTools.GetNextTime(next, this.TimeStepSize)) <= Items[i].Time)
        {
          counter++;
        }


        Yvalues.Add(Items[i].Value);
        Xvalues.Add(counter);

      }

      if (Method == InterpolationMethods.DeleteValue)
      {
        for (int i = 1; i < Yvalues.Count; i++)
        {
          for (int j = Xvalues[i - 1]+1; j < Xvalues[i]; j++)
            Items.Insert(j, new TimeStampValue(TSTools.GetNextTime(Items[j - 1].Time, this.TimeStepSize), DeleteValue));
        }
      }
      else
        throw new Exception("Not implemented yet");
    }





    public double GetValue(DateTime Time, InterpolationMethods interpolate)
    {
      double value= DeleteValue;
      int index;
      if (DateIndex.TryGetValue(Time.Date, out index))
        value = Items[index].Value;

      return value;
    }

    public DateTime StartTime
    {
      get
      {
        return Items.First().Time;
      }
    }

    public DateTime EndTime
    {
      get
      {
        return Items.Last().Time;
      }
    }



  }
}
