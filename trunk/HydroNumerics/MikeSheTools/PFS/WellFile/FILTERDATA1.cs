using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.PFS.Well
{
  public partial class FILTERDATA1
  {

    public FILTERITEM_1 AddFilterItem()
    {
      FILTERITEM_1 fi = new FILTERITEM_1("FILTERITEM_" + (this.NoFilters + 1));
      _pfsHandle.AddSection(fi._pfsHandle);
      NoFilters++;

      FILTERITEM_1s.Add(fi);

      return fi;

    }
  }
}
