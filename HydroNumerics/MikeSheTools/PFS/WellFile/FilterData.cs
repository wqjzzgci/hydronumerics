using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.PFS.SheFile;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.WellFile
{
  public class FILTERDATA:PFSMapper
  {

    private List<FILTERITEM> _fILTERITEM = new List<FILTERITEM>();


    internal FILTERDATA(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);

        _fILTERITEM.Add(new FILTERITEM(sub));
      }
    }

    public int NoFilters
    {
      get
      {
        return _pfsHandle.GetKeyword("NoFilters", 1).GetParameter(1).ToInt();
      }
    }

    public List<FILTERITEM> FILTERITEMS
    {
      get
      {
        return _fILTERITEM;
      }
    }

  }
}
