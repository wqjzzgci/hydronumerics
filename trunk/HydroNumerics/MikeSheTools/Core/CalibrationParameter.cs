using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace HydroNumerics.MikeSheTools.Core
{
  public class CalibrationParameter
  {
    private PropertyInfo propInfo;
    private object obj;
    

    public CalibrationParameter(string PropertyName, object o)
    {
      obj = o;
      propInfo = o.GetType().GetProperty(PropertyName);
     
    }
    
    [DataMember]
    public string DisplayName { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
    public double CurrentValue
    {
      get
      {
        return GetValue();
      }
      set
      {
        SetValue(value);
      }
    }

    [OperationContract]
    public double GetValue()
    {
      return (double)propInfo.GetValue(obj, null);
    }

    [OperationContract]
    public void SetValue(double value)
    {
      propInfo.SetValue(obj, value, null);
    }

  }
}
