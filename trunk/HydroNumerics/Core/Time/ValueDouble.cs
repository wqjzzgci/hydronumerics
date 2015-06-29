using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroNumerics.Core.Time
{
  public class ValueDouble:IValue
  {

    public ValueDouble(double Value)
    {
      _Value = Value;
    }


    private double _Value;
    public double Value
    {
      get
      {
        return _Value;
      }
      set
      {
        _Value = value;
      }


    }
  }
}
