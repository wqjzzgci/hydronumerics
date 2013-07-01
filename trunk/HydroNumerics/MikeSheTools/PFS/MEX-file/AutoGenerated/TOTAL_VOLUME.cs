using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.MEX
{
  /// <summary>
  /// This is an autogenerated class. Do not edit. 
  /// If you want to add methods create a new partial class in another file
  /// </summary>
  public partial class TOTAL_VOLUME: PFSMapper
  {


    internal TOTAL_VOLUME(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
        case "OPERATOR":
          OPERATOR = new OPERATOR(sub);
          break;
          default:
            _unMappedSections.Add(sub.Name);
          break;
        }
      }

      VOL_LIMIT = new VOL_LIMIT(_pfsHandle.GetKeyword("VOL_LIMIT", 1));
    }

    public TOTAL_VOLUME()
    {
      _pfsHandle = new PFSSection("TOTAL_VOLUME");

      OPERATOR = new OPERATOR();
      _pfsHandle.AddSection(OPERATOR._pfsHandle);

    }

    public OPERATOR OPERATOR{get; private set;}

    public VOL_LIMIT VOL_LIMIT{get; private set;}
  }
}