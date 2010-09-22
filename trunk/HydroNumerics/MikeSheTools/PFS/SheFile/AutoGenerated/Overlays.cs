using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.SheFile
{
  /// <summary>
  /// This is an autogenerated class. Do not edit. 
  /// If you want to add methods create a new partial class in another file
  /// </summary>
  public partial class Overlays: PFSMapper
  {

    private Foreground _foreground;
    private Background _background;
    private Current_Layer _current_Layer;

    internal Overlays(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
        case "Foreground":
          _foreground = new Foreground(sub);
          break;
        case "Background":
          _background = new Background(sub);
          break;
        case "Current_Layer":
          _current_Layer = new Current_Layer(sub);
          break;
          default:
            _unMappedSections.Add(sub.Name);
          break;
        }
      }
    }

    public Foreground Foreground
    {
     get { return _foreground; }
    }

    public Background Background
    {
     get { return _background; }
    }

    public Current_Layer Current_Layer
    {
     get { return _current_Layer; }
    }

    public int UseModelDomain
    {
      get
      {
        return _pfsHandle.GetKeyword("UseModelDomain", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("UseModelDomain", 1).GetParameter(1).Value = value;
      }
    }

  }
}