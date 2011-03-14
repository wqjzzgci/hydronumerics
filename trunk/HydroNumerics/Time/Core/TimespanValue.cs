using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{

  [Serializable]
  [DataContract]
  public class TimespanValue : System.ComponentModel.INotifyPropertyChanged
  {

    [DataMember]
    private double val;
    [XmlAttribute]
    public double Value
    {
      get
      {
        return val;
      }
      set
      {
        val = value;
        NotifyPropertyChanged("Value");
      }
    }

    [XmlIgnore]
    public Timespan TimeSpan
    {
      get
      {
        return new Timespan(startTime, endTime);
      }
      set
      {
        startTime = value.Start;

        endTime = value.End;
        NotifyPropertyChanged("TimeSpan");
      }
    }

    [DataMember]
    private DateTime startTime;
    public DateTime StartTime
    {
      get
      {
        return startTime;
      }
      set
      {
        startTime = value;
        NotifyPropertyChanged("StartTime");
      }
    }

    [DataMember]
    private DateTime endTime;
    public DateTime EndTime
    {
      get
      {
        return endTime;
      }
      set
      {
        endTime = value;
        NotifyPropertyChanged("EndTime");
      }
    }

    private string _description;
    /// <summary>
    /// Gets and set a description
    /// </summary>
    [DataMember]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        if (_description != value)
        {
          _description = value;
          NotifyPropertyChanged("Description");
        }
      }
    }

    public TimespanValue()
    {

    }

    public TimespanValue(DateTime startTime, DateTime endTime, double value, string Description)
      : this(startTime, endTime, value)
    {
      this.Description = Description;
    }

    public TimespanValue(DateTime startTime, DateTime endTime, double value)
      : this()
    {
      this.startTime = startTime;
      this.endTime = endTime;
      this.val = value;
    }

    public TimespanValue(Timespan timespan, double value)
      : this()
    {
      this.startTime = timespan.Start;
      this.endTime = timespan.End;
      this.val = value;
    }

    public TimespanValue(TimespanValue obj)
      : this()
    {
      this.StartTime = obj.StartTime;
      this.EndTime = obj.EndTime;
      this.Value = obj.Value;
    }

    public override bool Equals(Object obj)
    {
      bool equals = true;
      if (obj == null || GetType() != obj.GetType()) return false;
      if (this.StartTime != ((TimespanValue)obj).StartTime) equals = false;
      if (this.EndTime != ((TimespanValue)obj).EndTime) equals = false;
      if (this.Value != ((TimespanValue)obj).Value) equals = false;

      return equals;
    }

    public override string ToString()
    {
      return "V= " + val + ". S= " + StartTime + ". E = " + EndTime;
    }

    #region INotifyPropertyChanged Members

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }

}
