using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS
{
  public class PFSFile
  {
        protected PFSClass _pfsClass;

        public PFSFile(string Sim11FileName)
    {
      FileName = Path.GetFullPath(Sim11FileName);
      _pfsClass = new PFSClass(FileName);
    }

    /// <summary>
    /// Saves the .she-file to a new name
    /// </summary>
    /// <param name="SheFileName"></param>
    public void SaveAs(string Sim11FileName)
    {
      FileName = Path.GetFullPath(Sim11FileName);
      Save();
    }

    /// <summary>
    /// Saves the .pfs file
    /// </summary>
    public void Save()
    {
      using (StreamWriter sw = new StreamWriter(FileName))
      {
        sw.Write(_pfsClass.ToString());
      }

    }

    /// <summary>
    /// Gets the name of the .pfs file
    /// </summary>
    public string FileName
    {
      get;
      private set;
    }

  }
}
