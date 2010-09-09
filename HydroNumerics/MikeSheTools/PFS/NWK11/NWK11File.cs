using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class NWK11File:PFSFile 
  {
    private MIKE_11_Network_editor  _m11;

    public NWK11File(string SheFileName):base(SheFileName)
    {
      _m11 = new MIKE_11_Network_editor(_pfsClass.GetTarget(1));
    }

    /// <summary>
    /// Access to the entries in the .she-file
    /// </summary>
    public MIKE_11_Network_editor MIKE_11_Network_editor
    {
      get { return _m11; }
    }


  }
}
