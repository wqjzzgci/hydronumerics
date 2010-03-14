using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.Misc;

namespace HydroNumerics.MikeSheTools.Core
{
  public class MikeSheWell:Well
  {
    public int Column { get; set; }
    public int Row { get; set; }

    public int Layer { get; set; }

    public MikeSheWell(string ID)
      : base(ID)
    {
 
    }

    public MikeSheWell(string ID, double UTMX, double UTMY)
      : base(ID,UTMX, UTMY)
    {

    }

  }
}
