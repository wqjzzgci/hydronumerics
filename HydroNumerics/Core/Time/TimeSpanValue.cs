using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using HydroNumerics.Core;


namespace HydroNumerics.Core.Time
{
  [DataContract]
  public class TimeSpanValue:GalaSoft.MvvmLight.ObservableObject
  {
    public TimeSpanValue()
    {
    }

    public TimeSpanValue(DateTime Start, DateTime End, double Value)
    {
      StartTime = Start;
      EndTime = End;
      if (End < Start)
        throw new ArgumentOutOfRangeException("End cannot be before start");
      this.Value = Value;
    }

    private bool isEnabled = true;

    /// <summary>
    /// Gets and sets IsEnabled;
    /// </summary>
    [DataMember]
    public bool IsEnabled
    {
      get { return isEnabled; }
      set
      {
        if (value != isEnabled)
        {
          isEnabled = value;
          RaisePropertyChanged("IsEnabled");
        }
      }
    }


    private DateTime startTime;

    /// <summary>
    /// Gets and sets StartTime;
    /// </summary>
    [DataMember]
    public DateTime StartTime
    {
      get { return startTime; }
      set
      {
        if (value != startTime)
        {
          startTime = value;
          RaisePropertyChanged("StartTime");
        }
      }
    }



    private DateTime endTime;

    /// <summary>
    /// Gets and sets EndTime;
    /// </summary>
    [DataMember]
    public DateTime EndTime
    {
      get { return endTime; }
      set
      {
        if (value != endTime)
        {
          endTime = value;
          RaisePropertyChanged("EndTime");
        }
      }
    }


    public TimeSpan Duration
    {
      get
      {
        return EndTime.Subtract(StartTime);
      }
    }

    private double _value;

    /// <summary>
    /// Gets and sets Value;
    /// </summary>
    [DataMember]
    public double Value
    {
      get { return _value; }
      set
      {
        if (value != _value)
        {
          _value = value;
          RaisePropertyChanged("Value");
        }
      }
    }

    public override bool Equals(object obj)
    {
      var tsv = obj as TimeSpanValue;
      if (tsv == null)
        return false;
      else
      {
        return tsv.StartTime == StartTime && tsv.EndTime == EndTime && tsv.Value == Value;
      }
    }


    public override int GetHashCode()
    {
      int result = 17;
      result = result * 37 + StartTime.GetHashCode();
      result = result * 37 + EndTime.GetHashCode();
      result = result * 37 + Value.GetHashCode();
      return result;
    }

    public override string ToString()
    {
      return "Start = " + StartTime.ToString("yyyy-MM-dd HH:mm") + ", Value = " + Value.ToString() + ", End = " + EndTime.ToString("yyyy-MM-dd HH:mm");
    }
  }
}
