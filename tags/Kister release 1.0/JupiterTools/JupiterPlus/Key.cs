using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class Key
  {
    public Type DotNetType { get; set; }
    public string Value { get; set; }

    public Key(Type dotNetType, string val)
    {
      DotNetType = dotNetType;
      Value = val;
    }

    public int ToInt()
    {
      //if (DotNetType.Equals(typeof(int))
        return int.Parse(Value);
    }

  }
}
