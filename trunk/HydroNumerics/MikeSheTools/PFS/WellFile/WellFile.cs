using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.Well
{
  public class WellFile: PFSFile
  {

    List<string> _unMappedSections = new List<string>();

    public WellFile(string SheFileName)
      : base(SheFileName)
    {

      for (int i = 1; i <= _pfsClass.GetTargetsNo(); i++)
      {
        PFSSection sub = _pfsClass.GetTarget(i);
        switch (sub.Name)
        {
        case "MAP":
          MAP = new MAP(sub);
          break;
        case "SOIL":
          SOIL = new SOIL(sub);
          break;
        case "WEL_CFG":
          WEL_CFG = new WEL_CFG(sub);
          break;
          default:
            _unMappedSections.Add(sub.Name);
          break;
        }
      }

    }


    public MAP MAP{get; private set;}

    public SOIL SOIL{get; private set;}

    public WEL_CFG WEL_CFG{get; private set;}

  }
}
