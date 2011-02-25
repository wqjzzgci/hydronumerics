using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;

using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class COMPUTATIONAL_SETUP:PFSMapper
  {

    public List<Branch2> Branches { get; private set; }

    internal COMPUTATIONAL_SETUP(PFSSection section)
    {
      _pfsHandle = section;

      Branches = new List<Branch2>();

      for (int i=1;i<=_pfsHandle.GetSectionsNo(); i++)
        Branches.Add(new Branch2(_pfsHandle.GetSection(i)));
    }

    public bool SaveAllGridPoints
    {
      get { return _pfsHandle.GetKeyword(1).GetParameter(1).ToBoolean(); }
      set { _pfsHandle.GetKeyword(1).GetParameter(1).Value = value; }
    }


  }
}
