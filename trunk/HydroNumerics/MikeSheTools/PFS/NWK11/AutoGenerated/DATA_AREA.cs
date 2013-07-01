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
  public partial class DATA_AREA: PFSMapper
  {


    internal DATA_AREA(PFSSection Section)
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

    public DATA_AREA()
    {
      _pfsHandle = new PFSSection("DATA_AREA");

      _pfsHandle.AddKeyword(new PFSKeyword("x0", PFSParameterType.Integer, 0));
      _pfsHandle.AddKeyword(new PFSKeyword("y0", PFSParameterType.Integer, 0));
      _pfsHandle.AddKeyword(new PFSKeyword("x1", PFSParameterType.Integer, 0));
      _pfsHandle.AddKeyword(new PFSKeyword("y1", PFSParameterType.Integer, 0));
      _pfsHandle.AddKeyword(new PFSKeyword("projection", PFSParameterType.String, ""));
    }

    public int x01
    {
      get
      {
        return _pfsHandle.GetKeyword("x0", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("x0", 1).GetParameter(1).Value = value;
      }
    }

    public int y01
    {
      get
      {
        return _pfsHandle.GetKeyword("y0", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("y0", 1).GetParameter(1).Value = value;
      }
    }

    public int x11
    {
      get
      {
        return _pfsHandle.GetKeyword("x1", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("x1", 1).GetParameter(1).Value = value;
      }
    }

    public int y11
    {
      get
      {
        return _pfsHandle.GetKeyword("y1", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("y1", 1).GetParameter(1).Value = value;
      }
    }

    public string projection1
    {
      get
      {
        return _pfsHandle.GetKeyword("projection", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("projection", 1).GetParameter(1).Value = value;
      }
    }

  }
}