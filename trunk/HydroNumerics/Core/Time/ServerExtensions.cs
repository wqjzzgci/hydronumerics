using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroNumerics.Core.Time
{
  static class ServerExtensions
  {
    public static void SaveAs(this TimeStampSeries ts, string Filename)
    {
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Filename))
      {
        foreach (var v in ts.Items)
          sw.WriteLine(v.Time.ToShortDateString() + "; " + v.Value);
      }
    }

  }
}
