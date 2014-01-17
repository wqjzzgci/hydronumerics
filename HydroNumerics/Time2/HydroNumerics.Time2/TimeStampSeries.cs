using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.Time2
{
  public class TimeStampSeries:BaseViewModel
  {
    [DataMember]
    public TimeStepUnit TimeStepSize { get; private set; }

    [DataMember]
    public ObservableCollection<TimeStampValue> Items { get; private set; }


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

      if (Items.Count > 0)
        TimeStepSize = TSTools.GetTimeStep(Items[0].Time, Items[1].Time);
    }


    void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      //This should take if the values are actually changed before notifying
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
