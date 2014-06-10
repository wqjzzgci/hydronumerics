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
  public partial class MOUSE_Files: PFSMapper
  {


    internal MOUSE_Files(PFSSection Section)
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

    public MOUSE_Files(string pfsname)
    {
      _pfsHandle = new PFSSection(pfsname);

      _pfsHandle.AddKeyword(new PFSKeyword("ADP_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("PWF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("MTF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("MJL_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("ERS_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("ERF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("UND_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("HGF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("TRP_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("DWF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("RPF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("RRF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("CRF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("ROF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("RSF_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("PRFreduced_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("Generate_joblist", PFSParameterType.Boolean, true));

      _pfsHandle.AddKeyword(new PFSKeyword("PRFcomplete_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("CRFcomplete_file", PFSParameterType.String, ""));

      _pfsHandle.AddKeyword(new PFSKeyword("NOFhotStart_file", PFSParameterType.String, ""));

    }

    public string ADP_file
    {
      get
      {
        return _pfsHandle.GetKeyword("ADP_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("ADP_file", 1).GetParameter(1).Value = value;
      }
    }

    public string PWF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("PWF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("PWF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string MTF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("MTF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("MTF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string MJL_file
    {
      get
      {
        return _pfsHandle.GetKeyword("MJL_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("MJL_file", 1).GetParameter(1).Value = value;
      }
    }

    public string ERS_file
    {
      get
      {
        return _pfsHandle.GetKeyword("ERS_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("ERS_file", 1).GetParameter(1).Value = value;
      }
    }

    public string ERF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("ERF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("ERF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string UND_file
    {
      get
      {
        return _pfsHandle.GetKeyword("UND_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("UND_file", 1).GetParameter(1).Value = value;
      }
    }

    public string HGF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("HGF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("HGF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string TRP_file
    {
      get
      {
        return _pfsHandle.GetKeyword("TRP_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("TRP_file", 1).GetParameter(1).Value = value;
      }
    }

    public string DWF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("DWF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("DWF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string RPF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("RPF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("RPF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string RRF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("RRF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("RRF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string CRF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("CRF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("CRF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string ROF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("ROF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("ROF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string RSF_file
    {
      get
      {
        return _pfsHandle.GetKeyword("RSF_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("RSF_file", 1).GetParameter(1).Value = value;
      }
    }

    public string PRFreduced_file
    {
      get
      {
        return _pfsHandle.GetKeyword("PRFreduced_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("PRFreduced_file", 1).GetParameter(1).Value = value;
      }
    }

    public bool Generate_joblist
    {
      get
      {
        return _pfsHandle.GetKeyword("Generate_joblist", 1).GetParameter(1).ToBoolean();
      }
      set
      {
        _pfsHandle.GetKeyword("Generate_joblist", 1).GetParameter(1).Value = value;
      }
    }

    public string PRFcomplete_file
    {
      get
      {
        return _pfsHandle.GetKeyword("PRFcomplete_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("PRFcomplete_file", 1).GetParameter(1).Value = value;
      }
    }

    public string CRFcomplete_file
    {
      get
      {
        return _pfsHandle.GetKeyword("CRFcomplete_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("CRFcomplete_file", 1).GetParameter(1).Value = value;
      }
    }

    public string NOFhotStart_file
    {
      get
      {
        return _pfsHandle.GetKeyword("NOFhotStart_file", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("NOFhotStart_file", 1).GetParameter(1).Value = value;
      }
    }

  }
}