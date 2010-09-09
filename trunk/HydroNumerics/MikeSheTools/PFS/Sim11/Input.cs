using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;


namespace HydroNumerics.MikeSheTools.PFS.Sim11
{
  public class Input : PFSMapper
  {

    internal Input(PFSSection Section)
    {
      _pfsHandle = Section;

    }


    /// <summary>
    /// Gets the cross sections file name
    /// </summary>
    public string XNS11FileName
    {
      get
      {
        return _pfsHandle.GetKeyword("xs", 1).GetParameter(1).ToFileName();
      }
    }

    /// <summary>
    /// Gets the network file name
    /// </summary>
    public string NWK11FileName
    {
      get
      {
        return _pfsHandle.GetKeyword("nwk", 1).GetParameter(1).ToFileName();
      }
    }

    /// <summary>
    /// Gets the boundary file name
    /// </summary>
    public string BND11FileName
    {
      get
      {
        return _pfsHandle.GetKeyword("bnd", 1).GetParameter(1).ToFileName();
      }
    }
  }
}
