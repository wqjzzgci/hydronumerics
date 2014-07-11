using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.PFS.Well
{
  public partial class WELLFIELD
  {

    public WellFieldItem AddWellField()
    {
      WellFieldItem wi = new WellFieldItem("WELLNO_" + (this.NoWellFields + 1));
      NoWellFields++;
      wi.WellFieldID = NoWellFields;
      _pfsHandle.AddSection(wi._pfsHandle);
      WellFieldItems.Add(wi);
      return wi;
    }

  }
}
