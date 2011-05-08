using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.PFS.SheFile;

using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.WellFile
{
  public class WELLDATA:PFSMapper
  {
    private List<Well> _wells = new List<Well>();

    internal WELLDATA(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);

        _wells.Add(new Well(sub));
      }
    }

    public List<Well> Wells
    {
      get { return _wells; }
    }


    /// <summary>
    /// Gets the cross sections file name
    /// </summary>
    public int NoWells
    {
      get
      {
        return _pfsHandle.GetKeyword("NoWells", 1).GetParameter(1).ToInt();
      }
    }


  }
}
