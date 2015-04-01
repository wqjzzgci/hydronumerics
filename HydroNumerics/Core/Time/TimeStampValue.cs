using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using HydroNumerics.Core;

namespace HydroNumerics.Core.Time
{
  [DataContract]
  public class TimeStampValue:GalaSoft.MvvmLight.ObservableObject
  {
    #region Constructors

    public TimeStampValue()
    { }

    public TimeStampValue(DateTime time, double Value)
    {
      this.Time = time;
      this.Value = Value;
    }


    #endregion

    #region Properties

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
          RaisePropertyChanged("Time");
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
          RaisePropertyChanged("Value");
        }
      }
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Returns true if both time and value are equal
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        var tsv = obj as TimeStampValue;
        if (tsv == null)
          return false;
        return tsv.Time == Time && tsv.Value == Value;
    }

    /// <summary>
    /// Gets a hash code from time and value
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      int result = 17;
      result = result * 37 + Time.GetHashCode();
      result = result * 37 + Value.GetHashCode();
      return result;
    }

    public override string ToString()
    {
      return "T= " + Time.ToString() + ", V= " + Value.ToString();
    }

    #endregion
  }
}
