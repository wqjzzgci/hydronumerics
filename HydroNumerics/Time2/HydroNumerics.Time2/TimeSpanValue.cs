using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace HydroNumerics.Time2
{
  [DataContract]
  public class TimeSpanValue:BaseViewModel
  {
    public TimeSpanValue()
    {
    }

    public TimeSpanValue(DateTime Start, DateTime End, double Value)
    {
      StartTime = Start;
      EndTime = End;
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
          NotifyPropertyChanged("StartTime");
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
          NotifyPropertyChanged("EndTime");
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
          NotifyPropertyChanged("Value");
        }
      }
    }

    public override bool Equals(object obj)
    {
      if (obj.GetType() != typeof(TimeSpanValue))
        return false;
      else
      {
        var tsv = obj as TimeSpanValue;
        return tsv.StartTime == StartTime && tsv.EndTime == EndTime && tsv.Value == Value;
      }
    }


    public override int GetHashCode()
    {
      return StartTime.GetHashCode() * EndTime.GetHashCode() * Value.GetHashCode();
    }

    public override string ToString()
    {
      return "Start = " + StartTime.ToString("yyyy-MM-dd HH:mm") + ", Value = " + Value.ToString();
    }
  }
}
