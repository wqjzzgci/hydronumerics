using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  internal class WetLandsReducer:BaseViewModel
  {

    private class Equation
    {
      public int StartMonth;
      public int EndMonth;
      public double Par1;
      public double Par2;
    }

    private Dictionary<int, Equation> equations = new Dictionary<int, Equation>();

    public void ReadConfiguration(XElement Configuration)
    {
      Name = Configuration.SafeParseString("Name");

      foreach (var eq in Configuration.Elements("Equation"))
      {
        var eqs =new Equation();
        eqs.StartMonth = eq.SafeParseInt("StartMonth")??1;
        eqs.EndMonth = eq.SafeParseInt("EndMonth") ?? 12;
        eqs.Par1 = eq.SafeParseDouble("Par1") ?? 3.882;
        eqs.Par2 = eq.SafeParseDouble("Par2") ?? 0.7753;

        if (eqs.StartMonth < eqs.EndMonth)
          for (int i = eqs.StartMonth; i <= eqs.EndMonth; i++)
            equations.Add(i, eqs);
        else
        {
          for (int i = eqs.StartMonth; i <= 12; i++)
            equations.Add(i, eqs);
          for (int i = 1; i <= eqs.EndMonth; i++)
            equations.Add(i, eqs);
        }
      }
    }

    public double GetReduction(DateTime CurrentTime, double flow)
    {
      var eq = equations[CurrentTime.Month];
      return Math.Pow(eq.Par1 * flow, eq.Par2);
    }
  }
}
