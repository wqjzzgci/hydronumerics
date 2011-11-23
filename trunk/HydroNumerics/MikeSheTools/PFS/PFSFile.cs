using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS
{
  public class PFSFile:FileClass
  {

    private PFSClass pfsClass;
    protected PFSClass _pfsClass
    {
      get
      {
        return pfsClass;
      }
    }


    public PFSFile(string Sim11FileName):base(Sim11FileName)
    {
      pfsClass = new PFSClass(Path.GetFullPath(FileName));
    }

    /// <summary>
    /// Saves the .pfs file
    /// </summary>
    public override void Save()
    {
      string s = _pfsClass.ToString();
      using (StreamWriter sw = new StreamWriter( Path.GetFullPath(FileName), false, Encoding.Default))
      {
        sw.Write(s);
      }

    }

  }
}
