using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using HydroNumerics.Core;

namespace HydroNumerics.Time2
{
  [DataContract]
  public class TimeStampValue:NotifyModel
  {

    public TimeStampValue()
    { }

    public TimeStampValue(DateTime time, double Value)
    {
      this.Time = time;
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
          NotifyPropertyChanged("IsEnabled");
        }
      }
    }


    private DateTime time;

    /// <summary>
    /// Gets and sets Time;
    /// </summary>
    [DataMember]
    public DateTime Time
    {
      get { return time; }
      set
      {
        if (value != time)
        {
          time = value;
          NotifyPropertyChanged("Time");
        }
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
          NotifyPropertyChanged("Value");
        }
      }
    }

    public override bool Equals(object obj)
    {
      if (obj.GetType() != typeof(TimeStampValue))
        return false;
      else
      {
        var tsv = obj as TimeStampValue;
        return tsv.Time == Time && tsv.Value == Value;
      }
    }


    public override int GetHashCode()
    {
      int result = 17;
      result = result * 37 + Time.GetHashCode();
      result = result * 37 + Value.GetHashCode();
      return result;
    }


  }
}
