using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.MEX
{

  public class MOUSE_CROSS_SECTIONS: PFSMapper
  {

    private List<Cross_Section> _cross_Sections = new List<Cross_Section>();

    internal MOUSE_CROSS_SECTIONS(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
        case "Cross_Section":
          _cross_Sections.Add(new Cross_Section(sub));
          break;
          default:
            _unMappedSections.Add(sub.Name);
          break;
        }
      }
    }

    public List<Cross_Section> Cross_Sections
    {
     get { return _cross_Sections; }
    }


    public int SYNTAX_VERSION
    {
      get
      {
        return _pfsHandle.GetKeyword("SYNTAX_VERSION", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("SYNTAX_VERSION", 1).GetParameter(1).Value = value;
      }
    }

    public int UNIT_TYPE
    {
      get
      {
        return _pfsHandle.GetKeyword("UNIT_TYPE", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("UNIT_TYPE", 1).GetParameter(1).Value = value;
      }
    }

  }
}
