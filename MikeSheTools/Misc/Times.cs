using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Misc
{
  public class Times:TimeSeries<double>
  {
    public void Test()
    {
      _entries.Add(DateTime.Now, 10.1);
    }
  }
}
