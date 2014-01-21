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
  public class TimeSpanSeries : BaseTimeSeries
  {


    #region Constructors
    public TimeSpanSeries()
    {
      TimeStepSize = TimeStepUnit.None;
      Items = new ObservableCollection<TimeSpanValue>();
      Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
    }

    public TimeSpanSeries(IEnumerable<TimeSpanValue> Values)
    {
      Items = new ObservableCollection<TimeSpanValue>(Values);
      Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);

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


    void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      //This should check if the values are actually changed before notifying
      NotifyPropertyChanged("StartTime");
      NotifyPropertyChanged("EndTime");
      NotifyPropertyChanged("Sum");
      NotifyPropertyChanged("Average");
      NotifyPropertyChanged("Min");
      NotifyPropertyChanged("Max");
    }

    #region Properties

    /// <summary>
    /// This list holds the actual values
    /// </summary>
    [DataMember]
    public ObservableCollection<TimeSpanValue> Items { get; private set; }

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

    /// <summary>
    /// Gets the sum og the values
    /// </summary>
    public double Sum
    {
      get
      {
        return Items.Sum(e => e.Value);
      }
    }

    /// <summary>
    /// Gets the average of the values
    /// </summary>
    public double Average
    {
      get
      {
        return Items.Average(e => e.Value);
      }
    }

    /// <summary>
    /// Gets the maximum of the values
    /// </summary>
    public double Max
    {
      get
      {
        return Items.Max(e => e.Value);
      }
    }
 
    /// <summary>
    /// Gets the minimum of the values
    /// </summary>
    public double Min
    {
      get
      {
        return Items.Min(e => e.Value);
      }
    }

    #endregion
  }
}
