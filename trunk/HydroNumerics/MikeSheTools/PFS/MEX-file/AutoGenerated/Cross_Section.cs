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
  public partial class Cross_Section: PFSMapper
  {


    internal Cross_Section(PFSSection Section)
    {
      _pfsHandle = Section;

      Datas = new List<Data>();
      for (int i = 1; i <= Section.GetKeywordsNo("Data"); i++)
        Datas.Add(new Data(Section.GetKeyword("Data",i)));
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

    public Cross_Section()
    {
      _pfsHandle = new PFSSection("Cross_Section");

      _pfsHandle.AddKeyword(new PFSKeyword("CRSID", PFSParameterType.String, ""));
      _pfsHandle.AddKeyword(new PFSKeyword("TypeNo", PFSParameterType.Integer, 0));
      _pfsHandle.AddKeyword(new PFSKeyword("Description", PFSParameterType.Missing, ""));
    }

    public List<Data> Datas {get; private set;}
    public string CRSID1
    {
      get
      {
        return _pfsHandle.GetKeyword("CRSID", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("CRSID", 1).GetParameter(1).Value = value;
      }
    }

    public int TypeNo1
    {
      get
      {
        return _pfsHandle.GetKeyword("TypeNo", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("TypeNo", 1).GetParameter(1).Value = value;
      }
    }

    public string Description1
    {
      get
      {
        return _pfsHandle.GetKeyword("Description", 1).GetParameter(1).ToString();
      }
      set
      {
        _pfsHandle.GetKeyword("Description", 1).GetParameter(1).Value = value;
      }
    }

  }
}