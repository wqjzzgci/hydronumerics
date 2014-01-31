using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time2
{
  public class TimeStampSeries:BaseTimeSeries
  {

    [DataMember]
    public ObservableCollection<TimeStampValue> Items { get; private set; }

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
    

    public TimeStampSeries()
    {
      TimeStepSize = TimeStepUnit.None;
      Items = new ObservableCollection<TimeStampValue>();
      Items.CollectionChanged+=new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
    }

    public TimeStampSeries(IEnumerable<TimeStampValue> Values)
    {
      Items = new ObservableCollection<TimeStampValue>(Values);
      Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

      if (Items.Count > 0 )
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



    void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (Items.Count > 1 & TimeStepSize == TimeStepUnit.None)
        TimeStepSize = TSTools.GetTimeStep(Items[0].Time, Items[1].Time);

      //This should take if the values are actually changed before notifying
      NotifyPropertyChanged("StartTime");
      NotifyPropertyChanged("EndTime");
      NotifyPropertyChanged("Sum");
      NotifyPropertyChanged("Average");
      NotifyPropertyChanged("Min");
      NotifyPropertyChanged("Max");

      dateIndex = null;
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

    public double Sum
    {
      get
      {
        return Items.Sum(e => e.Value);
      }
    }

    public double Average
    {
      get
      {
        return Items.Average(e => e.Value);
      }
    }

        public double Max
    {
      get
      {
        return Items.Max(e => e.Value);
      }
    }
        public double Min
    {
      get
      {
        return Items.Min(e => e.Value);
      }
    }

  }
}
