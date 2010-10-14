using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class DatumFinder
  {

    public static IEnumerable<Treple<CrossSection, double, bool>> CrossSectionsWithNewDatum(IEnumerable<M11Branch> branches)
    {
      foreach(var b in branches)
        foreach (var c in b.CrossSections)
        {
          Treple<CrossSection, double, bool> newC = new Treple<CrossSection, double, bool>();
          newC.First = c;
          newC.Second = 10;
          newC.Third = true;
          yield return newC;
        }
    }
  }
}
