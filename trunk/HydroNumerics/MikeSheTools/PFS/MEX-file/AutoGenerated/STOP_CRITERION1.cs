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
  public partial class STOP_CRITERION1: PFSMapper
  {


    internal STOP_CRITERION1(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
        case "TOTAL_VOLUME":
          TOTAL_VOLUME = new TOTAL_VOLUME(sub);
          break;
          default:
            _unMappedSections.Add(sub.Name);
          break;
        }
      }

    }

    public STOP_CRITERION1(string pfsname)
    {
      _pfsHandle = new PFSSection(pfsname);

      _pfsHandle.AddKeyword(new PFSKeyword("CRITERION_ID", PFSParameterType.String, ""));

      TOTAL_VOLUME = new TOTAL_VOLUME("TOTAL_VOLUME" );
      _pfsHandle.AddSection(TOTAL_VOLUME._pfsHandle);

    }

    public TOTAL_VOLUME TOTAL_VOLUME{get; private set;}

    public string CRITERION_ID
    {
      get
      {
        return _pfsHandle.GetKeyword("CRITERION_ID", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("CRITERION_ID", 1).GetParameter(1).Value = value;
      }
    }

  }
}