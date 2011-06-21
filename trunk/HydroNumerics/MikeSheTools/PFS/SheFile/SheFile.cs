using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.SheFile
{
  public class InputFile:PFSFile
  {
    private MIKESHE_FLOWMODEL _mshe;
    
    public InputFile(string SheFileName):base(SheFileName)
    {
    }


    /// <summary>
    /// Access to the entries in the .she-file
    /// </summary>
    public MIKESHE_FLOWMODEL MIKESHE_FLOWMODEL
    {
      get 
      { 
        if (_mshe==null)
          _mshe = new MIKESHE_FLOWMODEL(_pfsClass.GetTarget(1));

        return _mshe; }
    }


  }
}
