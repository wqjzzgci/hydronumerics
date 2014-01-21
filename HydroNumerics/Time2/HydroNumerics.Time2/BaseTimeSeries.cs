using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Time2
{
  [DataContract]
  public class BaseTimeSeries:BaseViewModel
  {

    [DataMember]
    public TimeStepUnit TimeStepSize { get; protected set; }


    private double _DeleteValue = 1e-035;
    [DataMember]
    public double DeleteValue
    {
      get { return _DeleteValue; }
      set
      {
        if (_DeleteValue != value)
        {
          _DeleteValue = value;
          NotifyPropertyChanged("DeleteValue");
        }
      }
    }
    
  }
}
