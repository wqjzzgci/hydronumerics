using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.Core
{
  public class MikeSheWell:Well
  {
    public int Column { get; set; }
    public int Row { get; set; }

    /// <summary>
    /// Gets the layer. The layer where most of the screen is located
    /// </summary>
    public int Layer { get; set; }

    public MikeSheWell(string ID)
      : base(ID)
    {
 
    }

    public MikeSheWell(string ID, double UTMX, double UTMY)
      : base(ID,UTMX, UTMY)
    {

    }

    public MikeSheWell(string ID, double UTMX, double UTMY, MikeSheGridInfo MSGI)
      : this(ID, UTMX, UTMY)
    {
      int i;
      int j;

      MSGI.TryGetIndex(UTMX, UTMY, out i, out j);
      Column = i;
      Row = j;
    }

  }
}
