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
  public partial class MOUSE_WEIRS: PFSMapper
  {


    internal MOUSE_WEIRS(PFSSection Section)
    {
      _pfsHandle = Section;

      Weirs = new List<Weir>();
      for (int i = 1; i <= Section.GetKeywordsNo("Weir"); i++)
        Weirs.Add(new Weir(Section.GetKeyword("Weir",i)));
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

      WeirHeader = new WeirHeader(_pfsHandle.GetKeyword("WeirHeader", 1));
    }

    public MOUSE_WEIRS(string pfsname)
    {
      _pfsHandle = new PFSSection(pfsname);

      Weirs = new List<Weir>();
      _pfsHandle.AddKeyword(new PFSKeyword("SYNTAX_VERSION", PFSParameterType.Integer, 0));

      _pfsHandle.AddKeyword(new PFSKeyword("UNIT_TYPE", PFSParameterType.Integer, 0));

      WeirHeader = new WeirHeader("WeirHeader");
      _pfsHandle.AddKeyword(WeirHeader._keyword);
    }

    public WeirHeader WeirHeader{get; private set;}
    public List<Weir> Weirs {get; private set;}
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
