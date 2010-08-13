using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.NWK11File
{
  public class MIKE_11_Network_editor:PFSMapper
  {
    POINTS _pOints;

    internal MIKE_11_Network_editor(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
          case "POINTS":
            _pOints = new POINTS(sub);
            break;
            break;
          default:
            _unMappedSections.Add(sub.Name);
            break;

        }
      }
        }
  }
}
