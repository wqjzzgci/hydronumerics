using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.PFS.SheFile;

using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.Well
{
  public partial class WELLDATA
  {

    public WELLNO_1 AddWell()
    {
      WELLNO_1 w = new WELLNO_1("WELLNO_" + (this.NoWells + 1));
      _pfsHandle.AddSection(w._pfsHandle);
      this.NoWells++;
      this.WELLNO_1s.Add(w);
      return w;
    }
  }
}
