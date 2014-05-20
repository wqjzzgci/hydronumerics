using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class DeliverabilityWell
  {
    public Element Eleme { get; set; }
    public double Pressure { get; set; }
    public double Resistance { get; set; }

    public override string ToString()
    {
      StringBuilder str = new StringBuilder(new string(' ', 100));
      str.Insert(0, Eleme.Name);
      str.Insert(35, "DELV");
      str.Insert(40, Resistance.ToString("0.0000E+00"));
      str.Insert(50, Pressure.ToString("0.0000E+00"));
      return str.ToString().TrimEnd();
    }
  }
}
