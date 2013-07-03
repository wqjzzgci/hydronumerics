using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public partial class BRANCHES
  {

    public branch AddBranch()
    {
      branch b = new branch("branch");
      _pfsHandle.AddSection(b._pfsHandle);
      this.branchs.Add(b);
      return b;
    }
  }
}
