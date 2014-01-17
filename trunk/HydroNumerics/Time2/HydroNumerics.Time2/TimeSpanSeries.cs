using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.Time2
{
  [DataContract]
  public class TimeSpanSeries : BaseViewModel
  {

    [DataMember]
    public TimeStepUnit TimeStepSize { get; private set; }

    [DataMember]
    public ObservableCollection<TimeSpanValue> Items { get; private set; }


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

    public DateTime StartTime
    {
      get
      {
        return Items.First().StartTime;
      }
    }

    public DateTime EndTime
    {
      get
      {
        return Items.Last().EndTime;
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
