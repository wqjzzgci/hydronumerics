using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HydroNumerics.Core.Time
{

  public enum InterpolationMethods
  {
    Linear,
    CubicSpline,
    DeleteValue,
    Nearest


  }


  [DataContract(Name = "TimeStepUnit")]
  public enum TimeStepUnit
  {
    [EnumMember]
    Year=1,
    [EnumMember]
    Month,
    [EnumMember]
    Day,
    [EnumMember]
    Hour,
    [EnumMember]
    Minute,
    [EnumMember]
    Second,
    [EnumMember]
    None
  }
}
