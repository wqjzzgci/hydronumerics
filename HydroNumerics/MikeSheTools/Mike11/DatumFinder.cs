using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class DatumFinder
  {

    public static IEnumerable<Tuple<CrossSection, double, bool>> CrossSectionsWithNewDatum(IEnumerable<M11Branch> branches)
    {
      foreach(var b in branches)
        foreach (var c in b.CrossSections)
        {
          Tuple<CrossSection, double, bool> newC = new Tuple<CrossSection, double, bool>(c,10,true);
          yield return newC;
        }
    }
  }
}
