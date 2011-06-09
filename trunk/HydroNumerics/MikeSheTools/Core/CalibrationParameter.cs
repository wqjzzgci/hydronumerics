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
    private List<object> obj;

    public CalibrationParameter(string PropertyName, IEnumerable<object> os)
    {
      obj = new List<object>(os);
      propInfo = os.First().GetType().GetProperty(PropertyName);
    }


    public CalibrationParameter(string PropertyName, object o)
    {
      obj = new List<object>();
      obj.Add(o);
      propInfo = o.GetType().GetProperty(PropertyName);     
    }

    public string ShortName { get; set; }

    [DataMember]
    public string DisplayName { get; set; }
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
      return (double)propInfo.GetValue(obj.First(), null);
    }

    [OperationContract]
    public void SetValue(double value)
    {
      foreach (var o in obj)
        propInfo.SetValue(o, value, null);
    }
  }
}
