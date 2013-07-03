using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  /// <summary>
  /// This is an autogenerated class. Do not edit. 
  /// If you want to add methods create a new partial class in another file
  /// </summary>
  public partial class MODFLOW: PFSMapper
  {


    internal MODFLOW(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
          default:
            _unMappedSections.Add(sub.Name);
          break;
        }
      }

    }

    public MODFLOW(string pfsname)
    {
      _pfsHandle = new PFSSection(pfsname);

      _pfsHandle.AddKeyword(new PFSKeyword("CalculateLevels", PFSParameterType.Integer, 0));

    }

    public int CalculateLevels
    {
      get
      {
        return _pfsHandle.GetKeyword("CalculateLevels", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("CalculateLevels", 1).GetParameter(1).Value = value;
      }
    }

  }
}
