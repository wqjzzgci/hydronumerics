using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Core.Time
{
  [DataContract]
  public class BaseTimeSeries<T>:BaseViewModel
  {

    private object Lock = new object();

    /// <summary>
    /// The function that picks the value out of the type.
    /// </summary>
//    [DataMember]
    protected Func<T, double> ValueGetter;


    public BaseTimeSeries(Func<T, double> ValueGetter)
    {
      this.ValueGetter = ValueGetter;
      TimeStepSize = TimeStepUnit.None;
      Items = new SmartCollection<T>();
      Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
    }

    public BaseTimeSeries(IEnumerable<T> Values, Func<T, double> ValueGetter)
    {
      this.ValueGetter = ValueGetter;
      TimeStepSize = TimeStepUnit.None;
      Items = new SmartCollection<T>(Values);
      Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
    }

    private string _Unit;
    [DataMember]
    public string Unit
    {
      get { return _Unit; }
      set
      {
        if (_Unit != value)
        {
          _Unit = value;
          RaisePropertyChanged("Unit");
        }
      }
    }

    protected void ResetStats()
    {
      _Sum = null;
      _Average = null;
      _Min = null;
      _Max = null;
      //This should check if the values are actually changed before notifying
      RaisePropertyChanged("StartTime");
      RaisePropertyChanged("EndTime");
      RaisePropertyChanged("Sum");
      RaisePropertyChanged("Average");
      RaisePropertyChanged("Min");
      RaisePropertyChanged("Max");
      RaisePropertyChanged("Count");
      RaisePropertyChanged("NonDeleteCount");
    }


    void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      ResetStats();
    }

    /// <summary>
    /// Adds a range of data to the Items collection. 
    /// </summary>
    /// <param name="Values"></param>
    public void AddRange(IEnumerable<T> Values)
    {
      Items.AddRange(Values);
      //List<T> temp = new List<T>(Items);
      //temp.AddRange(Values);
      //Items = new ObservableCollection<T>(temp);
      //Items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Items_CollectionChanged);
      //RaisePropertyChanged("Items");
      //ResetStats();
    }

    /// <summary>
    /// Gets the list of items
    /// </summary>
    [DataMember]
    public SmartCollection<T> Items { get; set; }

    /// <summary>
    /// Iterates the values
    /// </summary>
    public IEnumerable<double> Values
    {
      get
      {
        foreach(var v in Items)
          yield return ValueGetter(v);
      }
    }


    /// <summary>
    /// Gets and sets the unit of the time step size
    /// </summary>
    [DataMember]
    public TimeStepUnit TimeStepSize { get;  set; }


    private double _DeleteValue = 1e-035;
    /// <summary>
    /// Gets and set the delete value
    /// </summary>
    [DataMember]
    public double DeleteValue
    {
      get { return _DeleteValue; }
      set
      {
        if (_DeleteValue != value)
        {
          _DeleteValue = value;
          RaisePropertyChanged("DeleteValue");
        }
      }
    }

    private int? _NonDeleteCount;
    /// <summary>
    /// Gets the number of non delete values
    /// </summary>
    public int NonDeleteCount
    {
      get {
        if(!_NonDeleteCount.HasValue)
          lock (Lock)
            StatsLoop();        
        return _NonDeleteCount.Value; }
    }
    

    /// <summary>
    /// Gets the number of values. Also counts delete values
    /// </summary>
    public int Count
    {
      get { return Items.Count; }
    }

    /// <summary>
    /// Generates basic statistic values
    /// </summary>
    private void StatsLoop()
    {
      _Sum = 0;
      _Min = double.MaxValue;
      _Max = double.MinValue;
      _NonDeleteCount=0;

      for(int i =0;i<Items.Count;i++)
      {
        double currentvalue = ValueGetter(Items[i]);
        if (currentvalue != DeleteValue)
        {
          _NonDeleteCount++;
          _Sum += currentvalue;
          if (currentvalue < _Min)
          {
            _Min = currentvalue;
            _FirstMinValue = Items[i];
          }
          if (currentvalue > _Max)
          {
            _Max = currentvalue;
            _FirstMaxValue = Items[i];
          }
        }
      }
      _Average = _Sum.Value / _NonDeleteCount;
    }


    private double? _Sum;
    /// <summary>
    /// Gets the sum of the values
    /// </summary>
    public double Sum
    {
      get
      {
        if (!_Sum.HasValue)
          lock(Lock)
            StatsLoop();
        return _Sum.Value;
      }
    }


    private double? _Average;
    /// <summary>
    /// Gets the average of the values
    /// </summary>
    public double Average
    {
      get
      {
        if (!_Average.HasValue)
          lock (Lock)
            StatsLoop();
        return _Average.Value;
      }
    }

    private double? _Max;
    /// <summary>
    /// Gets the maximum of the values
    /// </summary>
    public double Max
    {
      get
      {
        if (!_Max.HasValue)
          lock (Lock)
            StatsLoop();
        return _Max.Value;
      }
    }

    private double? _Min;
    /// <summary>
    /// Gets the minimum of the values
    /// </summary>
    public double Min
    {
      get
      {
        if (!_Min.HasValue)
          lock (Lock)
            StatsLoop();
        return _Min.Value;
      }
    }

    private T _FirstMinValue;
    /// <summary>
    /// Gets the first occurence of the minimumvalue
    /// </summary>
    public T FirstMinValue
    {
      get
      {
        if (_FirstMinValue == null)
        {
          foreach (var v in Items)
            if (ValueGetter(v) == Min)
            {
              _FirstMinValue = v;
              break;
            }
        }
        return _FirstMinValue;
      }
    }


    private T _FirstMaxValue;
    /// <summary>
    /// Gets the first occurrence of the maximum value
    /// </summary>
    public T FirstMaxValue
    {
      get
      {
        if (_FirstMaxValue == null)
        {
          foreach (var v in Items)
            if (ValueGetter(v) == Max)
            {
              _FirstMaxValue = v;
              break;
            }
        }
        return _FirstMaxValue;
      }
    }
  }
}
